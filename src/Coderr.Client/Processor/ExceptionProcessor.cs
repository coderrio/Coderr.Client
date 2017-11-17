using System;
using System.Collections.Generic;
using System.Linq;
using codeRR.Client.Config;
using codeRR.Client.Contracts;
using codeRR.Client.Reporters;
using Coderr.Client;

namespace codeRR.Client.Processor
{
    /// <summary>
    ///     Will process the exception to generate context info and then upload it to the server.
    /// </summary>
    internal class ExceptionProcessor
    {
        private const string AlreadyReportedSetting = "ErrSetting.Reported";
        private readonly CoderrConfiguration _configuration;

        /// <summary>
        ///     Creates a new instance of <see cref="ExceptionProcessor" />.
        /// </summary>
        /// <param name="configuration">Current configuration.</param>
        public ExceptionProcessor(CoderrConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            _configuration = configuration;
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
            if (exception.Data.Contains(AlreadyReportedSetting))
                return null;

            var context = new ErrorReporterContext(null, exception);
            var contextInfo = _configuration.ContextProviders.Collect(context);
            var reportId = ReportIdGenerator.Generate(exception);
            return new ErrorReportDTO(reportId, new ExceptionDTO(exception), contextInfo.ToArray());
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
            if (exception.Data.Contains(AlreadyReportedSetting))
                return null;

            var context = new ErrorReporterContext(null, exception);
            var contextInfo = _configuration.ContextProviders.Collect(context);
            AppendCustomContextData(contextData, contextInfo);
            var reportId = ReportIdGenerator.Generate(exception);
            return new ErrorReportDTO(reportId, new ExceptionDTO(exception), contextInfo.ToArray());
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
            if (exception is CoderrClientException)
                return;
            if (exception.Data.Contains(AlreadyReportedSetting))
                return;

            var context = new ErrorReporterContext(null, exception);
            var contextInfo = _configuration.ContextProviders.Collect(context);
            var reportId = ReportIdGenerator.Generate(exception);
            var report = new ErrorReportDTO(reportId, new ExceptionDTO(exception), contextInfo.ToArray());
            var canUpload = _configuration.FilterCollection.CanUploadReport(report);
            if (!canUpload)
                return;

            exception.Data.Add(AlreadyReportedSetting, true);
            _configuration.Uploaders.Upload(report);
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
        public ErrorReportDTO Process(IErrorReporterContext context)
        {
            if (context.Exception is CoderrClientException)
                return null;
            if (context.Exception.Data.Contains(AlreadyReportedSetting))
                return null;
            if (context is IErrorReporterContext2 ctx2)
            {
                ErrorReporterContext.MoveCollectionsInException(context.Exception, ctx2.ContextCollections);
                InvokeFilter(ctx2);
            }

            var contextInfo = _configuration.ContextProviders.Collect(context);
            var reportId = ReportIdGenerator.Generate(context.Exception);
            var report = new ErrorReportDTO(reportId, new ExceptionDTO(context.Exception), contextInfo.ToArray());
            var canUpload = _configuration.FilterCollection.CanUploadReport(report);
            if (!canUpload)
                return null;

            context.Exception.Data.Add(AlreadyReportedSetting, true);
            return report;
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
            if (exception is CoderrClientException)
                return;
            if (exception.Data.Contains(AlreadyReportedSetting))
                return;

            var context = new ErrorReporterContext(null, exception);
            var contextInfo = _configuration.ContextProviders.Collect(context);
            AppendCustomContextData(contextData, contextInfo);

            var reportId = ReportIdGenerator.Generate(exception);
            var report = new ErrorReportDTO(reportId, new ExceptionDTO(exception), contextInfo.ToArray());
            var canUpload = _configuration.FilterCollection.CanUploadReport(report);
            if (!canUpload)
                return;

            exception.Data.Add(AlreadyReportedSetting, true);
            _configuration.Uploaders.Upload(report);
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
    }
}