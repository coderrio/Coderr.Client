using System;
using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;
using OneTrueError.Client.Config;
using OneTrueError.Client.Log4Net;

// ReSharper disable once CheckNamespace

namespace OneTrueError.Client
{
    /// <summary>
    ///     Adds the OneTrueError logger to log4net.
    /// </summary>
    public static class ConfigExtensions
    {
        /// <summary>
        ///     Adds the OneTrueError logger to log4net.
        /// </summary>
        /// <param name="config">config</param>
        /// <exception cref="NotSupportedException">
        ///     This configuration/version of Log4Net do not allow dynamic adding of appenders.
        ///     Configure this adapter using code instead. See our online documentation for an example.
        /// </exception>
        public static void CatchLog4NetExceptions(this OneTrueConfiguration config)
        {
            if (config == null) throw new ArgumentNullException("config");

            var root = ((Hierarchy) LogManager.GetRepository()).Root;
            var attachable = root as IAppenderAttachable;
            if (attachable == null)
                throw new NotSupportedException(
                    "This configuration/version of Log4Net do not allow dynamic adding of appenders. Configure this adapter using code instead. See our online documentation for an example.");

            var appender = new OneTrueAppender();
            appender.ActivateOptions();
            attachable.AddAppender(appender);
        }
    }
}