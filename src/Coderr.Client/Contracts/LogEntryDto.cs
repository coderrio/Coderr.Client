using System;

namespace Coderr.Client.Contracts
{
    /// <summary>
    ///     A log entry
    /// </summary>
    public class LogEntryDto
    {
        /// <summary>
        ///     Creates a new instance of <see cref="LogEntryDto" />.
        /// </summary>
        /// <param name="timestampUtc">when</param>
        /// <param name="logLevel">0 = trace, 1 = debug, 2 = info, 3 = warning, 4 = error, 5 = critical</param>
        /// <param name="message">message</param>
        public LogEntryDto(DateTime timestampUtc, int logLevel, string message)
        {
            TimestampUtc = timestampUtc;
            LogLevel = logLevel;
            Message = message;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="LogEntryDto" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Serialization constructor
        ///     </para>
        /// </remarks>
        protected LogEntryDto()
        {
        }

        /// <summary>
        ///     Exception as string (if any was attached to this log entry)
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        ///     0 = trace, 1 = debug, 2 = info, 3 = warning, 4 = error, 5 = critical
        /// </summary>
        public int LogLevel { get; set; }

        /// <summary>
        ///     Logged message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Location in the code that generated this log entry. Can be null.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        ///     When this log entry was written
        /// </summary>
        public DateTime TimestampUtc { get; set; }
    }
}