using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Coderr.Client.Config;
using Coderr.Client.Contracts;
using Newtonsoft.Json;

namespace Coderr.Client.Uploaders
{
    /// <summary>
    ///     Used to upload the report to the Coderr Server.
    /// </summary>
    /// <remarks>
    ///     Will queue reports and try to upload them at a later point if the server is not available (or there are no internet
    ///     connection).
    /// </remarks>
    public class UploadToCoderr : IReportUploader
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly IUploadQueue<FeedbackDTO> _feedbackQueue;
        private readonly Func<bool> _queueReportsAccessor;
        private readonly IUploadQueue<ErrorReportDTO> _reportQueue;
        private readonly Uri _reportUri, _feedbackUri;
        private readonly string _sharedSecret;
        private readonly Func<bool> _throwExceptionsAccessor;
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _uploadFunc;
        private readonly bool _signReport = true;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UploadToCoderr" /> class.
        /// </summary>
        /// <param name="oneTrueHost">
        ///     Uri to the root of the codeRR web. Example.
        ///     <code>http://yourWebServer/codeRR/</code>
        /// </param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="sharedSecret">The shared secret.</param>
        /// <exception cref="System.ArgumentNullException">apiKey</exception>
        public UploadToCoderr(Uri oneTrueHost, string apiKey, string sharedSecret)
        {
            if (string.IsNullOrEmpty(apiKey)) throw new ArgumentNullException("apiKey");
            if (string.IsNullOrEmpty(sharedSecret)) throw new ArgumentNullException("sharedSecret");

            if (!oneTrueHost.AbsolutePath.EndsWith("/"))
                oneTrueHost = new Uri(oneTrueHost + "/");

            if (oneTrueHost.AbsolutePath.Contains("/receiver/"))
            {
                throw new ArgumentException(
                    "The codeRR URI should not contain the reporting area '/receiver/', but should point at the site root.");
            }

            _reportUri = new Uri(oneTrueHost, "receiver/report/" + apiKey + "/");
            _feedbackUri = new Uri(oneTrueHost, "receiver/report/" + apiKey + "/feedback/");
            _sharedSecret = sharedSecret;

            _feedbackQueue = new UploadQueue<FeedbackDTO>(UploadFeedbackNow);
            _feedbackQueue.UploadFailed += OnUploadFailed;

            _reportQueue = new UploadQueue<ErrorReportDTO>(UploadReportNow);
            _reportQueue.UploadFailed += OnUploadFailed;

            _uploadFunc = message => _client.SendAsync(message);
            _queueReportsAccessor = () => Err.Configuration.QueueReports;
            _throwExceptionsAccessor = () => Err.Configuration.ThrowExceptions;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UploadToCoderr" /> class.
        /// </summary>
        /// <param name="oneTrueHost">
        ///     Uri to the root of the codeRR web. Example.
        ///     <code>http://yourWebServer/codeRR/</code>
        /// </param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="sharedSecret">The shared secret.</param>
        /// <param name="config">parameters for tests</param>
        /// <exception cref="System.ArgumentNullException">apiKey</exception>
        internal UploadToCoderr(Uri oneTrueHost, string apiKey, string sharedSecret, TestConfig config)
            : this(oneTrueHost, apiKey, sharedSecret)
        {
            _queueReportsAccessor = config.QueueReportsAccessor;
            _throwExceptionsAccessor = config.ThrowExceptionsAccessor;
            _uploadFunc = config.UploadFunc;
            _feedbackQueue = config.FeedbackQueue;
            _reportQueue = config.ReportQueue;
        }

        /// <summary>
        ///     We've tried several times and failed to upload the report.
        /// </summary>
        public event EventHandler<UploadReportFailedEventArgs> UploadFailed;

        /// <summary>
        ///     Upload feedback to Coderr.
        /// </summary>
        /// <param name="feedback">feedback to upload</param>
        /// <remarks>
        ///     <para>
        ///         Will either upload directly or queue if the <see cref="CoderrConfiguration.QueueReports" /> is true
        ///     </para>
        /// </remarks>
        public void UploadFeedback(FeedbackDTO feedback)
        {
            if (_queueReportsAccessor())
                _feedbackQueue.Enqueue(feedback);
            else
                UploadFeedbackNow(feedback);
        }

        /// <summary>
        ///     Upload report
        /// </summary>
        /// <param name="report">Report DTO</param>
        /// <remarks>
        ///     <para>
        ///         Will either upload directly or queue if the <see cref="CoderrConfiguration.QueueReports" /> is true
        ///     </para>
        /// </remarks>
        public void UploadReport(ErrorReportDTO report)
        {
            if (_queueReportsAccessor())
                _reportQueue.Enqueue(report);
            else
                UploadReportNow(report);
        }

        internal HttpRequestMessage CreateRequest(string uri, object dto)
        {
            var reportJson = JsonConvert.SerializeObject(dto, Formatting.None,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None,
                    ContractResolver =
                        new IncludeNonPublicMembersContractResolver()
                });
            var jsonBytes = Encoding.UTF8.GetBytes(reportJson);


            var ms = new MemoryStream();

            using (var gzip = new GZipStream(ms, CompressionLevel.Optimal, true))
            {
                gzip.Write(jsonBytes, 0, jsonBytes.Length);
                gzip.Flush();
            }

            var requestBody = ms.ToArray();

            var content = new ByteArrayContent(requestBody);
            if (_signReport)
            {
                var hashAlgo = new HMACSHA256(Encoding.UTF8.GetBytes(_sharedSecret));
                var hash = hashAlgo.ComputeHash(requestBody, 0, requestBody.Length);
                var signature = Convert.ToBase64String(hash);

                // this is version 2. Need to push that on the server side first
                // and we also need to calc the signature on the JSON and not the gzipped content
                //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                //content.Headers.ContentEncoding.Add("gzip");

                uri = uri + "?sig=" + signature + "&v=1&throw=" + (Err.Configuration.ThrowExceptions ? "1" : "0");
            }
            else
            {
                uri = uri + "?v=1&throw=" + (Err.Configuration.ThrowExceptions ? "1" : "0");
            }

            return new HttpRequestMessage(HttpMethod.Post, uri) {Content = content};
        }

