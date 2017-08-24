Core client for OneTrueError
================

![](https://onetrueerror.visualstudio.com/_apis/public/build/definitions/75570083-b1ef-4e78-88e2-5db4982f756c/4/badge)

This client library is used to manually report exceptions to OneTrueError (`OneTrue.Report(exception)`).

For automated handling, use one of the integration libraries:

Regular .NET

* [ASP.NET](https://github.com/onetrueerror/onetrueerror.client.aspnet)
* [ASP.NET MVC5](https://github.com/onetrueerror/onetrueerror.client.aspnet.mvc5)
* [ASP.NET WebApi2](https://github.com/onetrueerror/onetrueerror.client.aspnet.webapi2)
* [log4net](https://github.com/onetrueerror/onetrueerror.client.log4net)
* [WinForms](https://github.com/onetrueerror/onetrueerror.client.winforms)
* [WPF](https://github.com/onetrueerror/onetrueerror.client.wpf)

.NET Standard

* [NetStd](https://github.com/onetrueerror/OneTrueError.Client.NetStandard)


#  Features in this library

* HTTP proxy detection and usage when error reports are uploaded.
* Queued uploads (to allow the application to still be responsive, even if uploading are done over a slow connection)
* Compressed upload to minimize bandwidth usage.
* Context data collection
* Custom context data
 * Anonymous object
 * View models etc
* Adding tags to errors
* Allow user to leave feedback
* Automated information collection from windows, the process and the current thread.
