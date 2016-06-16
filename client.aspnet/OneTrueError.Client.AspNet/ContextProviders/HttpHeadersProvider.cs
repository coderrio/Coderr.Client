using System.Collections.Specialized;
using System.Web;
using OneTrueError.Client.ContextProviders;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.AspNet.ContextProviders
{
    /// <summary>
    ///     assembles all HTTP headers from the request.
    /// </summary>
    /// <remarks>They will be added to a collection called <c>HttpHeaders</c>.</remarks>
    public class HttpHeadersProvider : IContextInfoProvider
    {
        /// <summary>
        /// Gets "HttpHeaders"
        /// </summary>
        public string Name { get { return "HttpHeaders"; } }


        /// <summary>
        ///     Collect information
        /// </summary>
        /// <param name="context">Context information provided by the class which reported the error.</param>
        /// <returns>
        ///     Collection. Items with multiple values are joined using <c>";;"</c>
        /// </returns>
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            if (HttpContext.Current == null)
                return null;

            var myHeaders = new NameValueCollection(HttpContext.Current.Request.Headers);
            myHeaders["Url"] = HttpContext.Current.Request.Url.ToString();
            return new ContextCollectionDTO("HttpHeaders", myHeaders);
        }
    }
}