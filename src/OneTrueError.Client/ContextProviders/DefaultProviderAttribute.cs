using System;

namespace OneTrueError.Client.ContextProviders
{
    /// <summary>
    ///     Use to indicate which providers are added into the collection process per default
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultProviderAttribute : Attribute
    {
    }
}