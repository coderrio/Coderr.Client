using OneTrueError.Client.AspNet.ContextProviders;
using OneTrueError.Client.AspNet.ErrorPages;
using OneTrueError.Client.Config;

// Keeps in the root namespace to get intellisense

// ReSharper disable once CheckNamespace
namespace OneTrueError.Client
{
    /// <summary>
    ///     Extensions for the <see cref="OneTrue" /> configuration class.
    /// </summary>
    public static class ConfigExtensions
    {
        internal static bool CatchExceptions;

        internal static IErrorPageGenerator ErrorPageGenerator =
            new EmbeddedResourceGenerator(typeof (ConfigExtensions).Assembly, "OneTrueError.Client.AspNet.Views");

        /// <summary>
        ///     Activate the ASP.NET error catching library
        /// </summary>
        /// <param name="configurator">instance.</param>
        public static void CatchAspNetExceptions(this OneTrueConfiguration configurator)
        {
            // the HTTP module is always loaded.
            // this setting will be read by it to check if 
            // errors should be caught.
            CatchExceptions = true;

            configurator.ContextProviders.Add(new FormProvider());
            configurator.ContextProviders.Add(new QueryStringProvider());
            configurator.ContextProviders.Add(new SessionProvider());
            configurator.ContextProviders.Add(new HttpHeadersProvider());
        }

        /// <summary>
        ///     Sets the error page.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="errorPageGenerator">service used to provide error pages</param>
        /// <exception cref="System.ArgumentNullException">virtualPathOrCompleteErrorPageHtml</exception>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///     You have to have a <![CDATA[<form>]]> tag pointing at 'http://yourwebsite/onetrueerror/' in your error page if you
        ///     would like to collect error information.\r\n(Or set OneTrue.Configuration.AskUserForPermission and
        ///     OneTrue.Configuration.AskUserForDetails to false)
        ///     or
        ///     You have to have one or more '$reportId$' tags in your HTML which can be replaced with the correct report id
        ///     upon errors.
        /// </exception>
        /// <remarks>
        /// <para>
        /// For this plugin to work, you need to make sure that the <code>App_Data</code> folder exists and that the application pool account (typically IUSR) have write access to it. The folder
        /// is used to store error reports until they have been successfully uploaded to OneTrueError. In that way we can make sure that no reports are lost (even if there are network failure or other issues).
        /// </para>
        /// <para>
        ///     You can either return a complete error page (no virtual URIs for images etc) or a virtual path to a page which will
        ///     create the error page.
        /// </para>
        ///     <para>If you do the latter you can </para>
        ///     <para>
        ///         It's important that you've configured <c>OneTrue.Configuration.AskUserForDetails</c> and
        ///         <c>OneTrue.Configuration.AskUserForPermission</c> correctly if you
        ///         are using a custom error page, as setting one of those to true will result in that this module expects a HTTP
        ///         POST before sending the report.
        ///     </para>
        ///     <para>
        ///         All tags</para>
        ///         <list type="table">
        ///             <item>
        ///                 <term>$URL$</term>
        ///                 <description>
        ///                     Full URL which will be picked up by the OneTrueError module (to be able to send the report
        ///                     to the OneTrueError service)
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <term>$reportId$</term>
        ///                 <description>Token generated which is a unique identifier for the current exception.</description>
        ///             </item>
        ///         </list>
        ///     <para>Example HTML error page:</para>
        /// <code>
        /// <![CDATA[<!DOCTYPE html>
        /// <html lang="en">
        /// <head>
        ///     <meta charset="utf-8" />
        ///     <title>Something failed</title>
        ///     <meta name="ROBOTS" content="NOINDEX, NOFOLLOW" />
        ///     <style type="text/css">
        ///         /*CssStyles*/
        ///         body {
        ///             background: #f0f0f0;
        ///             font-family: Segoe UI, Arial;
        ///         }
        /// 
        ///         form {
        ///             padding: 10px;
        ///             background: white;
        ///             border: 1px solid #eeeeee;
        ///             width: 500px;
        ///         }
        ///     </style>
        /// </head>
        /// <body>
        ///     <h1>Something went wrong</h1>
        ///     <form method="post" action="$URL$">
        ///         <p>
        ///             We are terribly sorry for that. 
        ///         </p>
        ///         <p>
        ///             You can press the "Back" browser button to try again, or press the "Continue" button below.
        ///         </p>
        ///         <input type="hidden" value="$reportId$" name="reportId" />
        ///         <div class="AllowSubmissionStyle">
        ///             <p>
        ///                 However, If you allow us to collect error information we'll be able to analyze this error and fix it as soon as possible.
        ///             </p>
        ///             <input type="checkbox" name="Allowed" value="true" />
        ///             I allow you to collect information.
        ///         </div>
        ///         <div class="AllowFeedbackStyle">
        ///             <p>You can also help us understand this new feature by type in some information about how you got here:</p>
        ///             <textarea rows="10" cols="40" name="Description"></textarea>
        ///         </div>
        /// 
        ///         <div class="AskForEmailAddress">
        ///             <p>You can enter your email if you would like to get notified when this error has been fixed:</p>
        ///             <input type="text" name="email" placeholder="email address" />
        ///         </div>
        ///         <hr/>
        ///         <input type="submit" value="Continue" />
        ///     </form>
        /// </body>
        /// </html>
        /// ]]>
        /// </code>
        /// </remarks>
        public static void SetErrorPageGenerator(this OneTrueConfiguration configurator, IErrorPageGenerator errorPageGenerator)
        {
            ErrorPageGenerator = errorPageGenerator;
        }


    }
}