using System.Collections.Generic;
using OneTrueError.Client.ContextProviders;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.AspNet.Mvc5.ContextProviders
{
    /// <summary>
    ///     "RouteData"
    /// </summary>
    internal class RouteDataProvider : IContextInfoProvider
    {
        /// <inheritdoc />
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            var aspNetContext = context as AspNetMvcContext;
            if (aspNetContext == null || aspNetContext.RouteData == null || aspNetContext.RouteData.Values.Count == 0)
                return null;

            var dict = new Dictionary<string, string>();
            foreach (var token in aspNetContext.RouteData.DataTokens)
                dict.Add("DataToken[\"" + token.Key + "\"]", token.Value.ToString());
            foreach (var token in aspNetContext.RouteData.Values)
                dict.Add("Values[\"" + token.Key + "\"]", token.Value.ToString());

            return new ContextCollectionDTO(Name, dict);
        }

        /// <inheritdoc />
        public string Name
        {
            get { return "RouteData"; }
        }
    }
}