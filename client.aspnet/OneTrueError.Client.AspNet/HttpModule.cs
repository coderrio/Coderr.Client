using System;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using OneTrueError.Client.AspNet;
using OneTrueError.Client.AspNet.ErrorPages;
using OneTrueError.Client.ContextCollections;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Uploaders;

// This attribute is automatically picked up by ASP.NET/WebPages.

[assembly: PreApplicationStartMethod(typeof(HttpModule), "Register")]

namespace OneTrueError.Client.AspNet
{
    /// <summary>
    ///     HTTP module which OneTrueError uses to be able to catch exceptions.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The module is always loaded automatically (using
    ///         <c>[assembly:PreApplicationStartMethod(typeof(HttpModule), "Register")]</c>).
    ///     </para>
    ///     <para>
    ///         The module do not do anything unless you configure the library.
    ///     </para>
    /// </remarks>
    public class HttpModule : IHttpModule
    {
        static TempData TempData = new TempData();

        /// <summary>
        ///     Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">
        ///     An <see cref="T:System.Web.HttpApplication" /> that provides access to the methods, properties,
        ///     and events common to all application objects within an ASP.NET application
        /// </param>
        public void Init(HttpApplication context)
        {
            context.Error += OnError;
            context.BeginRequest += OnRequest;
        }

        /// <summary>
        ///     Disposes of the resources (other than memory) used by the module that implements
        ///     <see cref="T:System.Web.IHttpModule" />.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        ///     Used to add the module with the help of <c>DynamicModuleUtility.RegisterModule(typeof(HttpModule));</c>.
        /// </summary>
        public static void Register()
        {
            DynamicModuleUtility.RegisterModule(typeof(HttpModule));
        }

        private static string ExtractFirstLine(string message)
        {
            var pos = message.IndexOfAny(new[] { '\r', '\n' });
            return pos == -1 ? message : message.Substring(0, pos);
        }

        private void OnError(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;

            if (!ConfigExtensions.CatchExceptions)
                return;

            var exception = app.Server.GetLastError();
            var httpCodeIdentifier = new HttpCodeIdentifier(app, exception);

            var context = new HttpErrorReporterContext(this, exception)
            {
                HttpContext = app.Context,
                HttpStatusCode = httpCodeIdentifier.HttpCode,
                HttpStatusCodeName = httpCodeIdentifier.HttpCodeName,
                ErrorMessage = ExtractFirstLine(exception.Message)
            };
            var dto = OneTrue.GenerateReport(context);

            var collection =
                dto.ContextCollections.FirstOrDefault(
                    x => x.Name.Equals("ExceptionProperties", StringComparison.OrdinalIgnoreCase));
            if (collection != null)
            {
                if (!collection.Items.ContainsKey("HttpCode"))
                    collection.Items.Add("HttpCode", context.HttpStatusCode.ToString());
            }

            if (OneTrue.Configuration.UserInteraction.AskUserForPermission)
                TempData[dto.ReportId] = dto;
            else
                OneTrue.UploadReport(dto);

            app.Response.StatusCode = context.HttpStatusCode;
            app.Response.StatusDescription = context.ErrorMessage;
            app.Response.TrySkipIisCustomErrors = true;
            app.Response.ContentEncoding = Encoding.UTF8;

            var pageContext = new PageGeneratorContext
            {
                Request = new HttpRequestWrapper(app.Request),
                Response = new HttpResponseWrapper(app.Response),
                ReporterContext = context,
                ReportId = dto.ReportId
            };

            ConfigExtensions.ErrorPageGenerator.Generate(pageContext);
            app.Server.ClearError();
            app.Response.End();
        }

        private void OnRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            if (!app.Request.Url.AbsoluteUri.ToLower().Contains("/onetrueerror/"))
                return;

            var reportId = app.Request.Form["reportId"];
            if (string.IsNullOrEmpty(reportId))
                return;

            var reportDTO = (ErrorReportDTO)TempData[reportId];

            //Not allowed to upload report.
            if (app.Request.Form["Allowed"] != "true" && OneTrue.Configuration.UserInteraction.AskUserForPermission)
            {
                // accessor removes it.
                return;
            }

            if (reportDTO != null)
                OneTrue.UploadReport(reportDTO);

            var info = new UserSuppliedInformation(app.Request.Form["Description"], app.Request.Form["email"]);
            if (!string.IsNullOrEmpty(info.Description) || !string.IsNullOrEmpty(info.EmailAddress))
            {
                OneTrue.LeaveFeedback(reportId, info);
            }

            app.Response.Redirect("~/");
            app.Response.End();
        }
    }
}