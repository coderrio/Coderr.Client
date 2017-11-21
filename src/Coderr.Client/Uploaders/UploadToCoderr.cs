using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using codeRR.Client.Contracts;
using codeRR.Client.Converters;
using Coderr.Client.Uploaders;

namespace codeRR.Client.Uploaders
{
    /// <summary>
    ///     Upload reports to our web site.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         To use this reporter you have to create an account at https://coderrapp.com and then register an application
    ///         to get an application key and a shared secret.
    ///     </para>
    ///     <para>
    ///         Finally use that information to configure this reporter:
    ///     </para>
    ///     <example>
    ///         <code>
    /// 
    /// var uri = new Uri("http://coderr.yourdomain.com/");
    /// Err.Configuration.Uploaders.Add(new UploadToCoderr(uri, "yourAppKey",
    ///                                                             "yourSharedSecret"));
    /// Err.Configuration.UserInteraction.AskUserForDetails = false;
    /// </code>
    ///     </example>
    ///     <para>
    ///         Reports will be queued internally if there are no internet connection available. The queue have the same
    ///         constrains as in the global
    ///         configuration.  Thus the olders reports will be dropped if the connection is down and the queue limit have been
    ///         reached.
    ///     </para>
    ///     <para>
    ///         Nothing in the queue is persisted. Thus if the application are stopped before all reports have been uploaded,
    ///         they will
    ///         be lost. This is only a problem if the codeRR service is slow or if the internet connection is down.
    ///     </para>
    ///     <para>
    ///         This uploader will check the internet settings (that are configured in the windows control panel) to see if an
    ///         internet proxy
    ///         is required. If it is, the HttpClient will be configured to use it. So this library should work behind
    ///         corporate firewalls.
    ///     </para>
    /// </remarks>
    public class UploadToCoderr : IReportUploader, IDisposable
    {
        private readonly Func<bool> _queueReportsAccessor = () => Err.Configuration.QueueReports;
        private readonly Uri _reportUri, _feedbackUri;
        private UploadQueue<FeedbackDTO> _feedbackQueue;
        private UploadQueue<ErrorReportDTO> _reportQueue;


