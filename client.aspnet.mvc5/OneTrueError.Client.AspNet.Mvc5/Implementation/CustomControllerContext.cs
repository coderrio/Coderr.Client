using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using OneTrueError.Client.Contracts;

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
            _routeData.Values["action"] = HttpCodeName;

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
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("HandleErrorInfo"))
                {
                    requestContext.HttpContext.Response.Write(@"<html>
<head>
<title>Configuration error</title>
</head>
<body>
<h1>Configuration error</h1>
<p>The default ASP.NET MVC error view <code>Shared\Error.cshtml</code> uses <code>HandleErrorInfo</code> as a view model while OneTrueError expects <code>OneTrueViewModel</code>.</p>
<p>You have three options:</p>
<ol>
<li>Change view model in it: <code>@model OneTrueError.Client.AspNet.Mvc5.OneTrueViewModel</code></li>
<li>Remove the view <code>Shared\Errors.cshtml</code> to get OneTrueErrors built in error pages.</li>
<li>Disable OneTrueErrors error page handling, remove <code>OneTrue.Configuration.DisplayErrorPages();</code> from global.asax.</li>
</ol>
<h1>Actual error</h1>
<pre>" + model.Exception + "</pre></body></html>");
                    requestContext.HttpContext.Response.ContentType = "text/html";
                    requestContext.HttpContext.Response.End();
                }
                OneTrue.Report(ex, model);
            }
            catch (Exception ex)
            {
                OneTrue.Report(ex, model);
            }

            return true;
        }
    }
}