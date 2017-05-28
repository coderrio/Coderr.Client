using OneTrueError.Client.ContextProviders;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Converters;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.AspNet.Mvc5.ContextProviders
{
    /// <summary>
    ///     Name: "ViewBag"
    /// </summary>
    public class ViewBagProvider : IContextInfoProvider
    {
        /// <inheritdoc />
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            var aspNetContext = context as AspNetMvcContext;
            if (aspNetContext == null || aspNetContext.ViewBag == null)
                return null;

            var converter = new ObjectToContextCollectionConverter();
            return converter.Convert(Name, aspNetContext.ViewBag);
        }

        /// <inheritdoc />
        public string Name
        {
            get { return "ViewBag"; }
        }
    }
}