using System.Web;
using OneTrueError.Client.ContextProviders;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.AspNet.ContextProviders
{
    /// <summary>
    /// Adds a HTTP request query string collection.
    /// </summary>
    /// <remarks>The name of the collection is <c>HttpQueryString</c></remarks>
    public class QueryStringProvider : IContextInfoProvider
    {
        /// <summary>
        /// Gets "HttpQueryString"
        /// </summary>
        public string Name { get { return "HttpQueryString"; } }


        /// <summary>
        /// Collect information
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Collection</returns>
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            if (HttpContext.Current == null || HttpContext.Current.Request.QueryString.Count == 0)
                return null;

            return new ContextCollectionDTO("HttpQueryString", HttpContext.Current.Request.QueryString);
        }
    }
}