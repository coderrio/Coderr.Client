
// Keeps in the root namespace to get intelli sense

// ReSharper disable once CheckNamespace

using System;
using OneTrueError.Client.Config;
using OneTrueError.Client.Wpf;
using OneTrueError.Client.Wpf.ContextProviders;

namespace OneTrueError.Client
{
    /// <summary>
    ///     Use <c>OneTrue.Configuration.CatchWpfExceptions()</c> to get started.
    /// </summary>
    public static class ConfigExtensions
    {
        /// <summary>
        ///     Catch all uncaught wpf exceptions.
        /// </summary>
        /// <param name="configurator">OneTrueError configurator (accessed through <see cref="OneTrue.Configuration" />).</param>
        public static void CatchWinFormsExceptions(this OneTrueConfiguration configurator)
        {
            if (configurator == null) throw new ArgumentNullException("configurator");
            WpfErrorReporter.Activate();
            OneTrue.Configuration.ContextProviders.Add(new OpenWindowsCollector());
        }
    }
}
