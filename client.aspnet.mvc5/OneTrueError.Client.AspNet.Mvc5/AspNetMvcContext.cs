using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OneTrueError.Client.AspNet.Mvc5
{
    /// <summary>
    ///     Bring MVC specific sources
    /// </summary>
    public class AspNetMvcContext : AspNetContext
    {
        /// <summary>
        ///     Creates a new instance of <see cref="AspNetMvcContext" />.
        /// </summary>
        /// <param name="reporter">object triggering the collection</param>
        /// <param name="exception">caught exception</param>
        /// <param name="httpContext">context that the exception was thrown on</param>
        /// <exception cref="ArgumentNullException">reporter;exception</exception>
        public AspNetMvcContext(object reporter, Exception exception, HttpContextBase httpContext) : base(reporter,
            exception, httpContext)
        {
        }


        /// <summary>
        ///     Controller (if a controller was involved in the "incident")
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         May or may not be specified depending on where the collection process was started from. Not everything can be
        ///         collected when using <c>OneTrue.Report</c> instead of <c>controller.ReportException</c>.
        ///     </para>
        /// </remarks>
        public ControllerBase Controller { get; set; }

        /// <summary>
        ///     ModelState if any
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         May or may not be specified depending on where the collection process was started from. Not everything can be
        ///         collected when using <c>OneTrue.Report</c> instead of <c>controller.ReportException</c>.
        ///     </para>
        /// </remarks>
        public ModelStateDictionary ModelState { get; set; }

        /// <summary>
        ///     RouteData if any
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         May or may not be specified depending on where the collection process was started from. Not everything can be
        ///         collected when using <c>OneTrue.Report</c> instead of <c>controller.ReportException</c>.
        ///     </para>
        /// </remarks>
        public RouteData RouteData { get; set; }

        /// <summary>
        ///     Specified if we find it somewhere (like in a controller). Might be empty though..
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         May or may not be specified depending on where the collection process was started from. Not everything can be
        ///         collected when using <c>OneTrue.Report</c> instead of <c>controller.ReportException</c>.
        ///     </para>
        /// </remarks>
        public TempDataDictionary TempData { get; set; }

        /// <summary>
        ///     ViewBag if any
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         May or may not be specified depending on where the collection process was started from. Not everything can be
        ///         collected when using <c>OneTrue.Report</c> instead of <c>controller.ReportException</c>.
        ///     </para>
        /// </remarks>
        public object ViewBag { get; set; }

        /// <summary>
        ///     ViewData if any
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         May or may not be specified depending on where the collection process was started from. Not everything can be
        ///         collected when using <c>OneTrue.Report</c> instead of <c>controller.ReportException</c>.
        ///     </para>
        /// </remarks>
        public ViewDataDictionary ViewData { get; set; }
    }
}