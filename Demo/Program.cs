using System;
using System.Windows.Forms;
using OneTrueError.Client;

namespace Demo
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var url = new Uri("http://localhost:50473/");
            OneTrue.Configuration.Credentials(url, "13d82df603a845c7a27164c4fec19dd6",
                "6f0a0a7fac6d42caa7cc47bb34a6520b");
            OneTrue.Configuration.CatchWinFormsExceptions();

            OneTrue.Configuration.TakeScreenshotOfActiveFormOnly();
            OneTrue.Configuration.TakeScreenshots();
            OneTrue.Configuration.UserInteraction.AskUserForDetails = true;
            OneTrue.Configuration.UserInteraction.AskUserForPermission = true;
            OneTrue.Configuration.UserInteraction.AskForEmailAddress = true;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CreateUserForm());
        }
    }
}