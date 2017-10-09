using System;
using System.Collections.Specialized;
using codeRR.Client.ContextProviders.Helpers;
using codeRR.Client.Contracts;
using codeRR.Client.Reporters;

namespace codeRR.Client.ContextProviders
{
    /// <summary>
    ///     Loads information about all applications which start during system startup. Will be added to a collection called
    ///     <c>StartupCommands</c>.
    /// </summary>
    public class StartupCommandsProvider : IContextInfoProvider
    {
        /// <summary>
        ///     Gets "StartupCommands"
        /// </summary>
        public string Name => "StartupCommands";


        /// <summary>
        ///     Collect information
        /// </summary>
        /// <param name="context">Context information provided by the class which reported the error.</param>
        /// <returns>
        ///     Collection
        /// </returns>
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            var contextCollection = new NameValueCollection();

            try
            {
                var collector = new ManagementCollector(contextCollection);
                collector.Collect("Win32_StartupCommand");
            }
            catch (Exception exception)
            {
                contextCollection.Add("CollectionException", exception.ToString());
            }

            return new ContextCollectionDTO("StartupCommands", contextCollection);
        }
    }
}