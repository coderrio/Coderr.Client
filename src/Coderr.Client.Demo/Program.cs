using System;
using System.Data;
using System.IO;
using System.Net;
using System.Runtime.Remoting.Messaging;

namespace Coderr.Client.Demo
{
    internal class Program
    {
        private static void CallToThrow()
        {
            //throw new InvalidOperationException("Failed to execute something basic!");
            GoDeeper();
        }

        private static void GoDeeper()
        {
            //throw new InvalidOperationException("This is deep!");
            throw new SyntaxErrorException("no good syntax");
        }

        private static void Main(string[] args)
        {
            // Initialization
            //var uri = new Uri("http://localhost/coderr/");
            //Err.Configuration.Credentials(uri,
            //    "yourOwnAppKey",
            //    "yourOwnSharedSecret");

            //var url = new Uri("http://localhost:50473/");
            var url = new Uri("https://report.coderr.io/");
            Err.Configuration.Credentials(url,
                "d0c16767d68943dabe94dc581c45557c",
                "88db0622561d4d1db389524e13901c22");

            try
            {
                //throw new InvalidDataException("Hello world");
                throw new NotSupportedException("OSS!");
            }
            catch (Exception ex)
            {
                try
                {
                    Err.Report(ex);
                }
                catch (Exception e)
                {
                    var webEx = (WebException)e.InnerException;
                    if (webEx != null)
                    {
                        var reader = new StreamReader(webEx.Response.GetResponseStream());
                        var serverError = reader.ReadToEnd();
                    }
                }
            }
        }

        public static void DemoLogicalErrors()
        {
            Err.ReportLogicError("User skipped step Starting", new { user = "Arne" }, "SkipStepStarting");
            Err.ReportLogicError("This failed", new { id = 10 }, "MixMax2");
            Err.ReportLogicError("Network dropped packets",
                new
                {
                    destination = "chat.host.com",
                    packet = new { FromNick = "jonas", ToNick = "arne", message = "Hello world" }
                });

        }
    }
}