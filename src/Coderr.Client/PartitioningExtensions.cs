using System;
using System.Diagnostics;
using System.Linq;
using codeRR.Client.ContextCollections;
using codeRR.Client.Reporters;

namespace codeRR.Client
{
    /// <summary>
    ///     Partitioning is used to be able to understand the effect of an exception.
    /// </summary>
    /// <seealso cref="ErrPartitions" />
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
        ///         partitions, codeRR can tell you what impact the exception has on your system.
        ///     </para>
        /// </remarks>
        public static void AddPartition(this IErrorReporterContext context, string partitionName, string partitionKey)
        {
            if (partitionName == null) throw new ArgumentNullException(nameof(partitionName));
            if (partitionKey == null) throw new ArgumentNullException(nameof(partitionKey));

            if (!(context is IErrorReporterContext2 ctx2))
            {
                if (Debugger.IsAttached)
                    throw new NotSupportedException(
                        $"The current context,{context}, do not support custom collections.");

                return;
            }

            var collection = ctx2.ContextCollections.FirstOrDefault(x => x.Name == ErrPartitions.NAME);
            if (collection == null)
            {
                collection = new ErrPartitions();
                ctx2.ContextCollections.Add(collection);
            }

            collection.Properties.Add(partitionName, partitionKey);
        }
    }
}