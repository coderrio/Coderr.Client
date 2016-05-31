using System;
using System.Collections.Specialized;
using OneTrueError.Client.ContextProviders.Helpers;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.ContextProviders
{
    /// <summary>
    ///     Collects information about the CPU. Will be added to a collection called <c>Processor</c>.
    /// </summary>
    public class ProcessorProvider : IContextInfoProvider
    {
        /// <summary>
        ///     Gets "Processor"
        /// </summary>
        public string Name
        {
            get { return "Processor"; }
        }

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
                collector.Collect("Win32_Processor");
            }
            catch (Exception exception)
            {
                contextCollection.Add("CollectionException", exception.ToString());
            }

            return new ContextCollectionDTO("Processor", contextCollection);
        }
    }
}