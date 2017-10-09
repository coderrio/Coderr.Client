using System;
using System.Collections.Generic;
using codeRR.Client.Contracts;
using codeRR.Client.Reporters;

namespace codeRR.Client.ContextProviders
{
    /// <summary>
    ///     Collection information about all assemblies which has been loaded. Will be put into a collection named
    ///     <c>Assemblies</c>.
    /// </summary>
    [DefaultProvider]
    public class AssemblyProvider : IContextInfoProvider
    {
        /// <summary>
        ///     Gets "Assemblies"
        /// </summary>
        public string Name => "Assemblies";


        /// <summary>
        ///     Collect information
        /// </summary>
        /// <param name="context">Context information provided by the class which reported the error.</param>
        /// <returns>
        ///     Collection. Items with multiple values are joined using <c>";;"</c>
        /// </returns>
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            var items = new Dictionary<string, string>();
            try
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.IsDynamic)
                        continue;

                    items[assembly.GetName().Name] = assembly.GetName().Version.ToString();
                }
            }
            catch (Exception exception)
            {
                items.Add("CollectionException", exception.ToString());
            }
            return new ContextCollectionDTO("Assemblies", items);
        }
    }
}