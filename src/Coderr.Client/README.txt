Coderr client package
=====================

This library is the client library of Coderr. It allows you to manually report exceptions to the Coderr server.
If you would like to use automated reporting, install one of our automation nuget packages.

https://coderr.io/documentation/getting-started/

Simple reporting example:

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

For questions: https://discuss.coderr.io
Homepage: https://coderr.io
