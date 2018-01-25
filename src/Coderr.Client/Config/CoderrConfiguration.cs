using System;
using System.Reflection;
using codeRR.Client.ContextProviders;
using codeRR.Client.Converters;
using codeRR.Client.Processor;
using codeRR.Client.Uploaders;

namespace codeRR.Client.Config
{
    /// <summary>
    ///     Configuration root object.
    /// </summary>
    public class CoderrConfiguration : IDisposable
    {
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
            UserInteraction.AskUserForDetails = true;
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
        ///     Used to decide which reports can be uploaded (for instance to sample reports in high volume systems).
        /// </summary>
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
        ///         You can use the <see cref="UploadDispatcher.UploadFailed" /> event to get aware of errors when this flag is set
        ///         to
        ///         <c>true</c>.
        ///     </para>
        /// </remarks>
        public bool ThrowExceptions { get; set; }

        /// <summary>
        ///     The objects used to upload reports to the codeRR service.
        /// </summary>
        public UploadDispatcher Uploaders { get; private set; }

        /// <summary>
        ///     Configure how the reporting UI will interact with the user.
        /// </summary>
        public UserInteractionConfiguration UserInteraction { get; } = new UserInteractionConfiguration();


        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
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
        /// <param name="coderrServerAddress">Host. Host and absolute path to the codeRR server</param>
        /// <param name="appKey">Appkey from the web site</param>
        /// <param name="sharedSecret">Shared secret from the web site</param>
        public void Credentials(Uri coderrServerAddress, string appKey, string sharedSecret)
        {
            if (coderrServerAddress == null) throw new ArgumentNullException(nameof(coderrServerAddress));
            if (appKey == null) throw new ArgumentNullException(nameof(appKey));
            if (sharedSecret == null) throw new ArgumentNullException(nameof(sharedSecret));

            if (ApplicationVersion == null)
                AssignAssemblyVersion(Assembly.GetCallingAssembly());

            Uploaders.Register(new UploadToCoderr(coderrServerAddress, appKey, sharedSecret));
        }

        /// <summary>
        ///     Dispose pattern.
        /// </summary>
        /// <param name="isDisposing">Invoked from the dispose method.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (Uploaders != null)
            {
                Uploaders.Dispose();
                Uploaders = null;
            }
        }
    }
}