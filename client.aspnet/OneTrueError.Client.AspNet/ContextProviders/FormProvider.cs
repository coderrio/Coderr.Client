using System.Web;
using OneTrueError.Client.ContextProviders;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.AspNet.ContextProviders
{
    /// <summary>
    /// Adds a HTTP request form collection.
    /// </summary>
    /// <remarks>The name of the collection is <c>HttpForm</c></remarks>
    public class FormProvider : IContextInfoProvider
    {
        /// <summary>
        /// Gets "HttpForm"
        /// </summary>
        public string Name { get { return "HttpForm"; } }


        /// <summary>
        /// Collect information
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Collection</returns>
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            if (HttpContext.Current == null || HttpContext.Current.Request.Form.Count == 0)
                return null;

            return new ContextCollectionDTO("HttpForm", HttpContext.Current.Request.Form);
        }
    }
}