using System.Web;
using OneTrueError.Client.ContextProviders;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.AspNet.ContextProviders
{
    /// <summary>
    ///     Generates a collection named "HttpContextItems" consisting of <c>HttpContext.Current.Items</c>.
    /// </summary>
    public class HttpContextItemsProvider : IContextInfoProvider
    {
        /// <summary>Collect information</summary>
        /// <param name="context">Context information provided by the class which reported the error.</param>
        /// <returns>Collection. Items with multiple values are joined using <c>";;"</c></returns>
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            if (HttpContext.Current.Items.Count == 0)
                return null;

            return HttpContext.Current.Items.ToContextCollection("HttpContextItems");
        }

        /// <summary>
        ///     HttpContextItems
        /// </summary>
        public string Name { get; private set; }
    }
}