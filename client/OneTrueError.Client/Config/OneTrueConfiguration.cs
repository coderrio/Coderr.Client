using System;
using OneTrueError.Client.ContextProviders;
using OneTrueError.Client.Processor;
using OneTrueError.Client.Uploaders;

namespace OneTrueError.Client.Config
{
    /// <summary>
    ///     Configuration root object.
    /// </summary>
    public class OneTrueConfiguration : IDisposable
    {
        private readonly ContextProvidersRegistrar _contextProviders = new ContextProvidersRegistrar();
        private readonly ReportFilterDispatcher _filterDispatcher = new ReportFilterDispatcher();

        /// <summary>
        ///     Configure how the reporting UI will interact with the user.
        /// </summary>
        private UserInteractionConfiguration _userInteraction = new UserInteractionConfiguration();

        /// <summary>
        ///     Creates a new instance of <see cref="OneTrueConfiguration" />.
        /// </summary>
        public OneTrueConfiguration()
        {
            Uploaders = new UploadDispatcher(this);
            _userInteraction.AskUserForDetails = true;
            ThrowExceptions = true;
        }


        /// <summary>
        ///     Used to add custom context info providers.
        /// </summary>
        public ContextProvidersRegistrar ContextProviders
        {
            get { return _contextProviders; }
        }

        /// <summary>
        ///     Used to decide which reports can be uploaded (for instance to sample reports in high volume systems).
        /// </summary>
        public ReportFilterDispatcher FilterCollection
        {
            get { return _filterDispatcher; }
        }

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
        ///     The objects used to upload reports to the OneTrueError service.
        /// </summary>
        public UploadDispatcher Uploaders { get; private set; }

        /// <summary>
        ///     Configure how the reporting UI will interact with the user.
        /// </summary>
        public UserInteractionConfiguration UserInteraction
        {
            get { return _userInteraction; }
            set { _userInteraction = value; }
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
        ///     Configure uploads
        /// </summary>
        /// <param name="oneTrueHost">Host. Host and absolute path to the onetrueerror directory</param>
        /// <param name="appKey">Appkey from the web site</param>
        /// <param name="sharedSecret">Shared secret from the web site</param>
        public void Credentials(Uri oneTrueHost, string appKey, string sharedSecret)
        {
            if (oneTrueHost == null) throw new ArgumentNullException("oneTrueHost");
            if (appKey == null) throw new ArgumentNullException("appKey");
            if (sharedSecret == null) throw new ArgumentNullException("sharedSecret");
            Uploaders.Register(new UploadToOneTrueError(oneTrueHost, appKey, sharedSecret));
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