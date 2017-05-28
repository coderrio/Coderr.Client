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
            OneTrue.Configuration.Credentials(uri,
                "a8bf3b6ad3644068adf809dc12908c3a",
                "86aede6dc3d74f0ab11df444522d780d");


            OneTrue.Configuration.UserInteraction.AskUserForDetails = true;
            OneTrue.Configuration.UserInteraction.AskForEmailAddress = true;
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
