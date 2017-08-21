using System;
using System.Collections.Specialized;
using OneTrueError.Client.ContextProviders.Helpers;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.ContextProviders
{
    /// <summary>
    ///     Collects information about the operating system like version and service pack info. Will be added to a collection
    ///     called <c>OperatingSystem</c>.
    /// </summary>
    [DefaultProvider]
    public class OperatingSystemProvider : IContextInfoProvider
    {
        /// <summary>
        ///     Fields that will not be included.
        /// </summary>
        /// <remarks>
        ///     <para>Default filter is <code> "CSName", "RegisteredUser", "SerialNumber"</code> </para>
        /// </remarks>
        public static string[] FilteredFields = {"CSName", "RegisteredUser", "SerialNumber"};

        /// <summary>
        ///     Gets "OperatingSystem"
        /// </summary>
        public string Name => "OperatingSystem";

        /// <summary>
        ///     Collect information
        /// </summary>
        /// <param name="context">Context information provided by the class which reported the error.</param>
        /// <returns>
        ///     Collection. Items with multiple values are joined using <c>";;"</c>
        /// </returns>
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            var contextCollection = new NameValueCollection();

            try
            {
                var collector = new ManagementCollector(contextCollection);
                collector.Filter = FilteredFields;
                collector.Collect("Win32_OperatingSystem");
            }
            catch (Exception exception)
            {
                contextCollection.Add("CollectionException", exception.ToString());
            }

            return new ContextCollectionDTO("OperatingSystem", contextCollection);
        }
    }
}