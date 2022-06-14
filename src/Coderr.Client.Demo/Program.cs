using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Coderr.Client.Contracts;
using log4net;
using log4net.Config;

namespace Coderr.Client.Demo
{

    class Program
    {
        static void Main()
        {
            // This is required to activate Coderr +
            // the nuget package "Coderr.Client"
            var url = new Uri("http://report.coderr.io");

            // The appKey and sharedSecret can be found in Coderr
            Err.Configuration.Credentials(url,
                "28b83bdb5e474213922d8fcded2c1d47",
                "351b658e63974ab7bca125b7addd1185");

            // Attach the 100 latest log entries 
            // to every error report from log4net
            //
            // Requires the nuget package "Coder.Client.log4net"
            Err.Configuration.CatchLog4NetExceptions();
            var updatedUser = new string { };

            try
            {
                throw new InvalidOperationException("oops, something failed");
            }
            catch (Exception ex)
            {
                // This is how you manually report errors
                // You can attach any kind of information
                //
                // Escalate the error directly to "important",
                // which means that you get an notification and it's
                // automatically added to your backlog
                Err.Report(ex, new { User = updatedUser, ErrTags = "important,backend" });
            }


            var log = CreateLogger();
            log.Info("Starting application");

            //var url = new Uri("http://localhost:60473/");
            //Err.Configuration.Credentials(url,
            //    "5a617e0773b94284bef33940e4bc8384",
            //    "3fab63fb846c4dd289f67b0b3340fefc");
            url = new Uri("http://adhost:81/");
            Err.Configuration.Credentials(url,
                "28b83bdb5e474213922d8fcded2c1d47",
                "351b658e63974ab7bca125b7addd1185");
            Err.Configuration.CatchLog4NetExceptions();

            try
            {
                Err.Configuration.AddPartition(x => { x.AddPartition("ServerId", Environment.MachineName); });

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
                    if (report.LogEntries != null)
                    {
                        var entries = new List<LogEntryDto>(report.LogEntries)
                        {
                            new LogEntryDto(DateTime.UtcNow, 1, "This is an test")
                        };
                        report.LogEntries = entries.ToArray();
                    }
                    else
                    {
                        report.LogEntries = new LogEntryDto[]
                        {
                            new LogEntryDto(DateTime.UtcNow, 1, "This is an test")
                        };

                    }

                    Err.UploadReport(report);
                    Console.WriteLine("Report sent");
                    Err.LeaveFeedback(report.ReportId, "jonas@gauffin.com", "This is what I did: NOTHING!");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("Press ENTER to qiut");
            Console.ReadLine();
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