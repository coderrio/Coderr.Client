ASP.NET client for OneTrueError
================================

You've just installed the ASP.NET integration library for OneTrueError. 
All unhandled exceptions will automatically be uploaded to OneTrueError (http://onetrueerror.com).

To get started add the following code to your application:

	var url = new Uri("http://yourServer/onetrueerror/");
	OneTrue.Configuration.Credentials(url, "yourAppKey", "yourSharedSecret");
	OneTrue.Configuration.CatchAspNetExceptions();

(this library requires that you have installed a OneTrueError server somewhere)

More information
===================

http://onetrueerror.com/documentation/client/libraries/aspnet/install.md


