using System;
using codeRR.Client.Contracts;

namespace codeRR.Client.ContextCollections
{
    /// <summary>
    ///     Partitioning is used to be able to drill down reports to see how errors affect your system or user base.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Do note that this feature is for codeRR Live and codeRR OnPremise only.
    ///     </para>
    /// </remarks>
    public class ErrPartitions : ContextCollectionDTO
    {
        public static string NAME = "ErrPartitions";

        /// <summary>
        ///     Creates a new instance of <see cref="ErrPartitions" />
        /// </summary>
        public ErrPartitions() : base(NAME)
        {
        }

        /// <summary>
        ///     Add a custom partition.
        /// </summary>
        /// <param name="partitionName">Name of the segmentation</param>
        /// <param name="partitionKey">Key to identify a specific entity in the partition.</param>
        public void AddPartition(string partitionName, string partitionKey)
        {
            if (partitionName == null) throw new ArgumentNullException(nameof(partitionName));
            Properties[partitionName] = partitionKey ?? throw new ArgumentNullException(nameof(partitionKey));
        }

        /// <summary>
        ///     Partition incident on tenant.
        /// </summary>
        /// <param name="tenantId">Currently affected tenant (organization, team, company or similar)</param>
        /// <remarks>
        ///     <para>
        ///         The string doesn't necessary have to be something that identifies the exact tenant, but can be a hashed id or
        ///         similar. The key objective is to see how many tenants that are affected and not who those tenants are.
        ///     </para>
        /// </remarks>
        public void SetTenant(string tenantId)
        {
            Properties["Tenant"] = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        }

        /// <summary>
        ///     Partition incident on users.
        /// </summary>
        /// <param name="userIdentifier">Currently affected user</param>
        /// <remarks>
        ///     <para>
        ///         The string doesn't necessary have to be something that identifies the user, but can be a hashed userId or
        ///         similar. The key objective is to see how many users that are affected and not who those users are.
        ///     </para>
        /// </remarks>
        public void SetUser(string userIdentifier)
        {
            Properties["User"] = userIdentifier ?? throw new ArgumentNullException(nameof(userIdentifier));
        }
    }
}