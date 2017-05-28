using OneTrueError.Client.ContextProviders;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Converters;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.AspNet.Mvc5.ContextProviders
{
    /// <summary>
    ///     Name: "HttpContext.Application"
    /// </summary>
    public class HttpApplicationItemsProvider : IContextInfoProvider
    {
        /// <inheritdoc />
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            var aspNetContext = context as AspNetContext;
            if (aspNetContext == null || aspNetContext.HttpContext == null)
                return null;

            if (aspNetContext.HttpContext.Application == null || aspNetContext.HttpContext.Application.Count == 0)
                return null;

            var converter = new ObjectToContextCollectionConverter();
            return converter.Convert(Name, aspNetContext.HttpContext.Application);
        }

        /// <inheritdoc />
        public string Name
        {
            get { return "HttpContext.Application"; }
        }
    }
}