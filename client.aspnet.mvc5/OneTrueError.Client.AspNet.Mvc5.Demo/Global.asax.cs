using System;
using System.Web.Mvc;
using System.Web.Routing;
using OneTrueError.Client.Uploaders;

namespace OneTrueError.Client.AspNet.Mvc5.Demo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var uri = new Uri("http://localhost:50473/");
            //OneTrue.Configuration.DisplayErrorPages();
            OneTrue.Configuration.Credentials(uri,
                "13d82df603a845c7a27164c4fec19dd6",
                "6f0a0a7fac6d42caa7cc47bb34a6520b");

            OneTrue.Configuration.CatchMvcExceptions();
            OneTrue.Configuration.DisplayErrorPages();
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }


        private void OnError(object sender, UploadReportFailedEventArgs e)
        {
            
        }
    }
}
