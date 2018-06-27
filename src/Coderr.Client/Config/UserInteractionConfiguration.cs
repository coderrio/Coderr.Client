namespace Coderr.Client.Config
{
    /// <summary>
    ///     Adopts the user interface and the reporting flow.
    /// </summary>
    public class UserInteractionConfiguration
    {
        /// <summary>
        ///     Ask the user if he/she want to receive progress updates from you.
        /// </summary>
        public bool AskForEmailAddress { get; set; }

        /// <summary>
        ///     Display a dialog for the user where he/she can enter information about what he/she did when the exception was
        ///     thrown.
        /// </summary>
        public bool AskUserForDetails { get; set; }

        /// <summary>
        ///     Ask the user if we may upload the error report.
        /// </summary>
        public bool AskUserForPermission { get; set; }
    }
}