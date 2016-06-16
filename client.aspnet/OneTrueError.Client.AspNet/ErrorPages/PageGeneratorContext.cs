using System.IO;
using System.Web;

namespace OneTrueError.Client.AspNet.ErrorPages
{
    /// <summary>
    ///     Context for the page generator
    /// </summary>
    public class PageGeneratorContext
    {
        /// <summary>
        ///     Context from the context collection process.
        /// </summary>
        public HttpErrorReporterContext ReporterContext { get; set; }


        /// <summary>
        ///     Report identifier (used to be able to associate feedback to the report)
        /// </summary>
        public string ReportId { get; set; }

        /// <summary>
        ///     HTTP response
        /// </summary>
        public HttpResponseBase Response { get; set; }

        /// <summary>
        ///     HTTP request
        /// </summary>
        public HttpRequestBase Request { get; set; }

        /// <summary>
        ///     Send response to browser.
        /// </summary>
        /// <param name="contentType">content type</param>
        /// <param name="body">contents</param>
        public void SendResponse(string contentType, string body)
        {
            Response.ContentType = contentType;
            var sw = new StreamWriter(Response.OutputStream);
            sw.Write(body);
            sw.Flush();
        }
    }
}