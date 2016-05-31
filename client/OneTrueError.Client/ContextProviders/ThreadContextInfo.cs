using System;
using System.Globalization;
using System.Threading;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.ContextProviders
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
        public string Name
        {
            get { return NAME; }
        }

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
                info.Items.Add("Culture", Thread.CurrentThread.CurrentUICulture.IetfLanguageTag);
                info.Items.Add("Id", Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture));
                info.Items.Add("Name", Thread.CurrentThread.Name);
                info.Items.Add("IsBackground", Thread.CurrentThread.IsBackground.ToString(CultureInfo.InvariantCulture));
                if (Thread.CurrentThread.ExecutionContext != null)
                    info.Items.Add("ExecutionContext", Thread.CurrentThread.ExecutionContext.ToString());
                info.Items.Add("Priority", Thread.CurrentThread.Priority.ToString());
                info.Items.Add("ThreadState", Thread.CurrentThread.ThreadState.ToString());
                info.Items.Add("UICulture", Thread.CurrentThread.CurrentCulture.IetfLanguageTag);
            }
            catch (Exception ex)
            {
                info.Items.Add("CollectionException", "Failed to fetch thread info: " + ex);
            }
            return info;
        }
    }
}