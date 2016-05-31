using System.Collections.Generic;
using System.Web;
using OneTrueError.Client.ContextProviders;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.AspNet.Mvc5.ContextProviders
{
    /// <summary>
    ///     Attaches information about files uploaded in the request
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Provides filename (property name), content type and file size (combined as property value)
    ///     </para>
    /// </remarks>
    public class FileProvider : IContextInfoProvider
    {
        /// <summary>Collect information</summary>
        /// <param name="context">Context information provided by the class which reported the error.</param>
        /// <returns>Collection. Items with multiple values are joined using <c>";;"</c></returns>
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            var files = new Dictionary<string, string>();
            foreach (string key in HttpContext.Current.Request.Files)
            {
                var file = HttpContext.Current.Request.Files[key];
                files[file.FileName] = string.Format(file.ContentType + ";length=" + file.ContentLength);
            }
            return new ContextCollectionDTO("HttpRequestFiles", files);
        }

        /// <summary>
        ///     "HttpRequestFiles"
        /// </summary>
        public string Name
        {
            get { return "HttpRequestFiles"; }
        }
    }
}