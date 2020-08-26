using System;
using System.Collections.Generic;
using System.Reflection;
using Coderr.Client.ContextCollections;
using Coderr.Client.Processor;
using Coderr.Client.Uploaders;

namespace Coderr.Client.Config
{
    /// <summary>
    ///     Configuration root object.
    /// </summary>
    public class CoderrConfiguration : IDisposable
    {
        internal readonly List<Action<PartitionContext>> PartitionCallbacks = new List<Action<PartitionContext>>();

        /// <summary>
        ///     Configure how the reporting UI will interact with the user.
        /// </summary>
        private UserInteractionConfiguration _userInteraction = new UserInteractionConfiguration();

        /// <summary>
        ///     Used to be able to process error reports before they are delivered.
        /// </summary>
        public ExceptionPreProcessorHandler ExceptionPreProcessor;

        /// <summary>
        ///     Visit generated reports before they are sent.
        /// </summary>
        public ReportPreProcessorHandler ReportPreProcessor;

        /// <summary>
        ///     Creates a new instance of <see cref="CoderrConfiguration" />.
        /// </summary>
        public CoderrConfiguration()
        {
            Uploaders = new UploadDispatcher(this);
            _userInteraction.AskUserForDetails = true;
            ThrowExceptions = true;
            MaxNumberOfPropertiesPerCollection = 100;
        }


        /// <summary>
        ///     Creates a new instance of <see cref="CoderrConfiguration" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">uploadDispatcher</exception>
        public CoderrConfiguration(IUploadDispatcher uploadDispatcher)
        {
            Uploaders = uploadDispatcher ?? throw new ArgumentNullException(nameof(uploadDispatcher));
            _userInteraction.AskUserForDetails = true;
            ThrowExceptions = true;
            MaxNumberOfPropertiesPerCollection = 100;
        }

        /// <summary>
        ///     Version of your application
        /// </summary>
        /// <see cref="AssignAssemblyVersion(System.Reflection.Assembly)" />
        public string ApplicationVersion { get; private set; }

        /// <summary>
        ///     Used to add custom context info providers.
        /// </summary>
        public ContextProvidersRegistrar ContextProviders { get; } = new ContextProvidersRegistrar();

        /// <summary>
        /// Which environment are we running in? Dev, Production etc.
        /// </summary>
        public string EnvironmentName { get; set; }

        /// <summary>
        ///     Used to decide which reports can be uploaded.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Use it to filter out reports from certain areas in the system or to use sampling in high load systems.
        ///     </para>
        ///     <para>Collection is empty unless you add filters to it.</para>
        /// </remarks>
        public ReportFilterDispatcher FilterCollection { get; } = new ReportFilterDispatcher();

        /// <summary>
        ///     Limit the amount of properties that can be collected per context collection.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Some collections can contain thousands of properties when building collections by reflecting objects. Those
        ///         will take time to process and analyze by the server
        ///         when a lot of reports are uploaded for the same incident. To limit that you can specify a property limit wich
        ///         will make the <see cref="ObjectToContextCollectionConverter" />
        ///         stop after a certain amount of properties (when invoked from within the library).
        ///     </para>
        /// </remarks>
        /// <value>
        ///     Default is 100.
        /// </value>
        public int MaxNumberOfPropertiesPerCollection { get; set; }

        /// <summary>
        ///     Queue reports and upload them in the background.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This option is great if you do not want to freeze the UI while reports are being uploaded. They are queued in
        ///         an internal
        ///         queue until being uploaded in orderly fashion.
        ///     </para>
        /// </remarks>
        public bool QueueReports { get; set; }

        /// <summary>
        ///     The library may throw exceptions if the server cannot be contacted / accept the upload.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Default is <c>true</c>, turn of before going to production.
        ///     </para>
        ///     <para>
        ///         This option is not used when <seealso cref="QueueReports" /> is <c>true</c>.
        ///     </para>
        /// </remarks>
        public bool ThrowExceptions { get; set; }

        /// <summary>
        ///     The objects used to upload reports to the codeRR service.
        /// </summary>
        public IUploadDispatcher Uploaders { get; private set; }

        /// <summary>
        ///     Configure how the reporting UI will interact with the user.
        /// </summary>
        public UserInteractionConfiguration UserInteraction
        {
            get => _userInteraction;
            set => _userInteraction = value;
        }


        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     Configure a callback which is used to attach partitions to the error report
        /// </summary>
        /// <param name="callback"></param>
        /// <example>
        ///     <code>
        /// // Example for ASP.NET
        /// Err.Configuration.AddPartition(ctx => {
        ///    var aspNetContext = context as AspNetContext;
        /// 
        ///    // this check is required since different contexts are used
        ///    // if you use multiple client libraries.
        ///    if (aspNetContext?.HttpContext == null)
        ///        return null;
        /// 
        ///    ctx.AddPartition("DeviceId", ctx.HttpContext.Session["DeviceId"]);
        /// });
        /// </code>
        /// </example>
        public void AddPartition(Action<PartitionContext> callback)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            PartitionCallbacks.Add(callback);
        }

        /// <summary>
        ///     Version of the entry assembly in the user application
        /// </summary>
        /// <param name="assembly">Assembly containing the application version</param>
        public void AssignAssemblyVersion(Assembly assembly)
        {
            ApplicationVersion = assembly.GetName().Version?.ToString();
            if (ApplicationVersion == "0.0.0.0")
                ApplicationVersion = null;
        }

        /// <summary>
        ///     Your application version
        /// </summary>
        /// <param name="version">Assembly version, format: "1.0.0.0"</param>
        public void AssignAssemblyVersion(string version)
        {
            ApplicationVersion = version;
        }

        /// <summary>
        ///     Configure uploads
        /// </summary>
        /// <param name="oneTrueHost">Host. Host and absolute path to the coderr directory</param>
        /// <param name="appKey">Appkey from the web site</param>
        /// <param name="sharedSecret">Shared secret from the web site</param>
        public void Credentials(Uri oneTrueHost, string appKey, string sharedSecret)
        {
            if (oneTrueHost == null) throw new ArgumentNullException("oneTrueHost");
            if (appKey == null) throw new ArgumentNullException("appKey");
            if (sharedSecret == null) throw new ArgumentNullException("sharedSecret");

            if (ApplicationVersion == null)
            {
#if NETSTANDARD2_0
                AssignAssemblyVersion(Assembly.GetCallingAssembly());
#else
                AssignAssemblyVersion(Assembly.GetEntryAssembly());
#endif
            }
                

            Uploaders.Register(new UploadToCoderr(oneTrueHost, appKey, sharedSecret));
        }

        /// <summary>
        ///     Dispose pattern.
        /// </summary>
        /// <param name="isDisposing">Invoked from the dispose method.</param>
        protected virtual void Dispose(bool isDisposing)
        {
        }
    }
}