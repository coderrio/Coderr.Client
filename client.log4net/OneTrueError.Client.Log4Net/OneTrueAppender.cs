using log4net.Appender;
using log4net.Core;

namespace OneTrueError.Client.Log4Net
{
    /// <summary>
    ///     Our appender for logging.
    /// </summary>
    /// <remarks>
    ///     <para>Will upload all log entries that contains exceptions to OneTrueError.</para>
    /// </remarks>
    public class OneTrueAppender : AppenderSkeleton
    {
        /// <summary>
        /// Uploads all log entries that contains an exception to OneTrueError.
        /// </summary>
        /// <param name="loggingEvent">The logging event.</param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            if (loggingEvent.ExceptionObject == null)
                return;

            OneTrue.Report(loggingEvent.ExceptionObject, new LogEntryDetails
            {
                LogLevel = loggingEvent.Level.ToString(),
                Message = loggingEvent.RenderedMessage,
                ThreadName = loggingEvent.ThreadName,
                Timestamp = loggingEvent.TimeStamp
            });
        }
    }
}