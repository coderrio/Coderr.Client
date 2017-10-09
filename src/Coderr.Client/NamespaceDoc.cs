using System.Runtime.CompilerServices;
using codeRR.Client.Config;

namespace codeRR.Client
{
    /// <summary>
    ///     <para>
    ///         Welcome to the reporting library for codeRR. Read below the class list for more information about the
    ///         library.
    ///     </para>
    ///     <para>
    ///         This library is configured by using the <see cref="Err" /> class. A minimal setup contains a single line:
    ///     </para>
    ///     <code>
    /// Err.Credentials("appKey", "sharedSecret");
    /// //report 
    /// </code>
    ///     <para>
    ///         The appKey/sharedSecret can be found in our web once you have created an account: https://coderrapp.com. The
    ///         above
    ///         line will not detect any exceptions automatically, you have to report each exception by yourself:
    ///     </para>
    ///     <code>
    /// try
    /// {
    ///     //some business
    /// }
    /// catch (Exception ex)
    /// {
    ///     Err.Report(ex);
    /// }
    /// </code>
    ///     <para>
    ///         You can also include own context information by specifying a second argument:
    ///     </para>
    ///     <code>
    /// try
    /// {
    ///     //some business
    /// }
    /// catch (Exception ex)
    /// {
    ///     Err.Report(ex, yourDbEntity);
    /// }
    /// </code>
    ///     <para>
    ///         You can read more about reporting in the <see cref="Err" /> documentation.
    ///     </para>
    ///     <para>
    ///         If you are interested in providing your own context information automatically you should start by reading
    ///         the <see cref="ContextProviders" /> namespace and then add your own provider by using
    ///         <c>Err.Configuration.ContextProviders.Add()</c>.
    ///     </para>
    /// </summary>
    /// <seealso cref="Err" />
    /// <seealso cref="ErrConfiguration" />
    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }
}