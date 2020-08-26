using System;

namespace Coderr.Client.NetStd.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = new Uri("http://localhost:50473/");
            Err.Configuration.Credentials(url, 
                "ae0428b701054c5d9481024f81ad8b05", 
                "988cedd2bf4641d1aa228766450fab97");
            
            
            Err.ReportLogicError("User should have been assigned.");
            try
            {
                throw new NotSupportedException("Not invented here");
            }
            catch (Exception ex)
            {
                Err.Report(ex, new { myData = "hello", ErrTags = "important" });
            }
            Console.WriteLine("Hello World!");
        }
    }
}