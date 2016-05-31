using System.Collections.Generic;
using System.Threading;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.ContextProviders
{
    /// <summary>
    ///     Adds the logged in user (using <c>Thread.CurrentPrincipal</c>).
    /// </summary>
    public class CurrentUserProvider : IContextInfoProvider
    {
        /// <summary>
        ///     Returns <c>"CurrentUser"</c>
        /// </summary>
        public string Name
        {
            get { return "CurrentUser"; }
        }

        /// <summary>
        ///     Collects the identity name.
        /// </summary>
        /// <param name="context">Context collection</param>
        /// <returns>Generated collection</returns>
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            var contextInfo = new Dictionary<string, string>
            {
                {"Name", Thread.CurrentPrincipal.Identity.Name}
            };
            return new ContextCollectionDTO(Name, contextInfo);
        }
    }
}