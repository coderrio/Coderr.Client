Client libraries
================

Client libraries are used to detect exceptions, collect context information and finally upload everything to the server for analysis. There are one per library/framework that OneTrueError integrates with.

The libraries are installed through nuget. 

* [Documentation](https://onetrueerror.com/documentation/client) - Including how to extend or build a new library.

https://onetrueerror.com

## Core features

All features in the core library are also included in all other client libraries.

The core library have support for the following features:

* HTTP proxy detection and usage
* Queued uploads (to allow the application to still be responsive, even if uploading are done over a slow connection)
* Compressed upload to minimize bandwidth usage.
* Context data collection
* Custom context data
 * Anonymous object
 * View models etc
* Adding tags to errors
* Allow user to leave feedback
* Allow user to get status updates through email
 
## ASP.NET features

The ASP.NET library have support for the following features:

* Context information for:
 * Session contents
 * Form (if posted)
 * Files (names + size if uploaded)
 * Request headers (including URL)
* Custom error pages

## ASP.NET MVC5 features

All features for ASP.NET plus:

* Context information for:
 * ViewBag / ViewData
 * TempData
 * RouteData
 * ModelState
* A much easier (conventional) way to display error pages

## log4net features

Allows you to start reporting all exceptions that you are logging by just adding one line of code. Great for legacy applications.

The log4net library have support for the following features:

* Context information for:
 * Log message which had an exception included
 * Type that threw the exception

## WinForms features

The WinForms library have support for the following features:

* Context information for:
  * State of all open forms (i.e. text content of controls and properties)
  * Screen shot of all open forms (if configured)
