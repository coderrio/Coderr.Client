using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;

[assembly: XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]

namespace OneTrueError.Client.Log4net.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("log4net.config"));

            var oneTrueServiceUri = new Uri("http://localhost/your/own/installation");
            OneTrue.Configure(oneTrueServiceUri,
                "99948f8a-545d-491b-8dff-4fef6c250110",
                "c742290e-a80d-4ecb-8065-338780b61b2a");

            // injects into the log4net pipeline
            OneTrue.Configuration.CatchLog4NetExceptions();

            Console.ReadLine();
        }
    }
}
