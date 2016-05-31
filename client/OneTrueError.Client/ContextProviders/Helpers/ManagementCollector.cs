using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Management;

namespace OneTrueError.Client.ContextProviders.Helpers
{
    /// <summary>
    ///     Collector used to fetch information from the windows management API
    /// </summary>
    internal class ManagementCollector
    {
        private readonly NameValueCollection _collection;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ManagementCollector" /> class.
        /// </summary>
        /// <param name="collection">Collection to fill with context information.</param>
        /// <exception cref="System.ArgumentNullException">collection</exception>
        public ManagementCollector(NameValueCollection collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            _collection = collection;
        }

        /// <summary>
        ///     Filter out the specified names (if any).
        /// </summary>
        public string[] Filter { get; set; }

        /// <summary>
        ///     Collects the specified name.
        /// </summary>
        /// <param name="name">Management object table.</param>
        public void Collect(string name)
        {
            var filter = Filter;
            var searcher = new ManagementObjectSearcher("SELECT * FROM " + name);
            foreach (var item in searcher.Get())
            {
                foreach (var sp in item.Properties)
                {
                    if (sp.Value == null)
                        continue;
                    if (filter != null && filter.Any(x => x.Equals(sp.Name, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    if (sp.Value is string)
                    {
                        var value = Convert.ToString(sp.Value);
                        if (string.IsNullOrEmpty(value))
                            continue;

                        //naive attempt to detect dates.
                        if (value.Length == "20100131022308.000000-360".Length &&
                            (value[value.Length - 4] == '-' || value[value.Length - 4] == '+') && value[14] == '.')
                        {
                            try
                            {
                                value = ManagementDateTimeConverter.ToDateTime(value).ToUniversalTime().ToString();
                            }
                            catch (Exception exception)
                            {
                                _collection.Add(sp.Name + ".error", exception.ToString());
                            }
                        }

                        _collection.Add(sp.Name, value);
                    }
                    else if (sp.Value is IEnumerable || sp.IsArray)
                    {
                        _collection.Add(sp.Name, string.Join(";;", (IEnumerable) sp.Value));
                    }
                    else
                    {
                        //System.Management.ManagementDateTimeConverter.ToDateTime("20100131022308.000000-360");

                        var value = Convert.ToString(sp.Value);

                        if (string.IsNullOrEmpty(value))
                            continue;

                        _collection.Add(sp.Name, value);
                    }
                }
            }
        }
    }
}