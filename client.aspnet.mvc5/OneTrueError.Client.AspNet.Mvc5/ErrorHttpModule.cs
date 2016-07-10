using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using OneTrueError.Client.AspNet.Mvc5;
using OneTrueError.Client.AspNet.Mvc5.Implementation;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Uploaders;

[assembly: PreApplicationStartMethod(typeof(ErrorHttpModule), "AddModuleToAspNet")]

namespace OneTrueError.Client.AspNet.Mvc5
{
    /// <summary>
    ///     Module that will catch unhandled exceptions
    /// </summary>
    [CompilerGenerated]
    public class ErrorHttpModule : IHttpModule
    {
        private static readonly TempData TempData = new TempData();

        /// <summary>
        ///     Module is activated and may intercept exceptions
        /// </summary>
        public static bool Activated { get; set; }

        /// <summary>
        ///     Use our built in error pages.
        /// </summary>
        public static bool DisplayErrorPage { get; set; }

        /// <summary>
        ///     Application that inited this module.
        /// </summary>
        protected HttpApplication Application { get; set; }

        /// <summary>
        ///     Initialize the module to capture new exceptions
        /// </summary>
        /// <param name="context">The http application instance (ASP.NET)</param>
        public void Init(HttpApplication context)
        {
            context.Error += OnError;
            Application = context;
        }

        /// <summary>
        ///     Disposes of the resources (other than memory) used by the module that implements
        ///     <see cref="T:System.Web.IHttpModule" />.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        ///     Module is activated and may do it's thing.
        /// </summary>
        public static void Activate()
        {
            Activated = true;
        }

        /// <summary>
        ///     Add our module to the ASP.NET pipeline.
        /// </summary>
        public static void AddModuleToAspNet()
        {
            DynamicModuleUtility.RegisterModule(typeof(ErrorHttpModule));
        }

        /// <summary>
        ///     Do the OneTrueError collection pipeline.
        /// </summary>
        /// <param name="source">Thingy that detected the exception</param>
        /// <param name="exception">Exception that was caught</param>
        /// <param name="httpContext">Context currently executing</param>
        /// <param name="contextCollections">Extras</param>
        public static void ExecutePipeline(object source, Exception exception, HttpContextBase httpContext,
            params ContextCollectionDTO[] contextCollections)
        {
            if (
                httpContext.Request.Url.AbsolutePath.IndexOf("/onetrueerror/submit", StringComparison.OrdinalIgnoreCase) !=
                -1)
            {
                ProcessSubmit(httpContext);
                httpContext.Response.Redirect("~/");
                return;
            }

            HttpApplication application = null;
            var module = source as ErrorHttpModule;
            if (module != null && module.Application != null)
            {
                application = module.Application;
                if (DisplayErrorPage)
                    module.Application.Response.Clear();
            }

            var httpCodeIdentifier = new HttpCodeIdentifier(application, exception);
            var reportingContext = new AspNetContext(application ?? source, exception, httpContext);
            var report = OneTrue.GenerateReport(reportingContext);
            if (contextCollections.Any())
            {
                var newList = new List<ContextCollectionDTO>(report.ContextCollections);
                newList.AddRange(contextCollections);
                report.ContextCollections = newList.ToArray();
            }

            // Add http code
            var exceptionCollection = report.ContextCollections.FirstOrDefault(x => x.Name == "ExceptionProperties");
            if (exceptionCollection != null)
            {
                if (!exceptionCollection.Properties.ContainsKey("HttpCode"))
                    exceptionCollection.Properties["HttpCode"] = httpCodeIdentifier.HttpCode.ToString();
            }

            if (!DisplayErrorPage || !OneTrue.Configuration.UserInteraction.AskUserForPermission)
            {
                OneTrue.UploadReport(report);
                if (!DisplayErrorPage)
                    return;
            }
            else
                TempData[report.ReportId] = report;

            var handler = new CustomControllerContext(exception, report.ReportId)
            {
                HttpCode = httpCodeIdentifier.HttpCode,
                HttpCodeName = httpCodeIdentifier.HttpCodeName
            };

            var ctx = new HttpErrorReporterContext(source, exception, httpContext)
            {
                ErrorId = report.ReportId,
                HttpStatusCode = handler.HttpCode,
                HttpStatusCodeName = handler.HttpCodeName
            };

            httpContext.Response.StatusCode = handler.HttpCode;
            httpContext.Response.StatusDescription = handler.HttpCodeName;
            handler.Execute(ctx);
        }

        private void OnError(object sender, EventArgs e)
        {
            if (!Activated)
                return;


            var app = (HttpApplication)sender;
            var exception = app.Server.GetLastError();

            ExecutePipeline(this, exception, new HttpContextWrapper(app.Context));

            if (DisplayErrorPage)
                app.Response.End();
        }

        private static void ProcessSubmit(HttpContextBase httpContext)
        {
            var reportId = httpContext.Request.Form["ReportId"];
            if (string.IsNullOrEmpty(reportId))
                return;

            if (OneTrue.Configuration.UserInteraction.AskUserForPermission)
            {
                if (httpContext.Request.Form["Allowed"] != "true")
                    return;

                // report have been sent in the previous HTTP post for other cases.
                var report = TempData[reportId];
                if (report != null)
                    OneTrue.Configuration.Uploaders.Upload((ErrorReportDTO)report);
            }


            var description = httpContext.Request.Form["Description"];
            var email = httpContext.Request.Form["Email"];
            if (!string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(description))
            {
                OneTrue.Configuration.Uploaders.Upload(new FeedbackDTO
                {
                    Description = description,
                    EmailAddress = email,
                    ReportId = reportId
                });
            }
        }
    }
}