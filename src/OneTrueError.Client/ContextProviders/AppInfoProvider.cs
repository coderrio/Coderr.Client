using System;
using System.Diagnostics;
using System.Globalization;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.ContextProviders
{
    /// <summary>
    ///     Collects information about the currently running application.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The collection is named <c>ApplicationInfo</c> and the collected information is:
    ///     </para>
    ///     <list type="table">
    ///         <listheader>
    ///             <term>Property name</term>
    ///             <description>Description</description>
    ///         </listheader>
    ///         <item>
    ///             <term>ThreadCount</term>
    ///             <description>Number of OS threads.</description>
    ///         </item>
    ///         <item>
    ///             <term>HandleCount</term>
    ///             <description>Amount of OS handles that your application have opened.</description>
    ///         </item>
    ///         <item>
    ///             <term>StartTime</term>
    ///             <description>When the process was started.</description>
    ///         </item>
    ///         <item>
    ///             <term>TotalProcessorTime</term>
    ///             <description>
    ///                 Total amount used by your process (including OS time like reading from files or sending stuff
    ///                 over a socket).
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <term>UserProcessorTime</term>
    ///             <description>Amount of time used to execute your code.</description>
    ///         </item>
    ///         <item>
    ///             <term>CurrentDirectory</term>
    ///             <description>Current directory.</description>
    ///         </item>
    ///         <item>
    ///             <term>MainModule</term>
    ///             <description>
    ///                 Executable used to start the process, including the module name, file name, and module memory
    ///                 details.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <term>ProcessName</term>
    ///             <description>Executable file name, without path and file extension.</description>
    ///         </item>
    ///         <item>
    ///             <term>WorkingSet</term>
    ///             <description>number of bytes of physical memory mapped to the process context.</description>
    ///         </item>
    ///         <item>
    ///             <term>VirtualMemorySize</term>
    ///             <description>Current size of virtual memory used by the process.</description>
    ///         </item>
    ///         <item>
    ///             <term>PrivateMemorySize</term>
    ///             <description>
    ///                 Current size of memory used by the process that cannot be shared with other processes (i.e.
    ///                 dedicated memory).
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <term>BasePriority</term>
    ///             <description>Starting priority for threads created within the associated process.</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    [DefaultProvider]
    public class AppInfoProvider : IContextInfoProvider
    {
        /// <summary>
        ///     Name of the context collection
        /// </summary>
        public const string NameConstant = "ApplicationInfo";

        /// <summary>
        ///     Gets "ApplicationInfo"
        /// </summary>
        public string Name => NameConstant;

        /// <summary>
        ///     Collect information
        /// </summary>
        /// <param name="context">Context information provided by the class which reported the error.</param>
        /// <returns>
        ///     Collection. Items with multiple values are joined using <c>";;"</c>
        /// </returns>
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            var info = new ContextCollectionDTO(Name);
            info.Properties.Add("WorkingSet", Environment.WorkingSet.ToString(CultureInfo.InvariantCulture));
            info.Properties.Add("CurrentDirectory", Environment.CurrentDirectory);

            try
            {
                var process = Process.GetCurrentProcess();
                info.Properties.Add("ThreadCount", process.Threads.Count.ToString(CultureInfo.InvariantCulture));
                info.Properties.Add("StartTime", process.StartTime.ToString(CultureInfo.InvariantCulture));
                info.Properties.Add("TotalProcessorTime", process.TotalProcessorTime.ToString());
                info.Properties.Add("UserProcessorTime", process.UserProcessorTime.ToString());
                info.Properties.Add("HandleCount", process.HandleCount.ToString(CultureInfo.InvariantCulture));
                info.Properties.Add("ProcessName", process.ProcessName);
                info.Properties.Add("MainModule", process.MainModule.ToString());
                info.Properties.Add("BasePriority", process.BasePriority.ToString(CultureInfo.InvariantCulture));
                info.Properties.Add("VirtualMemorySize",
                    process.VirtualMemorySize64.ToString(CultureInfo.InvariantCulture));
                info.Properties.Add("PrivateMemorySize",
                    process.PrivateMemorySize64.ToString(CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                info.Properties.Add("CollectionException", "Failed to fetch process info: " + ex);
            }

            return info;
        }
    }
}