using System;
using System.Globalization;
using System.Threading;
using codeRR.Client.Contracts;
using codeRR.Client.Reporters;

namespace codeRR.Client.ContextProviders
{
    /// <summary>
    ///     Collects information from <c>Thread.CurrentThread</c> to a context called "Thread".
    /// </summary>
    [DefaultProvider]
    public class ThreadContextInfo : IContextInfoProvider
    {
        /// <summary>
        ///     "Thread"
        /// </summary>
        public const string NAME = "Thread";

        /// <summary>
        ///     "Thread"
        /// </summary>
        public string Name => NAME;

        /// <summary>
        ///     Collects information from <c>Thread.CurrentThread</c> to a context called "Thread".
        /// </summary>
        /// <param name="context">Contains information about the currently processed exception and where it came from.</param>
        /// <returns>generated context info</returns>
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            var info = new ContextCollectionDTO(NAME);
            try
            {
                info.Properties.Add("Culture", Thread.CurrentThread.CurrentUICulture.IetfLanguageTag);
                info.Properties.Add("Id", Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture));
                info.Properties.Add("Name", Thread.CurrentThread.Name);
                info.Properties.Add("IsBackground",
                    Thread.CurrentThread.IsBackground.ToString(CultureInfo.InvariantCulture));
                if (Thread.CurrentThread.ExecutionContext != null)
                    info.Properties.Add("ExecutionContext", Thread.CurrentThread.ExecutionContext.ToString());
                info.Properties.Add("Priority", Thread.CurrentThread.Priority.ToString());
                info.Properties.Add("ThreadState", Thread.CurrentThread.ThreadState.ToString());
                info.Properties.Add("UICulture", Thread.CurrentThread.CurrentCulture.IetfLanguageTag);
            }
            catch (Exception ex)
            {
                info.Properties.Add("CollectionException", "Failed to fetch thread info: " + ex);
            }
            return info;
        }
    }
}