        /// <summary>
        ///     Initializes a new instance of the <see cref="UploadToCoderr" /> class.
        /// </summary>
        /// <param name="coderrServerAddress">
        ///     Uri to the root of the codeRR web. Example.
        ///     <code>http://yourWebServer/coderr/</code>
        /// </param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="sharedSecret">The shared secret.</param>
        /// <exception cref="System.ArgumentNullException">apiKey</exception>
        public UploadToCoderr(Uri coderrServerAddress, string apiKey, string sharedSecret)
        {
            if (string.IsNullOrEmpty(apiKey)) throw new ArgumentNullException("apiKey");
            if (string.IsNullOrEmpty(sharedSecret)) throw new ArgumentNullException("sharedSecret");

            if (coderrServerAddress.AbsolutePath.Contains("/receiver/"))
                throw new ArgumentException(
                    "The codeRR URI should not contain the reporting area '/receiver/', but should point at the site root.");
            if (!coderrServerAddress.AbsolutePath.EndsWith("/"))
                coderrServerAddress = new Uri(coderrServerAddress + "/");

            _reportUri = new Uri(coderrServerAddress, "receiver/report/" + apiKey + "/");
            _feedbackUri = new Uri(coderrServerAddress, "receiver/report/" + apiKey + "/feedback/");
            ApiKey = apiKey;
            SharedSecret = sharedSecret;
            _feedbackQueue = new UploadQueue<FeedbackDTO>(TryUploadFeedbackNow);
            _feedbackQueue.UploadFailed += OnUploadFailed;
            _reportQueue = new UploadQueue<ErrorReportDTO>(TryUploadReportNow);
            _reportQueue.UploadFailed += OnUploadFailed;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UploadToCoderr" /> class.
        /// </summary>
        /// <param name="coderrServerAddress">
        ///     Uri to the root of the codeRR web. Example.
        ///     <code>http://yourWebServer/coderr/</code>
        /// </param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="sharedSecret">The shared secret.</param>
        /// <param name="queueReportsAccessor">Used to access the property that determines if the reports should be queued or not.</param>
        /// <exception cref="ArgumentNullException">queueReportsAccessor</exception>
        public UploadToCoderr(Uri coderrServerAddress, string apiKey, string sharedSecret,
            Func<bool> queueReportsAccessor)
            : this(coderrServerAddress, apiKey, sharedSecret)
        {
            _queueReportsAccessor = queueReportsAccessor ?? throw new ArgumentNullException(nameof(queueReportsAccessor));
        }

        public string ApiKey { get; set; }

        /// <summary>
        ///     Max number of upload attempts per report
        /// </summary>
        public int MaxAttempts
        {
            get => _reportQueue.MaxAttempts;
            set
            {
                _reportQueue.MaxAttempts = value;
                _feedbackQueue.MaxAttempts = value;
            }
        }

        /// <summary>
        ///     Max number of items that may wait in queue to get upload.
        /// </summary>
        public int MaxQueueSize
        {
            get => _reportQueue.MaxQueueSize;
            set
            {
                _reportQueue.MaxQueueSize = value;

                _feedbackQueue.MaxQueueSize = value;
            }
        }

        /// <summary>
        ///     Amount of time to wait between each attempt
        /// </summary>
        public TimeSpan RetryInterval
        {
            get => _reportQueue.RetryInterval;
            set
            {
                _reportQueue.RetryInterval = value;
                _feedbackQueue.RetryInterval = value;
            }
        }

        public string SharedSecret { get; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     Upload the report to the web service.
        /// </summary>
        /// <param name="report">CreateReport to submit</param>
        public void UploadReport(ErrorReportDTO report)
        {
            if (report == null) throw new ArgumentNullException("report");

            if (!NetworkInterface.GetIsNetworkAvailable() || _queueReportsAccessor())
                _reportQueue.Add(report);
            else
                TryUploadReportNow(report);
        }

        /// <summary>
        ///     Failed to deliver DTO within the given parameters.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public event EventHandler<UploadReportFailedEventArgs> UploadFailed;


        /// <summary>
        ///     Send feedback for a previously submitted error report.
        /// </summary>
        /// <param name="feedback">Feedback to send</param>
        /// <remarks>
        ///     <para>
        ///         Will be queued internally (in memory) if the OS reports that there are no Internet connection available.
        ///     </para>
        /// </remarks>
        public void UploadFeedback(FeedbackDTO feedback)
        {
            if (feedback == null) throw new ArgumentNullException("feedback");

            if (!NetworkInterface.GetIsNetworkAvailable() || _queueReportsAccessor())
                _feedbackQueue.Add(feedback);
            else
                TryUploadFeedbackNow(feedback);
        }

        /// <summary>
        ///     Try to upload a report directly
        /// </summary>
        /// <param name="feedback">Report to upload</param>
        /// <exception cref="WebException">No Internet connection is available; Destination server did not accept the report.</exception>
        public void TryUploadFeedbackNow(FeedbackDTO feedback)
        {
            if (feedback == null) throw new ArgumentNullException("feedback");
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new UploadFailedException("Not connected, try again later.");

            var reportJson = JsonConvert.SerializeObject(feedback, Formatting.None,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None,
                    ContractResolver =
                        new IncludeNonPublicMembersContractResolver()
                });
            var buffer = Encoding.UTF8.GetBytes(reportJson);
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString(2);
            var hashAlgo = new HMACSHA256(Encoding.UTF8.GetBytes(SharedSecret));
            var hash = hashAlgo.ComputeHash(buffer);
            var signature = Convert.ToBase64String(hash);

            try
            {
                var uri = _feedbackUri + "?sig=" + signature + "&v=" + version;
                var request = (HttpWebRequest) WebRequest.Create(uri);
                AddProxyIfRequired(request, uri);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.GetRequestStream().Write(buffer, 0, buffer.Length);
                using (request.GetResponse())
                {
                }
            }
            catch (Exception err)
            {
                AnalyzeException(err);
                throw new UploadFailedException(
                    "The actual upload failed (probably network error). We'll try again later..", err);
            }
        }

        /// <summary>
        ///     Try to upload a report directly
        /// </summary>
        /// <param name="report">Report to upload</param>
        /// <exception cref="WebException">No internet connection is available; Destination server did not accept the report.</exception>
        public void TryUploadReportNow(ErrorReportDTO report)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new UploadFailedException("Not connected, try again later.");

            var buffer = CompressErrorReport(report);
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString(2);
            var hashAlgo = new HMACSHA256(Encoding.UTF8.GetBytes(SharedSecret));
            var hash = hashAlgo.ComputeHash(buffer);
            var signature = Convert.ToBase64String(hash);

            try
            {
                var uri = _reportUri + "?sig=" + signature + "&v=" + version;
                var request = (HttpWebRequest) WebRequest.Create(uri);
                AddProxyIfRequired(request, uri);

                request.Method = "POST";
                request.ContentType = "application/octet-stream";
                var evt = request.BeginGetRequestStream(null, null);
                var stream = request.EndGetRequestStream(evt);
                stream.Write(buffer, 0, buffer.Length);
                var responseRes = request.BeginGetResponse(null, null);
                var response = (HttpWebResponse) request.EndGetResponse(responseRes);
                using (response)
                {
                    //Console.WriteLine(response);
                }
            }
            catch (Exception err)
            {
                AnalyzeException(err);
                throw new UploadFailedException(
                    "The actual upload failed (probably network error). We'll try again later..", err);
            }
        }

