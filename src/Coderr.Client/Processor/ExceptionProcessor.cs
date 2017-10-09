using System;
using System.Collections.Generic;
using System.Linq;
using codeRR.Client.Config;
using codeRR.Client.Contracts;
using codeRR.Client.Reporters;

namespace codeRR.Client.Processor
{
    /// <summary>
    ///     Will proccess the exception to generate context info and then upload it to the server.
    /// </summary>
    internal class ExceptionProcessor
    {
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
        /// <remarks>
        ///     <para>
        ///         Will collect context info and generate a report.
        ///     </para>
        /// </remarks>
        public ErrorReportDTO Build(Exception exception)
        {
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
            var context = new ErrorReporterContext(null, exception);
            var contextInfo = _configuration.ContextProviders.Collect(context);
            var reportId = ReportIdGenerator.Generate(exception);
            var report = new ErrorReportDTO(reportId, new ExceptionDTO(exception), contextInfo.ToArray());
            var canUpload = _configuration.FilterCollection.CanUploadReport(report);
            if (!canUpload)
                return;
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
            var contextInfo = _configuration.ContextProviders.Collect(context);
            var reportId = ReportIdGenerator.Generate(context.Exception);
            var report = new ErrorReportDTO(reportId, new ExceptionDTO(context.Exception), contextInfo.ToArray());
            var canUpload = _configuration.FilterCollection.CanUploadReport(report);
            if (!canUpload)
                return null;
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
            var context = new ErrorReporterContext(null, exception);
            var contextInfo = _configuration.ContextProviders.Collect(context);
            AppendCustomContextData(contextData, contextInfo);

            var reportId = ReportIdGenerator.Generate(exception);
            var report = new ErrorReportDTO(reportId, new ExceptionDTO(exception), contextInfo.ToArray());
            var canUpload = _configuration.FilterCollection.CanUploadReport(report);
            if (!canUpload)
                return;
            _configuration.Uploaders.Upload(report);
        }

        private static void AppendCustomContextData(object contextData, IList<ContextCollectionDTO> contextInfo)
        {
            var dtos = contextData as IEnumerable<ContextCollectionDTO>;
            if (dtos != null)
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
    }
}