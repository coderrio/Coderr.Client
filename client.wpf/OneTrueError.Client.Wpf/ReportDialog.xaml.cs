using System;
using OneTrueError.Client.Contracts;

namespace OneTrueError.Client.Wpf
{
    /// <summary>
    /// Interaction logic for ReportDialog.xaml
    /// </summary>
    public partial class ReportDialog
    {
        public string ExceptionMessage { get; set; }

        public ReportDialog(ErrorReportDTO dto, string exceptionMessage)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            ErrorReportDetailsProvider.DtoReport = dto;
            ErrorReportDetailsProvider.ExceptionMessage = exceptionMessage;
            InitializeComponent();
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
