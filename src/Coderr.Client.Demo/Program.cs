using System;
using System.Data;
using System.IO;
using System.Net;

namespace codeRR.Client.Demo
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

            var url = new Uri("http://localhost:51970/");
            Err.Configuration.Credentials(url,
                "fda370d3a4444964b52d785a9b26fe21", 
                "c3f786f9205c4572b5bbe4cfb81ba4f0");
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
                    var webEx = (WebException) e.InnerException;
                    if (webEx != null)
                    {
                        var reader = new StreamReader(webEx.Response.GetResponseStream());
                        var serverError = reader.ReadToEnd();
                    }
                }
            }
        }
    }
}