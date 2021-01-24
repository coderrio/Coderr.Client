using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Coderr.Client;
using Coderr.Client.Contracts;
using Coderr.Client.Uploaders;

namespace Coderr.Client.Demo
{
  
    class Program
    {
        static void Main()
        {
            //var url = new Uri("http://localhost:60473/");
            //Err.Configuration.Credentials(url,
            //    "5a617e0773b94284bef33940e4bc8384",
            //    "3fab63fb846c4dd289f67b0b3340fefc");

            var url = new Uri("https://report.coderr.io/");
            Err.Configuration.Credentials(url,
                "d0c16767d68943dabe94dc581c45557c",
                "88db0622561d4d1db389524e13901c22");

            Err.Configuration.EnvironmentName = "Production";

            try
            {
                throw new InvalidOperationException("Hello");
            }
            catch (Exception ex)
            {
                //var report = Err.GenerateReport(ex, new { userId = 10, ErrTags = "important" });
                //Err.UploadReport(report);
                //Err.LeaveFeedback(report.ReportId, "jonas@gauffin.com", "This is what I did: NOTHING!");

                Err.Report(ex, new { userId = 10, ErrTags="console" });
            }

            Err.ReportLogicError("User should have been assigned.", errorId: "MainN");

            ThrowImportantError();
            Console.WriteLine("Hello World!");
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