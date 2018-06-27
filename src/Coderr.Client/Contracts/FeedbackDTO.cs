using Coderr.Client.Processor;

namespace Coderr.Client.Contracts
{
    /// <summary>
    ///     To be able to write feedback after the actual error have been sent.
    /// </summary>
    public class FeedbackDTO
    {
        /// <summary>
        ///     Description written by the user about what he/she did when the error occurred.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Email address to user (if he/she would like to get status updates)
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        ///     Id returned from <see cref="ReportIdGenerator.Generate" /> (which is used when generating the error report).
        /// </summary>
        public string ReportId { get; set; }
    }
}