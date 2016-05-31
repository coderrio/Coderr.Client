using System;
using System.Collections.Specialized;
using OneTrueError.Client.ContextProviders.Helpers;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.ContextProviders
{
    /// <summary>
    ///     Collects information about all disks in the computer. Will be added into a collection called <c>DiskDrives</c>.
    /// </summary>
    public class DiskDriveProvider : IContextInfoProvider
    {
        /// <summary>
        ///     Gets "DiskDrives"
        /// </summary>
        public string Name
        {
            get { return "DiskDrives"; }
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
                collector.Collect("Win32_DiskDrive");
            }
            catch (Exception exception)
            {
                contextCollection.Add("CollectionException", exception.ToString());
            }

            return new ContextCollectionDTO("DiskDrives", contextCollection);
        }
    }
}