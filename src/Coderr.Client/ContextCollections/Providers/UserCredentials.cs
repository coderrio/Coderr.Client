using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Coderr.Client.Contracts;

namespace Coderr.Client.ContextCollections.Providers
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
    public sealed class UserCredentials : ContextCollectionDTO
    {
        private const uint MurmurSeed = 0xc17ca3d3;

        /// <summary>
        ///     Creates a new instance of <see cref="UserCredentials" />.
        /// </summary>
        /// <param name="identity">
        ///     Identity, typically from <c>Thread.CurrentPrincipal.Identity</c> or <c>Request.User.Identity</c>
        ///     .
        /// </param>
        public UserCredentials(IIdentity identity) : base("UserCredentials")
        {
            if (identity == null) throw new ArgumentNullException("identity");
            Properties = new ConcurrentDictionary<string, string>();

            if (string.IsNullOrEmpty(identity.Name))
                Properties.Add("UserName", "[Anonynmous]");
            else
                SplitAccountName(identity.Name);
        }

        /// <summary>
        ///     Creates a new instance of <see cref="UserCredentials" />.
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="domainName">Domain name</param>
        public UserCredentials(string domainName, string userName) : base("UserCredentials")
        {
            DomainName = userName ?? throw new ArgumentNullException(nameof(userName));
            DomainName = domainName ?? throw new ArgumentNullException(nameof(domainName));
        }

        /// <summary>
        ///     Creates a new instance of <see cref="UserCredentials" />.
        /// </summary>
        /// <param name="userName">User name without domain (i.e. should not include "domainName\")</param>
        public UserCredentials(string userName) : base("UserCredentials")
        {
            SplitAccountName(userName);
        }

        /// <summary>
        /// Domain name (if any)
        /// </summary>
        public string DomainName
        {
            get => Properties["DomainName"];
            set => Properties["DomainName"] = value;
        }

        /// <summary>
        /// User name
        /// </summary>
        public string UserName
        {
            get => Properties["UserName"];
            set => Properties["UserName"] = value;
        }


        /// <summary>
        /// User name, but hashed with MurmurHash to make the user anonymous.
        /// </summary>
        public string UserToken
        {
            get => Properties["UserToken"];
            set => Properties["UserToken"] = value;
        }


        /// <summary>
        ///     checks if the account name contains a domain name
        /// </summary>
        /// <param name="accountName">samAccountName</param>
        /// <returns>Item 1 = domain (or null), item2 = userName</returns>
        private static Tuple<string, string> SplitAccountName(string accountName)
        {
            if (accountName == null) throw new ArgumentNullException(nameof(accountName));

            var pos = accountName.IndexOf("\\", StringComparison.Ordinal);
            if (pos == -1)
                return new Tuple<string, string>(null, accountName);

            var a = accountName.Substring(0, pos);
            var b = accountName.Substring(pos + 1);
            return new Tuple<string, string>(a, b);

        }


        /// <summary>
        ///     Md5 hashes the username so that the user cannot be identified.
        /// </summary>
        /// <param name="identity">identity</param>
        /// <returns>collection</returns>
        /// <exception cref="ArgumentNullException">identity</exception>
        public static ContextCollectionDTO CreateToken(IIdentity identity)
        {
            if (identity == null) throw new ArgumentNullException(nameof(identity));

            var props = new Dictionary<string, string>();

            var userDomain = SplitAccountName(identity.Name);

            var buffer = Encoding.UTF8.GetBytes(userDomain.Item2);
            var hash = MurmurHash2.Hash(buffer, MurmurSeed);
            props["UserToken"] = hash.ToString("x");

            if (userDomain.Item1 != null)
                props.Add("DomainName", userDomain.Item1);
            props.Add("IsAuthenticated", identity.IsAuthenticated ? "true" : "false");
            props.Add("AuthenticationType", identity.AuthenticationType);

            return new ContextCollectionDTO("UserCredentials", props);
        }

        /// <summary>
        ///     Md5 hashes the username so that the user cannot be identified.
        /// </summary>
        /// <param name="identity">identity</param>
        /// <returns>collection</returns>
        /// <exception cref="ArgumentNullException">identity</exception>
        public static ContextCollectionDTO Create(IIdentity identity)
        {
            if (identity == null) throw new ArgumentNullException(nameof(identity));

            var props = new Dictionary<string, string>();

            var userDomain = SplitAccountName(identity.Name);

            props["UserName"] = userDomain.Item2;
            if (userDomain.Item1 != null)
                props.Add("DomainName", userDomain.Item1);
            props.Add("IsAuthenticated", identity.IsAuthenticated ? "true" : "false");
            props.Add("AuthenticationType", identity.AuthenticationType);

            return new ContextCollectionDTO("UserCredentials", props);
        }


    }
}