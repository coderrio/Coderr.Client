using System;
using Coderr.Client.Contracts;
using Coderr.Client.Reporters;

namespace Coderr.Client.Processor
{
    /// <summary>
    /// Runs the codeRR client pipeline
    /// </summary>
    public interface IExceptionProcessor
    {
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
        ErrorReportDTO Build(Exception exception);

        /// <summary>
        ///     Build an report, but do not upload it
        /// </summary>
        /// <param name="context">Error reporter context</param>
        /// <returns>Report if it can be processed; otherwise <c>null</c>.</returns>
        /// <remarks>
        ///     <para>
        ///         Will collect context info and generate a report.
        ///     </para>
        /// </remarks>
        ErrorReportDTO Build(IErrorReporterContext context);

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
        ErrorReportDTO Build(Exception exception, object contextData);

        /// <summary>
        ///     Process exception.
        /// </summary>
        /// <param name="exception">caught exception</param>
        /// <remarks>
        ///     <para>
        ///         Will collect context info, generate a report, go through filters and finally upload it.
        ///     </para>
        /// </remarks>
        void Process(Exception exception);

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
        void Process(IErrorReporterContext context);

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
        void Process(Exception exception, object contextData);
    }
}