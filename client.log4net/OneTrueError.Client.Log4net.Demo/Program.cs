using System;
using System.IO;
using log4net;
using log4net.Config;

namespace OneTrueError.Client.Log4net.Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("log4net.config"));

            var url = new Uri("http://localhost:50473/");
            OneTrue.Configuration.Credentials(url,
                "13d82df603a845c7a27164c4fec19dd6",
                "6f0a0a7fac6d42caa7cc47bb34a6520b");

            // injects into the log4net pipeline
            OneTrue.Configuration.CatchLog4NetExceptions();

            var log = LogManager.GetLogger(typeof(Program));
            log.Info("Hello word");

            var service = new SomeService();
            service.DoSomeStuff();

            Console.WriteLine("Exception have been logged.");
            Console.ReadLine();
        }
    }
}