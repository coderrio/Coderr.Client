using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.UI;

namespace OneTrueError.Client.AspNet
{
    /// <summary>
    ///     Helper function for building an OneTrueError error page.
    /// </summary>
    public class PageBuilder
    {
        /// <summary>
        ///     Build an ASPX or HTML file to be used as our error page.
        /// </summary>
        /// <param name="virtualPath">path to directory where error pages are located</param>
        /// <param name="context">Context for OneTrueError</param>
        /// <returns>Complete string</returns>
        public static string Build(string virtualPath, HttpErrorReporterContext context)
        {
            var url =
                new Uri(string.Format("{0}://{1}{2}", HttpContext.Current.Request.Url.Scheme,
                    HttpContext.Current.Request.Url.Authority,
                    HttpContext.Current.Request.Url.AbsolutePath));

            if (!virtualPath.EndsWith("/"))
                virtualPath += "/";

            var locations = new[]
            {
                virtualPath + context.HttpStatusCodeName + ".aspx",
                virtualPath + context.HttpStatusCodeName + ".html",
                virtualPath + "error.aspx",
                virtualPath + "error.html"
            };

            var virtualFilePath = "";
            foreach (var location in locations)
            {
                if (HostingEnvironment.VirtualPathProvider.FileExists(location))
                {
                    virtualFilePath = location;
                    break;
                }
            }

            if (virtualFilePath == "")
                throw new ConfigurationErrorsException("Failed to find an error page in " + virtualPath);

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            if (virtualFilePath.EndsWith(".aspx"))
            {
                var request = new HttpRequest(null, url.ToString(), "");
                var response = new HttpResponse(sw);
                var httpContext = new HttpContext(request, response);
                httpContext.Items["ErrorReportContext"] = context;
                httpContext.Items["Exception"] = context.Exception;

                var pageType = BuildManager.GetCompiledType(virtualFilePath);
                var pageObj = Activator.CreateInstance(pageType);

                var exceptionProperty = pageObj.GetType().GetProperty("Exception");
                if (exceptionProperty!=null && exceptionProperty.PropertyType == typeof(Exception))
                    exceptionProperty.SetValue(pageObj, context.Exception, null);

                var contextProperty = pageObj.GetType().GetProperty("ExceptionContext");
                if (contextProperty != null && contextProperty.PropertyType == typeof(HttpErrorReporterContext))
                    contextProperty.SetValue(pageObj, context, null);


                var executeMethod = pageType.GetMethod("Execute", new Type[0]);

                // support for web pages in the future?
                if (executeMethod != null && false)
                {
                    executeMethod.Invoke(pageObj, new object[0]);
                }
                var page = (Page) pageObj;
                page.ProcessRequest(httpContext);
            }
            else if (virtualFilePath.EndsWith(".html"))
            {
                //VirtualPathUtility.ToAbsolute is really required if you do not want ASP.NET to complain about the path.
                using (
                    var stream =
                        VirtualPathProvider.OpenFile(
                            VirtualPathUtility.ToAbsolute(virtualFilePath)))
                {
                    var reader = new StreamReader(stream);
                    return reader.ReadToEnd();
                }
            }

            return sb.ToString();
        }
    }
}