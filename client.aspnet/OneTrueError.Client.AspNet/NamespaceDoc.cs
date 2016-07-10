using System.Runtime.CompilerServices;
using OneTrueError.Client.Config;

namespace OneTrueError.Client.AspNet
{
    /// <summary>
    ///     <para>
    ///         The ASP.NET integration library.
    ///     </para>
    ///     <para>
    ///         To use this package you have to install the <c>onetrueerror.aspnet</c> nuget package. To active the package,
    ///         invoke <c>OneTrue.Configuration.CatchAspNetErrors();</c>
    ///     </para>
    ///     <para>
    ///         This library is used to catch all asp.net related errors, including errors for MVC etc. However, if you want
    ///         ASP.NET MVC specific information like RouteData or ViewBag to be included in the error report you need to use
    ///         the <c>onetrueerror.mvc5</c> package instead.
    ///     </para>
    ///     <para>
    ///         The error returned by this library depends on the content-type requested by the client (using the Accept HTTP
    ///         header). For instance, if
    ///         only JSON is requested, the error will be returned as an JSON object to the client.
    ///     </para>
    ///     <para>
    ///         You can customize the HTML error page by using
    ///         <see cref="ConfigExtensions.SetErrorPageGenerator" /> method. Read it's
    ///         documentation for more information.
    ///     </para>
    ///     <example>
    ///         <para>
    ///             To start using the library (i.e. allow OTE to automatically handle all uncaught exceptions):
    ///         </para>
    ///         <code>
    /// public class WebApiApplication : System.Web.HttpApplication
    /// {
    /// 	protected void Application_Start()
    /// 	{
    ///         // The appkey and shared key can be found in onetrueeror.com
    /// 		OneTrue.Configuration.Credentials("yourAppKey", "yourSharedSecret");
    /// 		OneTrue.Configuration.CatchAspNetExceptions();
    /// 	}
    /// }
    /// </code>
    ///         <para>you can also configure your own error page:</para>
    ///         <code>
    /// OneTrue.Configuration.SetErrorPageGenerator("~/views/error.aspx");
    /// </code>
    ///         <para>
    ///             It must contain some macros which is documented here:
    ///             <see cref="ConfigExtensions.SetErrorPageGenerator" />
    ///         </para>
    ///     </example>
    ///     <para>
    ///         Additional ASP.NET specific configuration options:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             <see cref="ConfigExtensions.SetErrorPageGenerator" />
    ///         </item>
    ///     </list>
    /// </summary>
    /// <seealso cref="ConfigExtensions" />
    [CompilerGenerated]
    public class NamespaceDoc
    {
    }
}