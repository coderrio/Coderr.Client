Core client for codeRR
======================

![](https://1tcompany.visualstudio.com/_apis/public/build/definitions/75570083-b1ef-4e78-88e2-5db4982f756c/4/badge) [![NuGet](https://img.shields.io/nuget/dt/codeRR.Client.svg?style=flat-square)]()

This client library is used to manually report exceptions to codeRR (`Err.Report(exception)`).

For more information about codeRR, check the [homepage](https://coderrapp.com).

# Installation

1. Download and install the [codeRR server](https://github.com/coderrapp/coderr.server) or create an account at [coderrapp.com](https://coderrapp.com/live)
2. Install this client library (using nuget `coderr.client`)
3. Configure the credentials from your codeRR account in your `Program.cs`.

# Getting started

Simply catch an exception and report it:

```csharp
public void UpdatePost(int uid, ForumPost post)
{
	try
	{
		_service.Update(uid, post);
	}
	catch (Exception ex)
	{
		Err.Report(ex, new{ UserId = uid, ForumPost = post });
	}
}
```

The context information will be attached as:

![](https://coderrapp.com/images/features/custom-context.png)

[Read more...](https://coderrapp.com/features/)


# Automated handling

For automated handling, use one of the integration libraries:

Regular .NET

* [ASP.NET](https://github.com/coderrapp/coderr.client.aspnet)
* [ASP.NET MVC5](https://github.com/coderrapp/coderr.client.aspnet.mvc5)
* [ASP.NET WebApi2](https://github.com/coderrapp/coderr.client.aspnet.webapi2)
* [log4net](https://github.com/coderrapp/coderr.client.log4net)
* [WinForms](https://github.com/coderrapp/coderr.client.winforms)
* [WPF](https://github.com/coderrapp/coderr.client.wpf)

.NET Standard

* [NetStd](https://github.com/coderrapp/coderr.Client.NetStandard)
* [ASP.NET Core MVC](https://www.nuget.org/packages?q=coderr.client.aspnetcore.mvc)


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

# Requirements

You need to either install [codeRR Community Server](https://github.com/coderrapp/coderr.server) or use [codeRR Live](https://coderrapp.com/live).

# More information

* [Documentation](https://coderrapp.com/documentation/client/libraries/)
* [Forum](http://discuss.coderrapp.com)
