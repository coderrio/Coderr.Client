using System.Web.Mvc;
using OneTrueError.Client.AspNet.Mvc5;
using OneTrueError.Client.AspNet.Mvc5.ContextProviders;
using OneTrueError.Client.Config;

// ReSharper disable once CheckNamespace

namespace OneTrueError.Client
{
    /// <summary>
    ///     Configuration extensions specific for ASP.NET MVC. Read the <see cref="OneTrueConfiguration" /> documentation for
    ///     all
    ///     configuration options.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        ///     Activate our automatic detection of unhandled exceptions.
        /// </summary>
        /// <param name="configurator">config class</param>
        /// <remarks>
        ///     <para>Adds context collectecors for forms, query string, sessions, user agent and http headers.</para>
        ///     <para>
        ///         MVC exceptions are dected through a custom global error filter.
        ///     </para>
        /// </remarks>
        public static void CatchMvcExceptions(this OneTrueConfiguration configurator)
        {
            configurator.ContextProviders.Add(new FormProvider());
            configurator.ContextProviders.Add(new QueryStringProvider());
            configurator.ContextProviders.Add(new SessionProvider());
            configurator.ContextProviders.Add(new HttpHeadersProvider());
            GlobalFilters.Filters.Add(new OneTrueErrorFilter());
            ErrorHttpModule.Activate();
        }

        /// <summary>
        ///     Display the built in error pages.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         OneTrueError has a set of built in error pages which can shown when an exception is thrown.
        ///     </para>
        /// </remarks>
        public static void DisplayErrorPages(this OneTrueConfiguration instance)
        {
            ErrorHttpModule.DisplayErrorPage = true;
        }
    }
}