Log4Net client for OneTrueError
================================

You've just installed the log4net integration library for OneTrueError. 
All exceptions that are logged through log4net will automatically be uploaded to OneTrueError (http://onetrueerror.com).

To get started add the following code to your application:

	var url = new Uri("http://yourServer/onetrueerror/");
	OneTrue.Configuration.Credentials(url, "yourAppKey", "yourSharedSecret");
	OneTrue.Configuration.CatchLog4NetExceptions();

It must be added after the log4net configuration, but before the first usage of `LogManager.GetLogger()`.

(this library requires that you have installed a OneTrueError server somewhere)

More information
===================

http://onetrueerror.com/documentation/client/libraries/log4net/install.md


