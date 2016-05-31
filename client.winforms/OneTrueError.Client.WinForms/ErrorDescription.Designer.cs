namespace OneTrueError.Reporting.WinForms
{
    partial class ErrorDescription
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
            this.feedback1 = new System.Windows.Forms.Label();
            this.feedback2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // feedback1
            // 
            this.feedback1.AutoSize = true;
            this.feedback1.Dock = System.Windows.Forms.DockStyle.Top;
            this.feedback1.Location = new System.Drawing.Point(0, 20);
            this.feedback1.Name = "feedback1";
            this.feedback1.Size = new System.Drawing.Size(248, 13);
            this.feedback1.TabIndex = 12;
            this.feedback1.Text = "Could you please let us know how to reproduce it? ";
            // 
            // feedback2
            // 
            this.feedback2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.feedback2.Location = new System.Drawing.Point(0, 33);
            this.feedback2.Multiline = true;
            this.feedback2.Name = "feedback2";
            this.feedback2.Size = new System.Drawing.Size(590, 152);
            this.feedback2.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.label1.Size = new System.Drawing.Size(78, 20);
            this.label1.TabIndex = 13;
            this.label1.Text = "Feedback";
            // 
            // ErrorDescription
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.feedback2);
            this.Controls.Add(this.feedback1);
            this.Controls.Add(this.label1);
            this.Name = "ErrorDescription";
            this.Size = new System.Drawing.Size(590, 185);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label feedback1;
        private System.Windows.Forms.TextBox feedback2;
        private System.Windows.Forms.Label label1;
    }
}
