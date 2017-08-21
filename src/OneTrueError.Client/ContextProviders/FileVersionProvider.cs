using System;
using System.Collections.Generic;
using System.Diagnostics;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.ContextProviders
{
    /// <summary>
    ///     File versions for all loaded assemblies (can be different than the assembly version).
    /// </summary>
    /// <remarks>
    ///     <para>Collection name is "FileVersions"</para>
    /// </remarks>
    [DefaultProvider]
    public class FileVersionProvider : IContextInfoProvider
    {
        /// <summary>
        ///     "FileVersions"
        /// </summary>
        public const string NAME = "FileVersions";

        /// <summary>
        ///     Name of the collection that this provider adds.
        /// </summary>
        public string Name => NAME;

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

                    var info = FileVersionInfo.GetVersionInfo(assembly.Location);
                    items[assembly.GetName().Name] = info.FileVersion;
                }
            }
            catch (Exception exception)
            {
                items.Add("CollectionException", exception.ToString());
            }
            return new ContextCollectionDTO(NAME, items);
        }
    }
}