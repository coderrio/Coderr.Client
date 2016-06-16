ASP.NET installation
====================

You've just installed the ASP.NET integration library for OneTrueError. 
All uncaught exceptions will automatically be uploaded to OneTrueError.

To get started add the following code to your application:

	var url = new Uri("http://yourServer/onetrueerror/");
	OneTrue.Configuration.Credentials(url, "yourAppKey", "yourSharedSecret");
	OneTrue.Configuration.CatchAspNetExceptions();


This library requires that you have installed a OneTrueError server somewhere.


More information
================

http://onetrueerror.com/?from=aspnet - The service
http://onetrueerror.com/documentation/client/aspnet/install.md - ASP.NET help
http://onetrueerror.com/documentation/client/index.md - General Client help
