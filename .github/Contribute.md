Contribution guidelines
=======================

First of all: Thank you for contributing to OneTrueError. 

We have a checklist that you need to go through before getting your pullrequest accepted:

## Maintain backwards compability

Tread carefully when changing the DTOs, backwards compatibility is very important to ensure that reports can be uploaded to older versions of the server too.

## Code should be documented

All public methods, types etc should be [documented](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/xml-documentation-comments)

## Code should be tested

Not all code is covered by tests today. However, our goal is to get everything covered by tests. Thus all code in pull requests should be covered by tests.

Tests method names should explain the business rule and not mirror the method name and the arguments of the tested method.

`public void should_required_userId_in_the_constructor_so_that_we_can_trace_who_made_the_change` instead of `AuditLog_Constructor_Require_UserId`.

## Code style

We use Resharper and resharpers default code style. It typicall follows the [Microsoft Framework Design Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/).
