using System;
using System.Windows.Controls;
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
            var height = CalculateFormHeight();
            Height = height;
        }

        private void DialogPresenterFinishedReporting(object sender, EventArgs eventArgs)
        {
            Close();
        }

        private int CalculateFormHeight()
        {
            var height = 0;
            if (OneTrue.Configuration.UserInteraction.AskUserForDetails)
            {
                height += 200;
            }
            if (OneTrue.Configuration.UserInteraction.AskForEmailAddress)
            {
                height += 100;
            }
            height += 100;
            height += 100;
            return height;
        }
    }
}
