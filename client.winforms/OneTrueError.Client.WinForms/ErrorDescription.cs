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
    internal partial class ErrorDescription : UserControl
    {
        public ErrorDescription()
        {
            InitializeComponent();
        }

        public string UserInfo { get { return feedback2.Text; } }
    }
}
