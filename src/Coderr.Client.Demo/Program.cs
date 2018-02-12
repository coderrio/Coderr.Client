using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using codeRR.Client.ContextProviders;
using codeRR.Client.Contracts;
using codeRR.Client.Processor;
using codeRR.Client.Reporters;

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