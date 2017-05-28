using System;
using System.Data;
using System.IO;
using System.Net;
using System.Resources;
using System.Text;
using OneTrueError.Client.ContextCollections;
using OneTrueError.Client.ContextProviders;

namespace OneTrueError.Client.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialization
            //var url = new Uri("http://localhost/onetrueerror/");
            //var url = new Uri("http://localhost:50473/");
            //var url = new Uri("http://localhost/ote_receiver/");
            //var url = new Uri("https://report.onetrueerror.com/");
            //OneTrue.Configuration.Credentials(url, "13d82df603a845c7a27164c4fec19dd6", "6f0a0a7fac6d42caa7cc47bb34a6520b");
            var uri = new Uri("http://localhost:50473/");
            OneTrue.Configuration.Credentials(uri,
                                              "a8bf3b6ad3644068adf809dc12908c3a",
                                              "86aede6dc3d74f0ab11df444522d780d");





            //org2
            //OneTrue.Configuration.Credentials(url, "84d867155dd649eb844dc4fec94a6a44", "5de125e1334c4e6caf6752a31dfbb2af");

            //org3
            //OneTrue.Configuration.Credentials(url, "bd0059cef67a42359439f72eeac25344", "36b6c2a655c84514b06a75763ceca03a");
            //OneTrue.Configuration.Credentials(url, "4345458073274131b9bf4f2260e527d2",
            //                    "e177ac8280394a6f9a6afe999b20810d");
            /*4345458073274131b9bf4f2260e527d2
SharedSecret	e177ac8280394a6f9a6afe999b20810d*/
            //var feedback = new UserSuppliedInformation("I pressed the 'any' key.", "tjosan@gauffin.com");
            //OneTrue.LeaveFeedback("dbDTmB8cAEus7vwVviH2Zw", feedback);
            //return;
            try
            {
                //throw new ArgumentException("Stop that arguing FFS!");
                //throw new InvalidOperationException("mofo2");
                throw new InvalidDataException("corrupt data2");
            }
            catch (Exception ex)
            {
                try
                {
                    OneTrue.Report(ex);
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
