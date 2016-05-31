using System.Collections.Generic;
using System.Web.Mvc;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Converters;

namespace OneTrueError.Client.AspNet.Mvc5
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

            var items = new List<ContextCollectionDTO>
            {
                converter.Convert("IsChildAction", filterContext.IsChildAction),
                new ContextCollectionDTO("Controller",
                    new Dictionary<string, string> {{"FullName", filterContext.Controller.GetType().FullName}})
            };

            if (filterContext.HttpContext != null)
            {
                //gave no meaningful keys and a lot of built in stored objects. Filter those out in the future.
                //if (filterContext.HttpContext.Items != null && filterContext.HttpContext.Items.Count > 0)
                //    items.Add(converter.Convert("HttpContext.Items", filterContext.HttpContext.Items));
                if (filterContext.HttpContext.Application != null && filterContext.HttpContext.Application.Count > 0)
                    items.Add(converter.Convert("HttpContext.Application", filterContext.HttpContext.Application));
            }

            if (filterContext.Controller != null)
            {
                if (filterContext.Controller.TempData != null && filterContext.Controller.TempData.Count > 0)
                    items.Add(converter.Convert("TempData", filterContext.Controller.TempData));
                if (filterContext.Controller.ViewBag != null)
                    items.Add(converter.Convert("ViewBag", filterContext.Controller.ViewBag));
                if (filterContext.Controller.ViewData != null && filterContext.Controller.ViewData.Count > 0)
                    items.Add(converter.Convert("ViewData", filterContext.Controller.ViewData));
            }

            if (filterContext.ParentActionViewContext != null)
                items.Add(converter.Convert("ParentActionViewContext", filterContext.ParentActionViewContext));
            if (filterContext.Result != null && !(filterContext.Result is EmptyResult))
                items.Add(converter.Convert("Result", filterContext.Result));
            if (filterContext.RouteData != null)
                items.Add(converter.Convert("RouteData", filterContext.RouteData));

            ErrorHttpModule.ExecutePipeline(this, filterContext.Exception, filterContext.HttpContext, items.ToArray());

            filterContext.ExceptionHandled = true;
            filterContext.Result = null;
        }
    }
}