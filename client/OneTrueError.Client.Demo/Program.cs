using System;
using System.Data;
using System.Text;
using OneTrueError.Client.ContextProviders;

namespace OneTrueError.Client.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = new Uri("http://localhost/onetrueerror/");
            OneTrue.Configuration.Credentials(url, "13d82df603a845c7a27164c4fec19dd6", "6f0a0a7fac6d42caa7cc47bb34a6520b");
            OneTrue.Configuration.ContextProviders.Add(new ProcessorProvider());
            try
            {
                CallToThrow();
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
