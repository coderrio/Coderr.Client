using codeRR.Client.Contracts;
using codeRR.Client.Reporters;

namespace codeRR.Client.ContextProviders
{
    /// <summary>
    ///     Collects information from a specific part of the system
    /// </summary>
    public interface IContextInfoProvider
    {
        /// <summary>
        ///     Name of the collection that this provider adds.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Collect information
        /// </summary>
        /// <param name="context">Context information provided by the class which reported the error.</param>
        /// <returns>Collection. Items with multiple values are joined using <c>";;"</c></returns>
        ContextCollectionDTO Collect(IErrorReporterContext context);
    }
}