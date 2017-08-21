using System;
using System.Collections.Generic;
using OneTrueError.Client.Contracts;

namespace OneTrueError.Client.Processor
{
    /// <summary>
    ///     Purpose of this class is to invoke all callbacks to see if any of them objects to uploading this report.
    /// </summary>
    public class ReportFilterDispatcher
    {
        private readonly List<IReportFilter> _filters = new List<IReportFilter>();

        /// <summary>
        ///     Add a filter to the collection
        /// </summary>
        /// <param name="filter">Filter</param>
        public void Add(IReportFilter filter)
        {
            if (filter == null) throw new ArgumentNullException("filter");
            _filters.Add(filter);
        }

        /// <summary>
        ///     Invoke callbacks
        /// </summary>
        /// <param name="dto">Report to be uploaded.</param>
        /// <returns><c>false</c> if any of the callbacks return <c>false</c>; otherwise <c>true</c></returns>
        /// <remarks>
        ///     <para>
        ///         All callbacks will be invoked, even if one of them returns <c>false</c>.
        ///     </para>
        /// </remarks>
        public bool CanUploadReport(ErrorReportDTO dto)
        {
            var ctx = new ReportFilterContext(dto);
            var canUpload = true;
            foreach (var callback in _filters)
            {
                callback.Invoke(ctx);
                if (!ctx.CanSubmitReport)
                    canUpload = false;
            }

            return canUpload;
        }
    }
}