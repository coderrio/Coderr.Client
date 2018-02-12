using System;
using codeRR.Client.ContextCollections;
using codeRR.Client.Reporters;

namespace codeRR.Client.Config
{
    /// <summary>
    ///     Context used for the partition callback.
    /// </summary>
    public class PartitionContext
    {
        private readonly ErrPartitionContextCollection _contextCollection;

        /// <summary>
        ///     Creates a new instance of <see cref="PartitionContext" />.
        /// </summary>
        /// <param name="contextCollection">context to add partitions to.</param>
        /// <param name="reporterContext">
        ///     Context used when collecting all other context data (before partition collection is
        ///     invoked)
        /// </param>
        public PartitionContext(ErrPartitionContextCollection contextCollection, IErrorReporterContext2 reporterContext)
        {
            _contextCollection = contextCollection ?? throw new ArgumentNullException(nameof(contextCollection));
            ReporterContext = reporterContext;
        }

        /// <summary>
        ///     Context that the partition will be added to.
        /// </summary>
        public IErrorReporterContext2 ReporterContext { get; private set; }


        /// <summary>
        ///     Add a custom partition.
        /// </summary>
        /// <param name="partitionKey">Name of the segmentation</param>
        /// <param name="value">Value for the given partition key.</param>
        public void AddPartition(string partitionKey, string value)
        {
            if (partitionKey == null) throw new ArgumentNullException(nameof(partitionKey));
            _contextCollection.AddPartition(partitionKey, value);
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
            _contextCollection.SetTenant(tenantId);
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
            _contextCollection.SetUser(userIdentifier);
        }
    }
}