using Coderr.Client.Contracts;
using Coderr.Client.Reporters;

namespace Coderr.Client.ContextCollections
{
    /// <summary>
    ///     Collects information from a specific part of the system
    /// </summary>
    public interface IContextCollectionProvider
    {
        /// <summary>
        ///     Name of the collection that this provider adds.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Collect information
        /// </summary>
        /// <param name="context">Context information provided by the class which reported the error.</param>
        /// <returns>Collection; <c>null</c> if the source information is missing and a collection cannot be created.</returns>
        /// <remarks>
        ///     <para>
        ///         Items with arrays should seperate the items with double semicolon <c>;;</c>, that way the website can present
        ///         them well formatted.
        ///     </para>
        /// </remarks>
        ContextCollectionDTO Collect(IErrorReporterContext context);
    }
}