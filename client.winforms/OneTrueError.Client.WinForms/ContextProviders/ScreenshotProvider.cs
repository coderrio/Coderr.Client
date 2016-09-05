using System;
using System.Collections.Generic;
using System.Threading;
using OneTrueError.Client.ContextProviders;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.WinForms.ContextProviders
{
    /// <summary>
    ///     Can capture a screenshot of all open forms
    /// </summary>
    public class ScreenshotProvider : IContextInfoProvider
    {
        /// <summary>
        ///     "Screenshots"
        /// </summary>
        public const string NAME = "Screenshots";

        private readonly bool _allForms;

        /// <summary>
        ///     Creates a new instance of "<see cref="ScreenshotProvider" />
        /// </summary>
        /// <param name="ofAllForms">if <c>true</c>: take a screenshot of all open forms; otherwise only of the active form.</param>
        public ScreenshotProvider(bool ofAllForms)
        {
            _allForms = ofAllForms;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="ScreenshotProvider" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Will only take a screenshot of the active form.
        ///     </para>
        /// </remarks>
        public ScreenshotProvider()
            : this(false)
        {
        }

        /// <summary>
        ///     "Screenshots"
        /// </summary>
        public string Name
        {
            get { return NAME; }
        }

        /// <summary>
        ///     Collect
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            try
            {
                var shooter = new FormScreenshooter();

                var context2 = _allForms
                    ? shooter.CaptureAllOpenForms()
                    : shooter.CaptureActiveForm();

                return context2;
            }
            catch (Exception ex)
            {
                return new ContextCollectionDTO("Screenshots", new Dictionary<string, string>
                {
                    {"Error", ex.ToString()},
                    {"Thread", Thread.CurrentThread.ManagedThreadId + "[" + Thread.CurrentThread.Name + "]"}
                });
            }
        }
    }
}