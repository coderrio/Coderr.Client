using System;
using System.Collections.Specialized;
using OneTrueError.Client.ContextProviders.Helpers;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.ContextProviders
{
    /// <summary>
    ///     Loads information about all system devices. Will be added to a collection called <c>SystemDevices</c>.
    /// </summary>
    public class SystemDevicesProvider : IContextInfoProvider
    {
        /// <summary>
        ///     Gets "SystemDevices"
        /// </summary>
        public string Name => "SystemDevices";


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
                collector.Collect("Win32_SystemDevices");
            }
            catch (Exception exception)
            {
                contextCollection.Add("CollectionException", exception.ToString());
            }

            return new ContextCollectionDTO("SystemDevices", contextCollection);
        }
    }
}