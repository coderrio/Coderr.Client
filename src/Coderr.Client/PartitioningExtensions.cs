using System;
using Coderr.Client.ContextCollections;
using Coderr.Client.Reporters;

namespace Coderr.Client
{
    /// <summary>
    ///     Partitioning is used to be able to understand the effect of an exception.
    /// </summary>
    public static class PartitioningExtensions
    {
        /// <summary>
        ///     Add a new partition
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="partitionName">partition</param>
        /// <param name="partitionKey">partitionKey</param>
        /// <remarks>
        ///     <para>
        ///         What partitions are depends on your application. It can be tenants, server clusters, users. By adding
        ///         partitions, codeRR can
        ///         tell you what impact the exception has on your system.
        ///     </para>
        /// </remarks>
        public static void AddPartition(this IErrorReporterContext context, string partitionName, string partitionKey)
        {
            if (partitionName == null) throw new ArgumentNullException(nameof(partitionName));
            if (partitionKey == null) throw new ArgumentNullException(nameof(partitionKey));

            var collection = context.GetCoderrCollection();
            collection.Properties.Add($"ErrPartition.{partitionName}", partitionKey);
        }
    }
}