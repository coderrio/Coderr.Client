using System;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Converters;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.ContextProviders
{
    /// <summary>
    ///     Goes through the exception and maps all custom properties. Will be added into a collection called
    ///     <c>ExceptionProperties</c>.
    /// </summary>
    [DefaultProvider]
    public class ExceptionPropertiesProvider : IContextInfoProvider
    {
        /// <summary>
        ///     Returns "ExceptionProperties"
        /// </summary>
        public string Name
        {
            get { return "ExceptionProperties"; }
        }

        /// <summary>
        ///     Collect information
        /// </summary>
        /// <param name="context">Context information provided by the class which reported the error.</param>
        /// <returns>
        ///     Collection. Items with multiple values are joined using <c>";;"</c>
        /// </returns>
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            try
            {
                var converter = new ObjectToContextCollectionConverter
                {
                    MaxPropertyCount = OneTrue.Configuration.MaxNumberOfPropertiesPerCollection
                };
                var collection = converter.Convert(context.Exception);
                collection.Name = "ExceptionProperties";
                return collection;
            }
            catch (Exception ex)
            {
                var context2 = new ContextCollectionDTO("ExceptionProperties");
                context2.Properties.Add("Failed", ex.ToString());
                return context2;
            }
        }
    }
}