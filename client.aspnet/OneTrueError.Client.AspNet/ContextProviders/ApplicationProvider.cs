using System;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;
using OneTrueError.Client.ContextProviders;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.AspNet.ContextProviders
{
    /// <summary>
    ///     Collects all items from the application collection in <c>HttpContext</c>.
    /// </summary>
    public class ApplicationProvider : IContextInfoProvider
    {
        /// <summary>Collect information</summary>
        /// <param name="context">Context information provided by the class which reported the error.</param>
        /// <returns>Collection named <c>Application</c>. Items with multiple values are joined using <c>";;"</c></returns>
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            var items = new Dictionary<string, string>();
            foreach (var key in HttpContext.Current.Application.AllKeys)
            {
                var value = HttpContext.Current.Application[key];
                if (value == null)
                {
                    items[key] = "null";
                    continue;
                }

                if (value.GetType().IsPrimitive || value.GetType() == typeof(string))
                    items[key] = value.ToString();

                try
                {
                    items[key] = JsonConvert.SerializeObject(value);
                }
                catch (Exception ex)
                {
                    items[key + ".Error"] = ex.ToString();
                }
            }

            if (items.Count == 0)
                return null;

            return new ContextCollectionDTO("Application", items);
        }

        /// <summary>
        ///     "Application"
        /// </summary>
        public string Name { get; private set; }
    }
}