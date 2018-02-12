using System;
using System.Collections.Generic;
using System.Linq;
using codeRR.Client.Config;
using codeRR.Client.ContextCollections;
using codeRR.Client.Contracts;
using codeRR.Client.Reporters;
using Coderr.Client;

namespace codeRR.Client.Processor
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
        public ExceptionProcessor(CoderrConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        ///     Build an report, but do not upload it
        /// </summary>
        /// <param name="exception">caught exception</param>
        /// <returns>Report if it can be processed; otherwise <c>null</c>.</returns>
        /// <remarks>
        ///     <para>
        ///         Will collect context info and generate a report.
        ///     </para>
        /// </remarks>
        public ErrorReportDTO Build(Exception exception)
        {
            return Build(exception, null);
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
        public ErrorReportDTO Build(Exception exception, object contextData)
        {
            if (exception is CoderrClientException)
                return null;
            if (exception.Data.Contains(AlreadyReportedSetting))
                return null;

            var context = new ErrorReporterContext(null, exception);
            if (contextData != null)
                AppendCustomContextData(contextData, context.ContextCollections);

            return Build(context);
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
        /// <returns>
        ///     Report if filter allowed the generated report; otherwise <c>null</c>.
        /// </returns>
        /// <seealso cref="IReportFilter" />
        public ErrorReportDTO Build(IErrorReporterContext context)
        {
            if (context.Exception is CoderrClientException)
                return null;
            if (context.Exception.Data.Contains(AlreadyReportedSetting))
                return null;
            context.Exception.Data.Add(AlreadyReportedSetting, true);

            if (context is IErrorReporterContext2 ctx2)
            {
                ErrorReporterContext.MoveCollectionsInException(context.Exception, ctx2.ContextCollections);
                InvokeFilter(ctx2);
            }

            InvokePartitionCollection(context);

            var contextInfo = _configuration.ContextProviders.Collect(context);
            var reportId = ReportIdGenerator.Generate(context.Exception);
            AddAddemblyVersion(contextInfo);
            var report = new ErrorReportDTO(reportId, new ExceptionDTO(context.Exception), contextInfo.ToArray());
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
            var report = Build(exception);
            Process(report);
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
        /// <returns>
        ///     Report if filter allowed the generated report; otherwise <c>null</c>.
        /// </returns>
        /// <seealso cref="IReportFilter" />
        public void Process(IErrorReporterContext context)
        {
            var report = Build(context);
            Process(report);
        }


        /// <summary>
        ///     Process exception.
        /// </summary>
        /// <param name="exception">caught exception</param>
        /// <param name="contextData">Context data</param>
        /// <remarks>
        ///     <para>
        ///         Will collect context info, generate a report, go through filters and finally upload it.
        ///     </para>
        /// </remarks>
        public void Process(Exception exception, object contextData)
        {
            var report = Build(exception, contextData);
            Process(report);
        }

        protected void Process(ErrorReportDTO report)
        {
            if (report == null)
                return;

            var canUpload = _configuration.FilterCollection.CanUploadReport(report);
            if (!canUpload)
                return;

            _configuration.Uploaders.Upload(report);
        }

        internal void AddAddemblyVersion(IList<ContextCollectionDTO> contextInfo)
        {
            if (_configuration.ApplicationVersion == null)
                return;

            var items = new Dictionary<string, string>
                {
                    {"AppAssemblyVersion", _configuration.ApplicationVersion}
                };

            var col = new ContextCollectionDTO("AppVersion", items);
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
                contextInfo.Add(col);
            }
        }

        private void InvokeFilter(IErrorReporterContext2 context)
        {
            Err.Configuration.ExceptionPreProcessor?.Invoke(context);
        }

        private void InvokePartitionCollection(IErrorReporterContext context)
        {
            var ctx2 = context as IErrorReporterContext2;
            if (ctx2 == null)
                return;

            var col = new ErrPartitionContextCollection();
            ctx2.ContextCollections.Add(col);

            var partitionContext = new PartitionContext(col, ctx2);
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
    }
}