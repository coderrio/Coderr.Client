using System.Collections.Specialized;
using System.Web;
using Newtonsoft.Json;
using OneTrueError.Client.ContextProviders;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.AspNet.Mvc5.ContextProviders
{
    /// <summary>
    ///     Adds a HTTP request query string collection.
    /// </summary>
    /// <remarks>
    ///     The name of the collection is "HttpSession".
    ///     <para>Session objects are serialized as JSON, strings are added as-is.</para>
    /// </remarks>
    public class SessionProvider : IContextInfoProvider
    {
        /// <summary>
        ///     Collect information
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Collection</returns>
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            if (HttpContext.Current.Session == null)
                return new ContextCollectionDTO("HttpSession", new NameValueCollection());

            var items = new NameValueCollection();
            foreach (string key in HttpContext.Current.Session)
            {
                var item = HttpContext.Current.Session[key];
                if (item is string)
                {
                    items.Add(key, (string) item);
                }
                else
                {
                    var json = JsonConvert.SerializeObject(item);
                    items.Add(key, json);
                }
            }

            return new ContextCollectionDTO("HttpSession", items);
        }

        /// <summary>
        ///     "HttpSession"
        /// </summary>
        public string Name
        {
            get { return "HttpSession"; }
        }
    }
}