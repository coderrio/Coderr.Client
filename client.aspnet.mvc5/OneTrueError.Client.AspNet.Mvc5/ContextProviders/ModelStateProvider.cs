using System.Collections.Generic;
using System.Web.Mvc;
using OneTrueError.Client.ContextProviders;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.AspNet.Mvc5.ContextProviders
{
    /// <summary>
    ///     Name: "ModelState"
    /// </summary>
    public class ModelStateProvider : IContextInfoProvider
    {
        /// <inheritdoc />
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            var aspNetContext = context as AspNetMvcContext;
            if (aspNetContext == null || aspNetContext.ModelState == null || aspNetContext.ModelState.Count == 0)
                return null;

            var dict = new Dictionary<string, string>();
            foreach (var kvp in (IDictionary<string, ModelState>) aspNetContext.ModelState)
            {
                if (kvp.Value == null)
                    continue;

                var state = kvp.Value;

                if (state.Value != null)
                {
                    if (state.Value.RawValue != null)
                        dict[kvp.Key + ".RawValue"] = state.Value.RawValue.ToString();
                    if (state.Value.AttemptedValue != null)
                        dict[kvp.Key + ".AttemptedValue"] = state.Value.AttemptedValue;
                }

                foreach (var error in state.Errors)
                    dict[kvp.Key + ".Error"] = error.ErrorMessage;
            }
            return new ContextCollectionDTO(Name, dict);
        }

        /// <inheritdoc />
        public string Name
        {
            get { return "ModelState"; }
        }
    }
}