        internal void UploadFeedbackNow(FeedbackDTO dto)
        {
            PostData(_feedbackUri.ToString(), dto)
                .GetAwaiter()
                .GetResult();
        }

        private void OnUploadFailed(object sender, UploadReportFailedEventArgs e)
        {
            UploadFailed?.Invoke(this, e);
        }

        private async Task PostData(string uri, object dto)
        {
            var msg = CreateRequest(uri, dto);
            try
            {
                var response = await _uploadFunc(msg);
                if (!response.IsSuccessStatusCode)
                    ProcessResponseError(response);
            }
            catch (Exception ex)
            {
                OnUploadFailed(this, new UploadReportFailedEventArgs(ex, dto));
                if (_throwExceptionsAccessor() || _queueReportsAccessor())
                    throw;
            }
        }

        private void ProcessResponseError(HttpResponseMessage response)
        {
            var title = response.ReasonPhrase;
            var description = response.Content.ReadAsStringAsync().Result;

            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedAccessException($"{response.StatusCode}: {title}\r\n{description}");
                case HttpStatusCode.NotFound:
                    //legacy handling of 404. Misconfigured web servers will report 404,
                    //so remove the usage to avoid ambiguity
                    if (title.IndexOf("key", StringComparison.OrdinalIgnoreCase) != -1)
                        throw new InvalidApplicationKeyException($"{response.StatusCode}: {title}\r\n{description}");
                    throw new InvalidOperationException($"{response.StatusCode}: {title}\r\n{description}");
                default:
                    if (response.StatusCode == HttpStatusCode.BadRequest && title.Contains("APP_KEY"))
                        throw new InvalidApplicationKeyException($"{response.StatusCode}: {title}\r\n{description}");
                    throw new InvalidOperationException($"{response.StatusCode}: {title}\r\n{description}");
            }
        }

        private void UploadReportNow(ErrorReportDTO dto)
        {
            PostData(_reportUri.ToString(), dto)
                .GetAwaiter()
                .GetResult();
        }
    }
}