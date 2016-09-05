using System;
using System.Data;
using System.Windows.Forms;

namespace Demo
{
    public partial class CreateUserForm : Form
    {
        public CreateUserForm()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SomeOperation();
        }

        private void SomeOperation()
        {
            throw new DataException("Failed to save user");
        }
    }
}