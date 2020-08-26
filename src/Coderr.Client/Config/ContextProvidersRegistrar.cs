using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coderr.Client.ContextCollections;
using Coderr.Client.ContextCollections.Providers;
using Coderr.Client.Contracts;
using Coderr.Client.Reporters;

namespace Coderr.Client.Config
{
    /// <summary>
    ///     Registrar used to configure which kind of context information to provide for each unhandled exception
    /// </summary>
    /// <remarks>
    ///     The built in providers can be found in the <see cref="ContextCollections" /> namespace.
    /// </remarks>
    public class ContextProvidersRegistrar
    {
        private readonly List<IContextCollectionProvider> _providers = new List<IContextCollectionProvider>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContextProvidersRegistrar" /> class.
        /// </summary>
        public ContextProvidersRegistrar()
        {
            var arm = typeof(ContextProvidersRegistrar).GetTypeInfo().Assembly;
            var providerType = typeof(IContextCollectionProvider).GetTypeInfo();
            foreach (var type in arm.ExportedTypes)
            {
                var typeInfo = type.GetTypeInfo();
                if (providerType.IsAssignableFrom(typeInfo) && typeInfo.IsClass)
                {
                    var isDefault = typeInfo.GetCustomAttributes(typeof(DefaultProviderAttribute), false).Any();
                    if (isDefault)
                        Add((IContextCollectionProvider) Activator.CreateInstance(type));
                }
            }
        }

        /// <summary>
        ///     Add a new provider.
        /// </summary>
        /// <param name="provider">Provider to add</param>
        /// <exception cref="System.ArgumentNullException">provider</exception>
        public void Add(IContextCollectionProvider provider)
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
        /// <remarks>
        /// <para>
        /// Collections are added to the <c>context.ContextCollections</c> property.
        /// </para>
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">context</exception>
        public void Collect(IErrorReporterContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            foreach (var provider in _providers)
            {
                try
                {
                    var item = provider.Collect(context);
                    if (item == null)
                        continue;

                    context.ContextCollections.Add(item);
                }
                catch (Exception exception)
                {
                    var item = new ContextCollectionDTO(provider.Name,
                        new Dictionary<string, string> {{"Error", exception.ToString()}});
                    context.ContextCollections.Add(item);
                }
            }
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