using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using codeRR.Client.Contracts;
using codeRR.Client.Reporters;

namespace codeRR.Client.ContextProviders
{
    /// <summary>
    ///     Registrar used to configure which kind of context information to provide for each unhandled exception
    /// </summary>
    /// <remarks>
    ///     The built in providers can be found in the <see cref="ContextProviders" /> namespace.
    /// </remarks>
    public class ContextProvidersRegistrar
    {
        private readonly List<IContextInfoProvider> _providers = new List<IContextInfoProvider>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContextProvidersRegistrar" /> class.
        /// </summary>
        public ContextProvidersRegistrar()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                if (typeof(IContextInfoProvider).IsAssignableFrom(type) && type.IsClass)
                {
                    var count = type.GetCustomAttributes(typeof(DefaultProviderAttribute), false).Length;
                    if (count == 1)
                        Add((IContextInfoProvider) Activator.CreateInstance(type));
                }
        }

        /// <summary>
        ///     Add a new provider
        /// </summary>
        /// <param name="provider">Provider to add</param>
        /// <exception cref="System.ArgumentNullException">provider</exception>
        public void Add(IContextInfoProvider provider)
        {
            if (provider == null) throw new ArgumentNullException("provider");
            _providers.Add(provider);
        }

        /// <summary>
        ///     Remove all registered providers.
        /// </summary>
        public void Clear()
        {
            _providers.Clear();
        }

        /// <summary>
        ///     Collect context information from all context providers.
        /// </summary>
        /// <param name="context">
        ///     CreateReport context (specialized for the different report adapters, for instance the ASP.NET
        ///     adapter contains the current <c>HttpContext</c>)
        /// </param>
        /// <returns>
        ///     Collected information
        /// </returns>
        /// <exception cref="System.ArgumentNullException">context</exception>
        public IList<ContextCollectionDTO> Collect(IErrorReporterContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var items = new List<ContextCollectionDTO>();

            if (context is IErrorReporterContext2 ctx2)
            {
                foreach (var collectionDto in ctx2.ContextCollections)
                {
                    items.Add(collectionDto);
                }
            }

            foreach (var provider in _providers)
                try
                {
                    var item = provider.Collect(context);
                    if (item == null)
                        continue;

                    items.Add(item);
                }
                catch (Exception exception)
                {
                    var item = new ContextCollectionDTO(provider.Name,
                        new Dictionary<string, string> {{"Error", exception.ToString()}});
                    items.Add(item);
                }


            return items;
        }

        /// <summary>
        ///     Returns the name of all providers which have been added.
        /// </summary>
        /// <returns>String array (or an empty array)</returns>
        public string[] GetAddedProviderNames()
        {
            return _providers.Select(x => x.Name).ToArray();
        }


        /// <summary>
        ///     Remove the provider with the specified name
        /// </summary>
        /// <param name="name">The name (can be found using <see cref="GetAddedProviderNames" />).</param>
        public void Remove(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            _providers.RemoveAll(x => x.Name == name);
        }
    }
}