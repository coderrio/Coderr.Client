using System.Web;
using OneTrueError.Client.ContextProviders;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Converters;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.AspNet.ContextProviders
{
    /// <summary>
    ///     Adds a HTTP request query string collection (<c>"HttpSession"</c>)
    /// </summary>
    /// <remarks>
    ///     <para>The name of the collection is <c>HttpSession</c>.</para>
    ///     <para>Session objects are serialized as JSON, strings are added as-is.</para>
    /// </remarks>
    public class SessionProvider : IContextInfoProvider
    {
        /// <summary>
        /// Gets "HttpSession"
        /// </summary>
        public string Name { get { return "HttpSession"; } }

        /// <summary>
        ///     Collect information
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Collection</returns>
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            if (HttpContext.Current.Session == null || HttpContext.Current.Session.Count == 0)
                return null;
                
            var converter = new ObjectToContextCollectionConverter();
            return converter.Convert("HttpSession", HttpContext.Current.Session);
        }
    }
}