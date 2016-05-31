using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OneTrueError.Client.AspNet.Mvc5.Implementation
{
    internal class CustomControllerContext
    {
        private readonly string _errorId;
        private readonly Exception _exception;
        private readonly RouteData _routeData = new RouteData();

        public CustomControllerContext(Exception exception, string errorId)
        {
            _exception = exception;
            _errorId = errorId;
            _routeData.Values.Add("exception", exception);
            _routeData.Values.Add("errorId", errorId);
            _routeData.Values.Add("controller", "Error");
            _routeData.Values.Add("action", "InternalServerError");

            HttpCode = 500;
            HttpCodeName = "InternalServerError";
            FindHttpCode(exception);
        }

        public int HttpCode { get; set; }
        public string HttpCodeName { get; set; }

        public static bool NoUserController { get; set; }

        public bool NoUserViews { get; set; }

        public void Execute(HttpErrorReporterContext context)
        {
            try
            {
                if (ExecuteUserController(context.HttpContext))
                    return;
                if (ExecuteUserView(context.HttpContext))
                    return;

                BuiltInViewRender.Render(context);
            }
            catch (Exception ex)
            {
                OneTrue.Report(ex, new {context.HttpStatusCodeName, context.HttpStatusCode});
            }
        }


        protected string GetViewName(ControllerContext context, params string[] names)
        {
            foreach (var name in names)
            {
                var result = ViewEngines.Engines.FindView(context, name, null);
                if (result.View != null)
                    return name;
            }
            return null;
        }

        private bool ExecuteUserController(HttpContextBase httpContext)
        {
            if (NoUserController)
                return false;

            var model = new OneTrueViewModel
            {
                ErrorId = _errorId,
                Exception = _exception,
                HttpStatusCode = HttpCode,
                HttpStatusCodeName = HttpCodeName
            };
            _routeData.DataTokens["OneTrueModel"] = model;

            try
            {
                var controllerFactory = ControllerBuilder.Current.GetControllerFactory();
                var controller = controllerFactory.CreateController(new RequestContext(httpContext, _routeData), "Error");
                controller.Execute(new RequestContext(httpContext, _routeData));
                return true;
            }
            catch (HttpException controllerNotFoundException)
            {
                if (controllerNotFoundException.StackTrace.Contains("HandleUnknownAction"))
                {
                    try
                    {
                        var controllerFactory = ControllerBuilder.Current.GetControllerFactory();
                        var controller = controllerFactory.CreateController(
                            new RequestContext(httpContext, _routeData), "Error");
                        _routeData.Values["Action"] = "Index";
                        controller.Execute(new RequestContext(httpContext, _routeData));
                        return true;
                    }
                    catch
                    {
                    }
                }
                NoUserController = true;
                return false;
            }
        }

        private bool ExecuteUserView(HttpContextBase httpContext)
        {
            if (NoUserViews)
                return false;

            var controller = new OneTrueErrorController();
            var requestContext = new RequestContext(httpContext, _routeData);

            var model = new OneTrueViewModel
            {
                ErrorId = _errorId,
                Exception = _exception,
                HttpStatusCode = HttpCode,
                HttpStatusCodeName = HttpCodeName
            };

            var ctx = new ControllerContext(requestContext, controller);
            ctx.Controller.ViewData.Model = model;
            var viewName = GetViewName(ctx, string.Format("~/Views/Error/{0}.cshtml", HttpCodeName),
                "~/Views/Error/Error.cshtml",
                "~/Views/Error/General.cshtml",
                "~/Views/Shared/Error.cshtml");
            if (viewName == null)
            {
                NoUserViews = true;
                return false;
            }

            var result = new ViewResult
            {
                ViewName = viewName,
                MasterName = null,
                ViewData = new ViewDataDictionary<OneTrueViewModel>(model)
            };
            try
            {
                result.ExecuteResult(ctx);
            }
            catch (Exception ex)
            {
                OneTrue.Report(ex, model);
            }

            return true;
        }

        private void FindHttpCode(Exception exception)
        {
            _routeData.Values["action"] = HttpCodeName;
            _routeData.Values.Add("httpStatusCode", HttpCode);
        }
    }
}