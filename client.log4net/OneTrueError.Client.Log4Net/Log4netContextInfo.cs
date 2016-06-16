using System;
using log4net.Core;

namespace OneTrueError.Client.Log4Net
{
    /// <summary>
    /// </summary>
    public class Log4netContextInfo
    {
        /// <summary>
        ///     <c>Type.FullName</c> of the logging type.
        /// </summary>
        public string LoggingType { get; set; }

        /// <summary>
        ///     Log level from <see cref="Level" />
        /// </summary>
        public string LogLevel { get; set; }

        /// <summary>
        ///     Logged message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Thread that logged
        /// </summary>
        public string ThreadName { get; set; }

        /// <summary>
        ///     When the entry was logged
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}