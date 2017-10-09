using System.Runtime.CompilerServices;

namespace codeRR.Client.Uploaders
{
    /// <summary>
    ///     Submitters are the classes which takes error reports and delivers them to a specific destination. The destination
    ///     itself
    ///     can be anything from an local log file to a web service.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The codeRR reporter is defined as <see cref="UploadToCoderr" />.
    ///     </para>
    ///     <para>
    ///         This library will enqueue every error report for each submitter and also persist the queue. It that way the
    ///         library can continue to upload error
    ///         reports to every submitter even if the application crashes.
    ///     </para>
    /// </remarks>
    [CompilerGenerated]
    public class NamespaceDoc
    {
    }
}