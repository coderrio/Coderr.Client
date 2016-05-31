using System;
using System.Collections.Generic;
using System.Threading;
using OneTrueError.Reporting.ContextProviders;
using OneTrueError.Reporting.Contracts;
using OneTrueError.Reporting.Reporters;

namespace OneTrueError.Reporting.WinForms.ContextProviders
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
        public ContextInfoDTO Collect(IErrorReporterContext context)
        {
            try
            {
                var shooter = new FormScreenshooter();
                var context2= shooter.CaptureAllOpenForms();
                
                return context2;
            }
            catch (Exception ex)
            {
                return new ContextInfoDTO("Screenshots", new Dictionary<string, string>()
                {
                    {"Error", ex.ToString()},
                    {"Thread", Thread.CurrentThread.ManagedThreadId + "[" + Thread.CurrentThread.Name + "]"}
                });
            }
        }
    }
}