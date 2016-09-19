using System;
using System.Windows;
using System.Windows.Threading;

namespace OneTrueError.Client.Wpf
{
    public class WpfErrorReporter
    {
        private static readonly WpfErrorReporter Instance = new WpfErrorReporter();
        private static bool _activated;

        static WpfErrorReporter()
        {
            FormFactory =
                model =>
                    new ReportDialog(model.Report) { ExceptionMessage = model.Context.Exception.Message };
        }

        internal static Func<WindowFactoryContext, Window> FormFactory { get; set; }

        /// <summary>
        ///     Activate this library.
        /// </summary>
        public static void Activate()
        {
            if (_activated)
                return;


            _activated = true;

            Application.Current.DispatcherUnhandledException += OnException;
        }

        private static void OnException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var context = new WpfErrorReportContext(Instance, e.Exception);

            var dto = OneTrue.GenerateReport(context);
            if (!OneTrue.Configuration.UserInteraction.AskUserForPermission)
                OneTrue.UploadReport(dto);

            var ctx = new WindowFactoryContext { Context = context, Report = dto };
            var dialog = FormFactory(ctx);
            dialog.ShowDialog();
        }

    }
}
