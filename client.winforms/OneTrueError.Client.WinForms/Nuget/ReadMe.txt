OneTrueError - WinForms integration
===================================

Congratulations on taking the first step toward a more efficient exception handling.

Now you need to either download and install the open source server: https://github.com/gauffininteractive/OneTrueError.Server/
.. or create an account at https://onetrueerror.com.

Once done, log into the server and find the configuration instructions.
(Or read the articles in our documentation: https://onetrueerror.com/documentation)


Configuration example
=====================

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var url = new Uri("https://report.onetrueerror.com");
        OneTrue.Configuration.Credentials(url, "yourAppKey", "yourSharedSecret");
        OneTrue.Configuration.CatchWinFormsExceptions();

        // take screen shot every time an exception is reported.
        OneTrue.Configuration.TakeScreenshotOfActiveFormOnly();
        OneTrue.Configuration.TakeScreenshots();

        //control the design of the built in error form
        OneTrue.Configuration.UserInteraction.AskUserForDetails = true;
        OneTrue.Configuration.UserInteraction.AskUserForPermission = true;
        OneTrue.Configuration.UserInteraction.AskForEmailAddress = true;

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new CreateUserForm());
    }
}
