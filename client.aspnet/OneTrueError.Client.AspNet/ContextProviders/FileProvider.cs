using System.Collections.Generic;
using System.Web;
using OneTrueError.Client.ContextProviders;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.AspNet.ContextProviders
{
    /// <summary>
    ///     Collects information about all files which was uploaded in the HTTP request. The collection is named <c>HttpRequestFiles</c>.
    /// </summary>
    /// <remarks>Collects name, content type and file name.</remarks>
    public class FileProvider : IContextInfoProvider
    {
        /// <summary>
        /// Gets "HttpRequestFiles"
        /// </summary>
        public string Name { get { return "HttpRequestFiles"; } }

        /// <summary>
        ///     Collect information
        /// </summary>
        /// <param name="context">Context information provided by the class which reported the error.</param>
        /// <returns>
        ///     Collection. Items with multiple values are joined using <c>";;"</c>
        /// </returns>
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            if (HttpContext.Current == null || HttpContext.Current.Request.Files.Count == 0)
                return null;

            var files = new Dictionary<string, string>();
            foreach (string key in HttpContext.Current.Request.Files)
            {
                var file = HttpContext.Current.Request.Files[key];
                files[file.FileName] = string.Format(file.ContentType + ";length=" + file.ContentLength);
            }
            return new ContextCollectionDTO("HttpRequestFiles", files);
        }
    }
}