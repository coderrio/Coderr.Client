using Coderr.Client.Contracts;

namespace Coderr.Client.Reporters
{
    /// <summary>
    /// A <see cref="IErrorReporterContext"/> specialization used to allow log attachment
    /// </summary>
    public interface IContextWithLogEntries
    {
        /// <summary>
        /// 100 last log entries (if specified, otherwise null)
        /// </summary>
        LogEntryDto[] LogEntries { get; set; }
    }
}