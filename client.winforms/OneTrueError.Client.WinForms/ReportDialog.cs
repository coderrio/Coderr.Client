using System;
using System.Windows.Forms;
using OneTrueError.Client.ContextCollections;
using OneTrueError.Client.Contracts;

namespace OneTrueError.Client.WinForms
{
    /// <summary>
    ///     Default dialog which is shown when an error has been caught
    /// </summary>
    public partial class ReportDialog : Form
    {
        private readonly ErrorReportDTO _dto;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReportDialog" /> class.
        /// </summary>
        public ReportDialog(ErrorReportDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            _dto = dto;

            InitializeComponent();
            /*feedback1.Visible = OneTrue.Configuration.AskUserForDetails;
            feedback2.Visible = OneTrue.Configuration.AskUserForDetails;
            allowCollect1.Visible = OneTrue.Configuration.AskUserForPermission;
            allowCollect2.Visible = OneTrue.Configuration.AskUserForPermission;
            allowCollect3.Visible = OneTrue.Configuration.AskUserForPermission;
            allowCollect4.Visible = OneTrue.Configuration.AskUserForPermission;*/

            if (!OneTrue.Configuration.UserInteraction.AskUserForDetails)
            {
                controlsPanel.Controls.Remove(errorDescription1);
            }
            if (!OneTrue.Configuration.UserInteraction.AskForEmailAddress)
            {
                controlsPanel.Controls.Remove(notificationControl1);
            }
            if (!OneTrue.Configuration.UserInteraction.AskUserForPermission)
            {
                btnCancel.Hide();
            }

            var height = CalculateFormHeight();
            Height = height;
            if (controlsPanel.Controls.Count == 2)
                Width = 550;
        }

        /// <summary>
        ///     Exception message
        /// </summary>
        public string ExceptionMessage
        {
            set { lblErrorMessage.Text = value; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSubmit_Click_1(object sender, EventArgs e)
        {
            var info = errorDescription1.UserInfo;
            var email = notificationControl1.Email;

            // only upload it if the flag is set, it have already been uploaded otherwise.
            if (OneTrue.Configuration.UserInteraction.AskUserForPermission)
                OneTrue.UploadReport(_dto);

            if (!string.IsNullOrEmpty(info) || !string.IsNullOrEmpty(email))
            {
                OneTrue.LeaveFeedback(_dto.ReportId, new UserSuppliedInformation(info, email));
            }

            Close();
        }

        private int CalculateFormHeight()
        {
            var height = 0;
            foreach (Control control in controlsPanel.Controls)
            {
                height += control.Height;
            }
            height += panel1.Height;
            height += 100;
            return height;
        }

        private void errorDescription1_Load(object sender, EventArgs e)
        {
        }

        private void flowLayoutPanel1_Resize(object sender, EventArgs e)
        {
            controlsPanel.SuspendLayout();

            foreach (Control control in controlsPanel.Controls)
            {
                control.Width = controlsPanel.Width - 10;
            }

            controlsPanel.ResumeLayout();
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }


        private void ReportDialog_Load(object sender, EventArgs e)
        {
        }
    }
}