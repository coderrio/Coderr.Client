using System;
using System.Collections.Generic;
using System.Linq;
using Coderr.Client.Config;
using Coderr.Client.ContextCollections;
using Coderr.Client.Contracts;
using Coderr.Client.Reporters;

namespace Coderr.Client.Processor
{
    /// <summary>
    ///     Will process the exception to generate context info and then upload it to the server.
    /// </summary>
    public class ExceptionProcessor : IExceptionProcessor
    {
        internal const string AlreadyReportedSetting = "ErrSetting.Reported";
        internal const string AppAssemblyVersion = "AppAssemblyVersion";
        private readonly CoderrConfiguration _configuration;

        /// <summary>
        ///     Creates a new instance of <see cref="ExceptionProcessor" />.
        /// </summary>
        /// <param name="configuration">Current configuration.</param>
        /// <exception cref="ArgumentNullException">configuration</exception>
        public ExceptionProcessor(CoderrConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        ///     Build an report, but do not upload it
        /// </summary>
        /// <param name="exception">caught exception</param>
        /// <remarks>
        ///     <para>
        ///         Will collect context info and generate a report.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">exception</exception>
        public ErrorReportDTO Build(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            if (IsReported(exception))
                return null;

            var context = new ErrorReporterContext(null, exception);
            return Build(context);
        }

        /// <summary>
        ///     Build an report, but do not upload it
        /// </summary>
        /// <param name="exception">caught exception</param>
        /// <param name="contextData">context data</param>
        /// <remarks>
        ///     <para>
        ///         Will collect context info and generate a report.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">exception;contextData</exception>
        public ErrorReportDTO Build(Exception exception, object contextData)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            if (contextData == null) throw new ArgumentNullException(nameof(contextData));
            if (IsReported(exception))
                return null;

            var context = new ErrorReporterContext(null, exception);
            AppendCustomContextData(contextData, context.ContextCollections);
            return Build(context);
        }

        /// <summary>
        ///     Build an report, but do not upload it
        /// </summary>
        /// <param name="context">
        ///     context passed to all context providers when collecting information. This context is typically
        ///     implemented by one of the integration libraries to provide more context that can be used to process the
        ///     environment.
        /// </param>
        /// <remarks>
        ///     <para>
        ///         Will collect context info and generate a report.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">exception;contextData</exception>
        public ErrorReportDTO Build(IErrorReporterContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.Exception is CoderrClientException)
                return null;
            if (IsReported(context.Exception))
                return null;
            ErrorReporterContext.MoveCollectionsInException(context.Exception, context.ContextCollections);
            InvokePreProcessor(context);

            _configuration.ContextProviders.Collect(context);

            // Invoke partition collection AFTER other context info providers
            // since those other collections might provide the property that
            // we want to create partions on.
            InvokePartitionCollection(context);

            var reportId = ReportIdGenerator.Generate(context.Exception);
            AddAddemblyVersion(context.ContextCollections);
            var report = new ErrorReportDTO(reportId, new ExceptionDTO(context.Exception),
                context.ContextCollections.ToArray())
            {
                EnvironmentName = _configuration.EnvironmentName,
                LogEntries = (context as IContextWithLogEntries)?.LogEntries
            };
            return report;
        }

        /// <summary>
        ///     Process exception.
        /// </summary>
        /// <param name="exception">caught exception</param>
        /// <remarks>
        ///     <para>
        ///         Will collect context info, generate a report, go through filters and finally upload it.
        ///     </para>
        /// </remarks>
        public void Process(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));

            if (IsReported(exception))
                return;

            var report = Build(exception);
            if (UploadReportIfAllowed(report))
                MarkAsReported(exception);
        }

        /// <summary>
        ///     Process exception.
        /// </summary>
        /// <param name="context">
        ///     Used to reports (like for ASP.NET) can attach information which can be used during the context
        ///     collection pipeline.
        /// </param>
        /// <remarks>
        ///     <para>
        ///         Will collect context info, generate a report, go through filters and finally upload it.
        ///     </para>
        /// </remarks>
        /// <seealso cref="IReportFilter" />
        public void Process(IErrorReporterContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (IsReported(context.Exception))
                return;

            ErrorReporterContext.MoveCollectionsInException(context.Exception, context.ContextCollections);
            var report = Build(context);
            if (UploadReportIfAllowed(report))
                MarkAsReported(context.Exception);
        }


        /// <summary>
        ///     Process exception and upload the generated error report (along with context data)
        /// </summary>
        /// <param name="exception">caught exception</param>
        /// <param name="contextData">Context data</param>
        /// <remarks>
        ///     <para>
        ///         Will collect context info, generate a report, go through filters and finally upload it.
        ///     </para>
        ///     <para>
        ///         Do note that reports can be discarded if a filter in <c>Err.Configuration.FilterCollection</c> says so.
        ///     </para>
        /// </remarks>
        public void Process(Exception exception, object contextData)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            if (IsReported(exception))
                return;

            var report = Build(exception, contextData);
            if (UploadReportIfAllowed(report))
                MarkAsReported(exception);
        }

        internal void AddAddemblyVersion(IList<ContextCollectionDTO> contextInfo)
        {
            if (_configuration.ApplicationVersion == null)
                return;

            var col = contextInfo.GetCoderrCollection();
            col.Properties.Add(AppAssemblyVersion, _configuration.ApplicationVersion);
            contextInfo.Add(col);
        }

        private static void AppendCustomContextData(object contextData, IList<ContextCollectionDTO> contextInfo)
        {
            if (contextData is IEnumerable<ContextCollectionDTO> dtos)
            {
                var arr = dtos;
                foreach (var dto in arr)
                    contextInfo.Add(dto);
            }
            else
            {
                var col = contextData.ToContextCollection();
                if (col.Properties.ContainsKey("ErrTags"))
                {
                    var coderrCollection = contextInfo.GetCoderrCollection();
                    if (coderrCollection.Properties.TryGetValue("ErrTags", out var value))
                    {
                        coderrCollection.Properties["ErrTags"] = value + "," + col.Properties["ErrTags"];
                    }
                    else
                    {
                        coderrCollection.Properties["ErrTags"] = col.Properties["ErrTags"];
                    }

                    col.Properties.Remove("ErrTags");
                }

                if (col.Properties.Count > 0)
                    contextInfo.Add(col);
            }
        }


        private void InvokePartitionCollection(IErrorReporterContext context)
        {
            var col = context.GetCoderrCollection();
            var partitionContext = new PartitionContext(col, context);
            foreach (var callback in _configuration.PartitionCallbacks)
            {
                try
                {
                    callback(partitionContext);
                }
                catch (Exception ex)
                {
                    col.Properties.Add("PartitionCollection.Err", $"Callback {callback} failed: {ex}");
                }
            }
        }

        private void InvokePreProcessor(IErrorReporterContext context)
        {
            _configuration.ExceptionPreProcessor?.Invoke(context);
        }

        private bool IsReported(Exception exception)
        {
            return exception.Data.Contains(AlreadyReportedSetting);
        }

        private void MarkAsReported(Exception exception)
        {
            exception.Data[AlreadyReportedSetting] = true;
        }

        private bool UploadReportIfAllowed(ErrorReportDTO report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            var canUpload = _configuration.FilterCollection.CanUploadReport(report);
            if (!canUpload)
                return false;

            _configuration.Uploaders.Upload(report);
            return true;
        }
    }
}