        /// <summary>
        ///     Dispose pattern
        /// </summary>
        /// <param name="isDisposing">Invoked from the dispose method.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (_feedbackQueue != null)
            {
                _feedbackQueue.Dispose();
                _feedbackQueue = null;
            }
            if (_reportQueue != null)
            {
                _reportQueue.Dispose();
                _reportQueue = null;
            }
        }

        /// <summary>
        ///     Compress an ErrorReport as JSON string
        /// </summary>
        /// <param name="errorReport">ErrorReport</param>
        /// <returns>Compressed JSON representation of the ErrorReport.</returns>
        internal byte[] CompressErrorReport(ErrorReportDTO errorReport)
        {
            var reportJson = JsonConvert.SerializeObject(errorReport, Formatting.None,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None,
                    ContractResolver =
                        new IncludeNonPublicMembersContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
            var buffer = Encoding.UTF8.GetBytes(reportJson);

            //collected by GZipStream
            var outMs = new MemoryStream();
            using (var zipStream = new GZipStream(outMs, CompressionMode.Compress))
            {
                zipStream.Write(buffer, 0, buffer.Length);

                //MUST close the stream, flush doesn't help and without close
                // the memory stream won't get its bytes
                zipStream.Close();

                var result = outMs.ToArray();
                return result;
            }
        }

        /// <summary>
        ///     Deflate a compressed error report in JSON format
        /// </summary>
        /// <param name="errorReport">Compressed JSON errorReport</param>
        /// <returns>JSON string decompressed</returns>
        internal string DeflateErrorReport(byte[] errorReport)
        {
            // collected by GZipStream
            var zipStream = new MemoryStream(errorReport);

            //disposed by GZipStream
            var deflateStream = new MemoryStream();
            using (var decompressor = new GZipStream(zipStream, CompressionMode.Decompress))
            {
                decompressor.CopyTo(deflateStream);
                deflateStream.Position = 0;
                var buffer = new byte[deflateStream.Length];
                deflateStream.Read(buffer, 0, (int) deflateStream.Length);
                var strBuffer = Encoding.UTF8.GetString(buffer);
                return strBuffer;
            }
        }

        private static void AddProxyIfRequired(HttpWebRequest request, string uri)
        {
            var proxy = request.Proxy;
            if (proxy != null && !proxy.IsBypassed(new Uri(uri)))
            {
                var proxyuri = proxy.GetProxy(request.RequestUri).ToString();
                request.UseDefaultCredentials = true;
                request.Proxy = new WebProxy(proxyuri, false)
                {
                    Credentials = CredentialCache.DefaultCredentials
                };
            }
        }

        private static void AnalyzeException(Exception err)
        {
            var exception = err as WebException;
            if (exception?.Response == null)
                return;

            var title = "Failed to execute";
            var description = "Did not get a response. Check your network connection.";

            var resp = (HttpWebResponse) exception.Response;
            var stream = exception.Response.GetResponseStream();
            if (stream != null)
            {
                var reader = new StreamReader(stream);
                description = reader.ReadToEnd();
                title = resp.StatusDescription;
            }
            try
            {
                exception.Response.Close();
            }
            catch
            {
                // ignored
            }

            switch (resp.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedAccessException(title + "\r\n" + description, err);
                case HttpStatusCode.NotFound:
                    //legacy handling of 404. Mal-configured web servers will report 404,
                    //so remove the usage to avoid ambiguity
                    if (title.IndexOf("key", StringComparison.OrdinalIgnoreCase) != -1)
                        throw new InvalidApplicationKeyException(title + "\r\n" + description, err);
                    throw new UploadFailedException("Server returned an error.\r\n" + description, err);
                default:
                    if (resp.StatusCode == HttpStatusCode.BadRequest && title.Contains("APP_KEY"))
                        throw new InvalidApplicationKeyException(title + "\r\n" + description, err);

                    throw new UploadFailedException("Server returned an error.\r\n" + description, err);
            }
        }

        private void OnUploadFailed(object sender, UploadReportFailedEventArgs args)
        {
            UploadFailed?.Invoke(this, args);
        }
    }
}