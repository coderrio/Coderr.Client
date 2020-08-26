using System;
using System.Collections.Generic;
using System.Security.Principal;
using Coderr.Client.ContextCollections.Providers;
using Coderr.Client.Contracts;

namespace Coderr.Client.ContextCollections
{
    /// <summary>
    ///     Can build collections for common context data.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         this class is not a <seealso cref="IContextCollectionProvider" />, but allow you to build and attach
    ///         collections manually to your error report.
    ///     </para>
    /// </remarks>
    public static class CollectionBuilder
    {
        /// <summary>
        ///     Information about the logged in user.
        /// </summary>
        /// <param name="principal">Logged in user</param>
        /// <returns>collection</returns>
        /// <exception cref="ArgumentNullException">principal</exception>
        public static ContextCollectionDTO CreateForCredentials(IPrincipal principal)
        {
            if (principal == null) throw new ArgumentNullException(nameof(principal));

            return CreateForCredentials(principal.Identity);
        }

        /// <summary>
        ///     Information about the logged in user.
        /// </summary>
        /// <param name="domainName">domain (company domain or your own segmentation)</param>
        /// <param name="userName">user or account name</param>
        /// <returns>collection</returns>
        /// <exception cref="ArgumentNullException">domainName;userName</exception>
        public static ContextCollectionDTO CreateForCredentials(string domainName, string userName)
        {
            if (domainName == null) throw new ArgumentNullException(nameof(domainName));
            if (userName == null) throw new ArgumentNullException(nameof(userName));

            var props = new Dictionary<string, string> { { "DomainName", domainName }, { "UserName", userName } };
            return new ContextCollectionDTO("UserCredentials", props);
        }

        /// <summary>
        ///     Information about the logged in user.
        /// </summary>
        /// <param name="identity">identity</param>
        /// <returns>collection</returns>
        /// <exception cref="ArgumentNullException">identity</exception>
        public static ContextCollectionDTO CreateForCredentials(IIdentity identity)
        {
            if (identity == null) throw new ArgumentNullException(nameof(identity));

            return UserCredentials.Create(identity);
        }

        /// <summary>
        ///     Tag incident server side.
        /// </summary>
        /// <param name="tags">Tags to add (StackOverflow-named tags or organization specific)</param>
        /// <returns>Collection</returns>
        /// <remarks>
        ///     <para>
        ///         Tags can be used to categorize and search after specific incidents.
        ///     </para>
        /// </remarks>
        public static ContextCollectionDTO CreateTags(params string[] tags)
        {
            if (tags == null) throw new ArgumentNullException(nameof(tags));
            if (tags.Length == 0) throw new ArgumentOutOfRangeException(nameof(tags), "Must specify at least one tag.");

            var props = new Dictionary<string, string> { { "ErrTags", string.Join(",", tags) } };
            return new ContextCollectionDTO("IncidentTags", props);
        }

        /// <summary>
        ///     Md5 hashes the username so that the user cannot be identified.
        /// </summary>
        /// <param name="identity">identity</param>
        /// <returns>collection</returns>
        /// <exception cref="ArgumentNullException">identity</exception>
        public static ContextCollectionDTO CreateTokenForCredentials(IIdentity identity)
        {
            if (identity == null) throw new ArgumentNullException(nameof(identity));

            return UserCredentials.CreateToken(identity);
        }

        /// <summary>
        ///     User experiencing the exception either want to get status updates or supplied a description of actions taken when
        ///     the exception was thrown.
        /// </summary>
        /// <param name="emailAddress">Want to receive status updates. (optional)</param>
        /// <param name="errorDescription">
        ///     error description supplied by the user. Hopefully steps to reproduce the error.
        ///     (optional)
        /// </param>
        /// <returns>collection</returns>
        public static ContextCollectionDTO Feedback(string emailAddress, string errorDescription)
        {
            var props = new Dictionary<string, string>();
            if (emailAddress != null)
                props.Add("EmailAddress", emailAddress);
            if (errorDescription != null)
                props.Add("Description", errorDescription);

            return new ContextCollectionDTO("UserSuppliedInformation", props);
        }

    }
}