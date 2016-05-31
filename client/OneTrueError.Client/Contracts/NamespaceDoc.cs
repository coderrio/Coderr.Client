using System.Runtime.CompilerServices;

namespace OneTrueError.Client.Contracts
{
    /// <summary>
    ///     The DTO's used to submit information to OneTrueError.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Everything is serialized to JSON and then DTO is then compressed using ZIP before being uploaded to
    ///         OneTrueError. The body
    ///         is signed using HMAC and the signature is provided as <c>sig</c> in the query string.
    ///     </para>
    /// </remarks>
    [CompilerGenerated]
    public class NamespaceDoc
    {
    }
}