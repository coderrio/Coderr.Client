Core client for Coderr
======================

![](https://1tcompany.visualstudio.com/_apis/public/build/definitions/75570083-b1ef-4e78-88e2-5db4982f756c/4/badge) [![NuGet](https://img.shields.io/nuget/dt/codeRR.Client.svg?style=flat-square)]()

This client library is used to manually report exceptions to Coderr (`Err.Report(exception)`).

For more information about Coderr, visit our [homepage](https://coderr.io).

[Getting started guide](https://coderr.io/documentation/getting-started/)

# Reporting the first error

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

![](https://coderrio/images/features/custom-context.png)

[Read more...](https://coderrio/features/)


# Automated handling

For automated handling, use one of the integration libraries found in nuget.

https://www.nuget.org/packages?q=coderr.client

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

# More information

* [Documentation](https://coderr.io/documentation/client/libraries/)
* [Forum](http://discuss.coderr.io)
