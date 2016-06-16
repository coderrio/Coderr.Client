using System;
using System.Configuration;
using System.Web;
using System.Web.Hosting;

namespace OneTrueError.Client.AspNet.ErrorPages
{
    /// <summary>
    ///     Uses <see cref="VirtualPathProvider" /> to load error pages.
    /// </summary>
    public class VirtualPathProviderBasedGenerator : PageGeneratorBase
    {
        private readonly string _virtualPath;

        public VirtualPathProviderBasedGenerator(string virtualPath)
        {
            if (virtualPath == null) throw new ArgumentNullException("virtualPath");
            _virtualPath = virtualPath;
        }

        protected override void GenerateHtml(PageGeneratorContext context)
        {
            var html = PageBuilder.Build(_virtualPath, context.ReporterContext);
            if (!html.ToLower().Contains("<form")
                &&
                (OneTrue.Configuration.UserInteraction.AskUserForDetails ||
                 OneTrue.Configuration.UserInteraction.AskUserForPermission))
                throw new ConfigurationErrorsException(
                    "You have to have a <form method=\"post\" action=\"http://yourwebsite/onetrueerror/'\"> in your error page if you would like to collect error information.\r\n(Or set OneTrue.Configuration.AskUserForPermission and OneTrue.Configuration.AskUserForDetails to false)");
            if (!html.Contains("$reportId$"))
                throw new ConfigurationErrorsException(
                    "You have to have a '<input type=\"hidden\" name=\"reportId\" value=\"$reportId$\" />' tag in your HTML. The '$reportId$' will be replaced with the correct report id upon errors by this library.");

            html = html.Replace("$reportId$", context.ReportId)
                .Replace("$URL$", VirtualPathUtility.ToAbsolute("~/OneTrueError/Submit/"));

            if (!OneTrue.Configuration.UserInteraction.AskUserForPermission &&
                (OneTrue.Configuration.UserInteraction.AskForEmailAddress ||
                 OneTrue.Configuration.UserInteraction.AskUserForDetails))
            {
                html = html.Replace("$AllowReportUploading$", "checked");
            }
            else
            {
                html = html.Replace("$AllowReportUploading$", "");
            }

            context.SendResponse("text/html", html);
        }
    }
}