using System;
using System.Data;
using System.IO;
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
            var url = new Uri("http://localhost:50473/");
            OneTrue.Configuration.Credentials(url, "13d82df603a845c7a27164c4fec19dd6", "6f0a0a7fac6d42caa7cc47bb34a6520b");

            //var feedback = new UserSuppliedInformation("I pressed the 'any' key.", "tjosan@gauffin.com");
            //OneTrue.LeaveFeedback("dbDTmB8cAEus7vwVviH2Zw", feedback);
            //return;
            try
            {
                throw new InvalidOperationException("mofo");
                //throw new InvalidDataException("corrupt data");
            }
            catch (Exception ex)
            {
                OneTrue.Report(ex);
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
