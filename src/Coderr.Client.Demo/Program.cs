using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;

namespace Coderr.Client.Demo
{
  
    class Program
    {
        static void Main()
        {
            var log = CreateLogger();
            log.Info("Starting application");

            var url = new Uri("http://localhost:60473/");
            Err.Configuration.Credentials(url,
                "5a617e0773b94284bef33940e4bc8384",
                "3fab63fb846c4dd289f67b0b3340fefc");
            Err.Configuration.CatchLog4NetExceptions();

            Err.Configuration.AddPartition(x =>
            {
                x.AddPartition("ServerId", Environment.MachineName);
            });

            Err.Configuration.EnvironmentName = "Production";

            try
            {
                log.Debug("Calling hello");
                throw new InvalidDataException("Hello, this is an long example exception");
            }
            catch (Exception ex)
            {
                //log.Error("Opps, failed!", ex);
                //Err.Report(ex, new { userId = 10, ErrTags = "console" });
                var report = Err.GenerateReport(ex, new { userId = 10, ErrTags = "important" });
                Err.UploadReport(report);
                Err.LeaveFeedback(report.ReportId, "jonas@gauffin.com", "This is what I did: NOTHING!");
            }

            return;
            Err.ReportLogicError("User should have been assigned.", errorId: "MainN");

            ThrowImportantError();
            Console.WriteLine("Hello World!");
        }

        private static ILog CreateLogger()
        {
            XmlConfigurator.Configure(
                new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));
            var log = LogManager.GetLogger(typeof(Program));
            Err.Configuration.CatchLog4NetExceptions(Assembly.GetExecutingAssembly());
            return log;
        }

        public static void ThrowImportantError()
        {
            try
            {
                throw new NotSupportedException("Not invented here");
            }
            catch (Exception ex)
            {
                Err.Report(ex, new
                {
                    myData = "hello",
                    ErrTags = "important",
                    Adress =
                        new
                        {
                            City = "Falun"
                        }
                });
            }
        }
    }
}