using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneTrueError.Reporting.WinForms
{
    internal partial class NotificationControl : UserControl
    {
        public NotificationControl()
        {
            InitializeComponent();
        }

        private void NotificationControl_Load(object sender, EventArgs e)
        {

        }

        public string Email { get { return tbEmail.Text; }}

        private void tbEmail_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
