codeRR client package
=====================

Welcome to codeRR! 

We try to answer questions as fast as we can at our forum: http://discuss.coderrapp.com. 
If you have any trouble at all, don't hesitate to post a message there.

This library is the client library of codeRR. What it does is to report exceptions to codeRR.

However, this library do not process the information but require a codeRR server for that.
You can either install the open source server from https://github.com/coderrapp/coderr.server, or
use our hosted service at https://coderrapp.com/live.


Configuration
=============

To start with, you need to configure the connection to the codeRR server, 
this code is typically added in your Program.cs. This information is found either
in our hosted service or in your installed codeRR server.

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

Questions? http://discuss.coderrapp.com
More examples: https://coderrapp.com/documentation/client/
