using System;

namespace Coderr.Client.Demo
{
    class Program
    {
        static void Main()
        {
            var url = new Uri("http://localhost:60473/");
            Err.Configuration.Credentials(url,
                "1a68bc3e123c48a3887877561b0982e2",
                "bd73436e965c4f3bb0578f57c21fde69");

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