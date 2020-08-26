using System;

namespace Coderr.Client.Processor
{
    /// <summary>
    ///     Used to be able to choose reportId id strategy
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The reportId id is the string which are used during communication between the end customers and the Err
    ///         error
    ///         customers.
    ///     </para>
    ///     <para>
    ///         The default implementation Base64 encodes a guid (the last "--" is removed from the Base64 string,"/" is
    ///         replaced with "_" and "+" is replaced with "-".
    ///     </para>
    ///     <para>
    ///         The ID must be globally unique (or at least within your own application scope).
    ///     </para>
    /// </remarks>
    /// <seealso cref="ShortGuid" />
    public class ReportIdGenerator
    {
        /// <summary>
        /// Used to configure the generator that this client will use
        /// </summary>
        public static ReportIdGenerator Instance = new ReportIdGenerator();

        private Func<Exception, string> _generator = report => ShortGuid.Encode(Guid.NewGuid());

        /// <summary>
        ///     Assign a custom ID generator.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <exception cref="System.ArgumentNullException">generator</exception>
        public static void Assign(Func<Exception, string> generator)
        {
            Instance.AssignImp(generator);
        }

        /// <summary>
        ///     Assign a custom ID generator.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <exception cref="System.ArgumentNullException">generator</exception>
        public void AssignImp(Func<Exception, string> generator)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        }

        /// <summary>
        ///     Generate a new ID
        /// </summary>
        /// <param name="exception">Exception to get an reportId for</param>
        /// <returns>reportId</returns>
        /// <exception cref="System.ArgumentNullException">exception</exception>
        public static string Generate(Exception exception)
        {
            return Instance.GenerateImp(exception);
        }

        /// <summary>
        ///     Generate a new ID
        /// </summary>
        /// <param name="exception">Exception to get an reportId for</param>
        /// <returns>reportId</returns>
        /// <exception cref="System.ArgumentNullException">exception</exception>
        public string GenerateImp(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));

            return _generator(exception);
        }
    }
}