using System;
using Microsoft.Owin;
using OneTrueError.Client;
using Owin;

[assembly: OwinStartup(typeof(DemoApp.Startup))]
namespace DemoApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            ConfigureAuth(app);
        }
    }
}
