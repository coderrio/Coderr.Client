using System;
using System.Collections.Generic;
using System.Web.Mvc;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Converters;

namespace OneTrueError.Client.AspNet.Mvc5.Handlers
{
    /// <summary>
    ///     Used to be able to catch MVC specific data when an error occur.
    /// </summary>
    public class OneTrueErrorFilter : IExceptionFilter
    {
        /// <summary>
        ///     Called when an exception occurs.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnException(ExceptionContext filterContext)
        {
            var converter = new ObjectToContextCollectionConverter();

            var items = new List<ContextCollectionDTO>();
            if (filterContext.Result != null && !(filterContext.Result is EmptyResult))
                items.Add(converter.Convert("Result", filterContext.Result));

            Invoke(this, filterContext.Controller.ControllerContext, filterContext.Exception, items);

            filterContext.ExceptionHandled = true;
            filterContext.Result = null;
        }

        internal static ErrorReportDTO Invoke(object source, ControllerContext filterContext, Exception exception,
            IEnumerable<ContextCollectionDTO> extras)
        {
            var context = new AspNetMvcContext(source, exception, filterContext.HttpContext);


            var converter = new ObjectToContextCollectionConverter();

            var items = new List<ContextCollectionDTO>(extras)
            {
                converter.Convert("IsChildAction", filterContext.IsChildAction),
                new ContextCollectionDTO("Controller",
                    new Dictionary<string, string> {{"FullName", filterContext.Controller.GetType().FullName}})
            };

            if (filterContext.Controller != null)
            {
                context.Controller = filterContext.Controller;

                if (filterContext.Controller.TempData != null && filterContext.Controller.TempData.Count > 0)
                    context.TempData = filterContext.Controller.TempData;

                if (filterContext.Controller.ViewBag != null)
                    context.ViewBag = filterContext.Controller.ViewBag;

                if (filterContext.Controller.ViewData != null && filterContext.Controller.ViewData.Count > 0)
                {
                    context.ViewData = filterContext.Controller.ViewData;

                    if (filterContext.Controller.ViewData.ModelState != null)
                        context.ModelState = filterContext.Controller.ViewData.ModelState;
                }
            }

            if (filterContext.ParentActionViewContext != null)
                items.Add(converter.Convert("ParentActionViewContext", filterContext.ParentActionViewContext));

            if (filterContext.RouteData != null)
                context.RouteData = filterContext.RouteData;

            return ErrorHttpModule.ExecutePipeline(context, items.ToArray());
        }
    }
}