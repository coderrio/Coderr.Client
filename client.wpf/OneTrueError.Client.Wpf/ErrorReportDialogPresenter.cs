using System;
using System.Drawing;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using OneTrueError.Client.ContextCollections;
using OneTrueError.Client.Contracts;

// ReSharper disable UseNullPropagation

// ReSharper disable ConvertPropertyToExpressionBody

namespace OneTrueError.Client.Wpf
{
    public class ErrorReportDialogPresenter
    {
        private readonly ErrorReportDTO _dto;

        public ErrorReportDialogPresenter(ErrorReportDTO dto)
        {
            _dto = dto;
        
            SubmitCommand = new DelegateCommand(SubmitReport);
            CancelCommand = new DelegateCommand(CancelReport);
        }


        public Bitmap SideBarImage
        {
            get { return Properties.Resources.bigstock_Log_Output_45918232; }
        }

        public bool AskForUserDetails
        {
            get { return OneTrue.Configuration.UserInteraction.AskUserForDetails; }
        }

        public bool AskForEmailAddress
        {
            get { return OneTrue.Configuration.UserInteraction.AskForEmailAddress; }
        }

        public bool AskForUserPermission
        {
            get { return OneTrue.Configuration.UserInteraction.AskUserForPermission; }
        }

        public ErrorMessagePresenter ErrorMessage { get; set; }

        public UserErrorDescriptionPresenter UserErrorDescription { get; set; }

        public NotificationControlPresenter NotificationControl { get; set; }

        public event EventHandler<EventArgs> FinishedReporting;

        public ICommand SubmitCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        private void SubmitReport()
        {
            var info = UserErrorDescription.UserInfo;
            var email = NotificationControl.Email;

            // only upload it if the flag is set, it have already been uploaded otherwise.
            if (OneTrue.Configuration.UserInteraction.AskUserForPermission)
                OneTrue.UploadReport(_dto);

            if (!string.IsNullOrEmpty(info) || !string.IsNullOrEmpty(email))
            {
                OneTrue.LeaveFeedback(_dto.ReportId, new UserSuppliedInformation(info, email));
            }
            PublishFinishedReporting();
        }

        private void CancelReport()
        {
            PublishFinishedReporting();
        }

        private void PublishFinishedReporting()
        {
            if (FinishedReporting != null)
            {
                FinishedReporting(this, new EventArgs());
            }
        }
    }
}
