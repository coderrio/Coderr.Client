using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Coderr.Client.ContextCollections
{
    /// <summary>
    ///     Carries the user credentials to the server.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Creates a context collection with the name <c>UserCredentials</c> with the properties <c>UserName</c> and if
    ///         available <c>DomainName</c>.
    ///     </para>
    /// </remarks>
    public sealed class UserCredentials : IContextCollection
    {
        private string _domainName;
        private string _userName;

        /// <summary>
        ///     Creates a new instance of <see cref="UserCredentials" />.
        /// </summary>
        /// <param name="identity">
        ///     Identity, typically from <c>Thread.CurrentPrincipal.Identity</c> or <c>Request.User.Identity</c>
        ///     .
        /// </param>
        public UserCredentials(IIdentity identity)
        {
            if (identity == null) throw new ArgumentNullException("identity");
            if (string.IsNullOrEmpty(identity.Name))
                _userName = "[Anonymous]";
            else
                SplitAccountName(identity.Name);
        }

        /// <summary>
        ///     Creates a new instance of <see cref="UserCredentials" />.
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="domainName">Domain name</param>
        public UserCredentials(string domainName, string userName)
        {
            _userName = userName ?? throw new ArgumentNullException(nameof(userName));
            _domainName = domainName ?? throw new ArgumentNullException(nameof(domainName));
        }

        /// <summary>
        ///     Creates a new instance of <see cref="UserCredentials" />.
        /// </summary>
        /// <param name="userName">User name without domain (i.e. should not include "domainName\")</param>
        public UserCredentials(string userName)
        {
            SplitAccountName(userName);
        }

        string IContextCollection.CollectionName => "UserCredentials";

        IDictionary<string, string> IContextCollection.Properties
        {
            get
            {
                var dic = new Dictionary<string, string>
                {
                    {"UserName", _userName}
                };
                if (!string.IsNullOrEmpty(_domainName))
                    dic.Add("DomainName", _domainName);

                return dic;
            }
        }

        private void SplitAccountName(string accountName)
        {
            var pos = accountName.IndexOf("\\", StringComparison.Ordinal);
            if (pos != -1)
            {
                _domainName = accountName.Substring(0, pos);
                _userName = accountName.Substring(pos + 1);
            }
            else
            {
                _userName = accountName;
            }
        }
    }
}