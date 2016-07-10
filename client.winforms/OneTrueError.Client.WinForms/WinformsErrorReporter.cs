using System;
using System.Threading;
using System.Windows.Forms;
using OneTrueError.Client.Contracts;

namespace OneTrueError.Client.WinForms
{
    /// <summary>
    ///     Class that processes unhandled exceptions that WinForms/WPF applications throw.
    /// </summary>
    public class WinFormsErrorReporter
    {
        private static readonly WinFormsErrorReporter _instance = new WinFormsErrorReporter();

        private static bool _activated;
        internal static Func<FormFactoryContext, Form> FormFactory { get; set; }

        static WinFormsErrorReporter()
        {
            FormFactory =
                model =>
                    new ReportDialog(model.Report) {ExceptionMessage = model.Context.Exception.Message};
        }

        private static void OnException(object sender, ThreadExceptionEventArgs e)
        {
            var context = new WinformsErrorReportContext(_instance, e.Exception);

            var dto = OneTrue.GenerateReport(context);
            if (!OneTrue.Configuration.UserInteraction.AskUserForPermission)
                OneTrue.UploadReport(dto);

            var ctx = new FormFactoryContext { Context = context, Report = dto };
            var dialog = FormFactory(ctx);
            dialog.ShowDialog();
        }

        /// <summary>
        ///     Activate this library.
        /// </summary>
        public static void Activate()
        {
            if (_activated)
                return;


            _activated = true;

            Application.ThreadException += OnException;
        }
    }
}