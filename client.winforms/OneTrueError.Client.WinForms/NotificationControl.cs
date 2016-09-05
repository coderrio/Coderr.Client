using System;
using System.Windows.Forms;

namespace OneTrueError.Client.WinForms
{
    internal partial class NotificationControl : UserControl
    {
        public NotificationControl()
        {
            InitializeComponent();
        }

        public string Email
        {
            get { return tbEmail.Text; }
        }

        private void NotificationControl_Load(object sender, EventArgs e)
        {
        }

        private void tbEmail_TextChanged(object sender, EventArgs e)
        {
        }
    }
}