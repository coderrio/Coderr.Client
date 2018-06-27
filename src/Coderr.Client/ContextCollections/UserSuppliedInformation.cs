using System.Collections.Generic;

namespace Coderr.Client.ContextCollections
{
    /// <summary>
    ///     Information supplied by the user about this error.
    /// </summary>
    public sealed class UserSuppliedInformation : IContextCollection
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UserSuppliedInformation" /> class.
        /// </summary>
        /// <param name="description">
        ///     Description entered by the user. i.e. information about what the user did when the exception
        ///     was thrown.
        /// </param>
        /// <param name="email">Email address (entered if the user want to get notified when the exception has been fixed).</param>
        public UserSuppliedInformation(string description, string email)
        {
            EmailAddress = email;
            Description = description;
        }

        /// <summary>
        ///     Description entered by the user. i.e. information about what the user did when the exception was thrown
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Email address if the user wants to receive a notification when the error has been fixed.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        ///     Specify the identity if you want to track which users are affected by an exception.
        /// </summary>
        public string UserIdentity { get; set; }

        string IContextCollection.CollectionName => "UserSuppliedInformation";

        IDictionary<string, string> IContextCollection.Properties => new Dictionary<string, string>
        {
            {"EmailAddress", EmailAddress},
            {"Description", Description},
            {"UserIdentity", UserIdentity}
        };
    }
}