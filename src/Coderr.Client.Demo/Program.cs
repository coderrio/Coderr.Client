using System;
using System.Data;
using System.IO;
using System.Net;
using System.Resources;
using System.Text;
using codeRR.Client.ContextCollections;
using codeRR.Client.ContextProviders;

namespace codeRR.Client.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialization
            var uri = new Uri("http://localhost/coderr/");
            Err.Configuration.Credentials(uri,
										  "yourOwnAppKey",
										  "yourOwnSharedSecret");

            try
            {
                throw new InvalidDataException("corrupt data");
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
    }
}
