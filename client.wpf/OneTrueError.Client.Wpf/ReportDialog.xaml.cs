using System;
using OneTrueError.Client.Contracts;

namespace OneTrueError.Client.Wpf
{
    /// <summary>
    /// Interaction logic for ReportDialog.xaml
    /// </summary>
    public partial class ReportDialog
    {
        public ErrorReportDialogPresenter DialogPresenter { get; set; }

        public string ExceptionMessage { get; set; }

        public ReportDialog(ErrorReportDTO dto, string exceptionMessage)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            InitializeComponent();
            var errorMessagePresenter = new ErrorMessagePresenter(exceptionMessage);
            var userErrorDescriptionPresenter = new UserErrorDescriptionPresenter();
            var notificationControlPresenter = new NotificationControlPresenter();

            DialogPresenter = new ErrorReportDialogPresenter(dto)
            {
                ErrorMessage = errorMessagePresenter,
                UserErrorDescription = userErrorDescriptionPresenter,
                NotificationControl = notificationControlPresenter,
            };

            DialogPresenter.FinishedReporting += DialogPresenterFinishedReporting;
//            var height = CalculateFormHeight();
//            Height = height;
//            if (controlsPanel.Controls.Count == 2)
//                Width = 550;
        }

        private void DialogPresenterFinishedReporting(object sender, EventArgs eventArgs)
        {
            Close();
        }
    }
}
