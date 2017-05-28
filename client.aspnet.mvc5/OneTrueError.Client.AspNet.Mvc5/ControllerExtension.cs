using System;
using System.Collections.Generic;
using System.Web.Mvc;
using OneTrueError.Client.AspNet.Mvc5.Handlers;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Converters;

// ReSharper disable CheckNamespace

namespace OneTrueError.Client
{
    /// <summary>
    ///     To be able to pickup ASP.NET MVC context collections
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        ///     Report exception through OneTrueError
        /// </summary>
        /// <param name="controller">controller used to report exception (used to be able to collect context data)</param>
        /// <param name="exception">exception to report</param>
        /// <param name="contextData">extra collections</param>
        /// <returns>sent report (can be used for instance for <c>OneTrue.LeaveFeedback</c>)</returns>
        public static ErrorReportDTO ReportException(this ControllerBase controller, Exception exception,
            IEnumerable<ContextCollectionDTO> contextData)
        {
            return OneTrueErrorFilter.Invoke(controller, controller.ControllerContext, exception, contextData);
        }

        /// <summary>
        ///     Report exception through OneTrueError
        /// </summary>
        /// <param name="controller">controller used to report exception (used to be able to collect context data)</param>
        /// <param name="exception">exception to report</param>
        /// <param name="contextData">extra context data</param>
        /// <returns>sent report (can be used for instance for <c>OneTrue.LeaveFeedback</c>)</returns>
        public static ErrorReportDTO ReportException(this ControllerBase controller, Exception exception,
            object contextData)
        {
            var converter = new ObjectToContextCollectionConverter();
            var collection = converter.Convert(contextData);
            return OneTrueErrorFilter.Invoke(controller, controller.ControllerContext, exception, new[] {collection});
        }

        /// <summary>
        ///     Report exception through OneTrueError
        /// </summary>
        /// <param name="controller">controller used to report exception (used to be able to collect context data)</param>
        /// <param name="exception">exception to report</param>
        /// <returns>sent report (can be used for instance for <c>OneTrue.LeaveFeedback</c>)</returns>
        public static ErrorReportDTO ReportException(this ControllerBase controller, Exception exception)
        {
            return OneTrueErrorFilter.Invoke(controller, controller.ControllerContext, exception,
                new ContextCollectionDTO[0]);
        }
    }
}