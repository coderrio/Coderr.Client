using System.Web;
using OneTrueError.Client.ContextProviders;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.AspNet.Mvc5.ContextProviders
{
    /// <summary>
    ///     Adds a HTTP request query string collection.
    /// </summary>
    /// <remarks>The name of the collection is "HttpQueryString"</remarks>
    public class QueryStringProvider : IContextInfoProvider
    {
        /// <summary>
        ///     Collect information
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Collection</returns>
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            return new ContextCollectionDTO("HttpQueryString", HttpContext.Current.Request.QueryString);
        }

        /// <summary>
        ///     "HttpQueryString"
        /// </summary>
        public string Name
        {
            get { return "HttpQueryString"; }
        }
    }
}