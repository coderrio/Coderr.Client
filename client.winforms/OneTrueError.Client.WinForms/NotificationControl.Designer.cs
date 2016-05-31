namespace OneTrueError.Reporting.WinForms
{
    partial class NotificationControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbEmail = new System.Windows.Forms.TextBox();
            this.allowCollect3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tbEmail
            // 
            this.tbEmail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbEmail.Location = new System.Drawing.Point(0, 33);
            this.tbEmail.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.tbEmail.Name = "tbEmail";
            this.tbEmail.Size = new System.Drawing.Size(401, 20);
            this.tbEmail.TabIndex = 6;
            this.tbEmail.TextChanged += new System.EventHandler(this.tbEmail_TextChanged);
            // 
            // allowCollect3
            // 
            this.allowCollect3.AutoSize = true;
            this.allowCollect3.Dock = System.Windows.Forms.DockStyle.Top;
            this.allowCollect3.Location = new System.Drawing.Point(0, 20);
            this.allowCollect3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.allowCollect3.Name = "allowCollect3";
            this.allowCollect3.Size = new System.Drawing.Size(398, 13);
            this.allowCollect3.TabIndex = 5;
            this.allowCollect3.Text = "Enter your email address if you would like to receive status updates about this e" +
    "rror.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.label1.Size = new System.Drawing.Size(111, 20);
            this.label1.TabIndex = 7;
            this.label1.Text = "Status updates";
            // 
            // NotificationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbEmail);
            this.Controls.Add(this.allowCollect3);
            this.Controls.Add(this.label1);
            this.Name = "NotificationControl";
            this.Size = new System.Drawing.Size(401, 53);
            this.Load += new System.EventHandler(this.NotificationControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbEmail;
        private System.Windows.Forms.Label allowCollect3;
        private System.Windows.Forms.Label label1;
    }
}
