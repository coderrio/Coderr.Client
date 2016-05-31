using System.Web;
using OneTrueError.Client.ContextProviders;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.AspNet.Mvc5.ContextProviders
{
    /// <summary>
    ///     Adds a HTTP request form collection.
    /// </summary>
    /// <remarks>The name of the collection is "HttpForm"</remarks>
    public class FormProvider : IContextInfoProvider
    {
        /// <summary>
        ///     Collect information
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Collection</returns>
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            return new ContextCollectionDTO("HttpForm", HttpContext.Current.Request.Form);
        }

        /// <summary>
        ///     "HttpForm"
        /// </summary>
        public string Name
        {
            get { return "HttpForm"; }
        }
    }
}