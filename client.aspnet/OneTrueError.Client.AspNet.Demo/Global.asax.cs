using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using OneTrueError.Client.AspNet.ErrorPages;

namespace OneTrueError.Client.AspNet.Demo
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            var url = new Uri("http://localhost:50473/");
            OneTrue.Configuration.Credentials(url, "13d82df603a845c7a27164c4fec19dd6", "6f0a0a7fac6d42caa7cc47bb34a6520b");
            OneTrue.Configuration.SetErrorPageGenerator(new VirtualPathProviderBasedGenerator("~/Errors/"));
            OneTrue.Configuration.CatchAspNetExceptions();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}