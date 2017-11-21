codeRR client package
=====================

Welcome to codeRR! 

This library is the client library of codeRR. It allows you to manually report exceptions to the codeRR server.
If you would like to use automated reporting, install one of the other codeRR nuget packages.

This library doesn’t process the generated information. 
Information processing is done by the codeRR server which you will need to install.

For a server with full functionality, we recommend you to use our hosted service at https://coderrapp.com/live. 
But you can also use and install the open source server version from https://github.com/coderrapp/coderr.server.

For any questions that you might have, please use our forum at http://discuss.coderrapp.com. At the forum, we will try to answer questions as fast as we can and post answers to questions that have already been asked. Don't hesitate to use it! 

Configuration
=============

Start by configuring the connection to the codeRR server. The code below is typically added in 
your global.asax, Program.cs or Startup.cs. The configuration settings is found either in codeRR Live or 
in your installed codeRR server.

    public class Program
    {
        public static void Main(string[] args)
        {

            // codeRR configuration
            var uri = new Uri("https://report.coderrapp.com/");
            Err.Configuration.Credentials(uri,
                "yourAppKey",
                "yourSharedSecret");

            // the usual stuff
			// [...]
        }
    }

Want automated exception reporting? Install one of the integration packages instead. 
They are listed here: https://github.com/coderrapp/coderr.client/


Reporting exceptions
====================

This is one of many examples:

    public void SomeMethod(PostViewModel model)
    {
        try
        {
            _somService.Execute(model);
        }
        catch (Exception ex)
        {
            Err.Report(ex, model);

            //some custom handling
        }

        // some other code here...
    }

Again for questions, go to http://discuss.coderrapp.com
Additional documentation can be found at https://coderrapp.com/documentation/client/
