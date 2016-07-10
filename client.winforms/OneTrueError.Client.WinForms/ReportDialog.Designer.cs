namespace OneTrueError.Client.WinForms
{
    partial class ReportDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportDialog));
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblErrorMessage = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.controlsPanel = new System.Windows.Forms.Panel();
            this.errorDescription1 = new ErrorDescription();
            this.notificationControl1 = new NotificationControl();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.controlsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 71);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 1;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(4);
            this.label4.Size = new System.Drawing.Size(411, 23);
            this.label4.TabIndex = 8;
            this.label4.Text = "Error message";
            // 
            // lblErrorMessage
            // 
            this.lblErrorMessage.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblErrorMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblErrorMessage.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblErrorMessage.Location = new System.Drawing.Point(0, 23);
            this.lblErrorMessage.Name = "lblErrorMessage";
            this.lblErrorMessage.Padding = new System.Windows.Forms.Padding(4);
            this.lblErrorMessage.Size = new System.Drawing.Size(411, 37);
            this.lblErrorMessage.TabIndex = 9;
            this.lblErrorMessage.Text = "label3";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnSubmit);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(199, 299);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(4);
            this.panel1.Size = new System.Drawing.Size(411, 35);
            this.panel1.TabIndex = 8;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSubmit.Location = new System.Drawing.Point(195, 4);
            this.btnSubmit.Margin = new System.Windows.Forms.Padding(2);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(124, 27);
            this.btnSubmit.TabIndex = 7;
            this.btnSubmit.Text = "&Send error report";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click_1);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.Location = new System.Drawing.Point(319, 4);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 27);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "&Don\'t send";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1.Image = global::OneTrueError.Client.WinForms.Properties.Resources.bigstock_Log_Output_45918232;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(199, 334);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // controlsPanel
            // 
            this.controlsPanel.Controls.Add(this.errorDescription1);
            this.controlsPanel.Controls.Add(this.lblErrorMessage);
            this.controlsPanel.Controls.Add(this.notificationControl1);
            this.controlsPanel.Controls.Add(this.label4);
            this.controlsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlsPanel.Location = new System.Drawing.Point(199, 0);
            this.controlsPanel.Name = "controlsPanel";
            this.controlsPanel.Size = new System.Drawing.Size(411, 299);
            this.controlsPanel.TabIndex = 10;
            // 
            // errorDescription1
            // 
            this.errorDescription1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorDescription1.Location = new System.Drawing.Point(0, 60);
            this.errorDescription1.Margin = new System.Windows.Forms.Padding(4);
            this.errorDescription1.Name = "errorDescription1";
            this.errorDescription1.Padding = new System.Windows.Forms.Padding(4);
            this.errorDescription1.Size = new System.Drawing.Size(411, 176);
            this.errorDescription1.TabIndex = 3;
            this.errorDescription1.Load += new System.EventHandler(this.errorDescription1_Load);
            // 
            // notificationControl1
            // 
            this.notificationControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.notificationControl1.Location = new System.Drawing.Point(0, 236);
            this.notificationControl1.Margin = new System.Windows.Forms.Padding(4);
            this.notificationControl1.Name = "notificationControl1";
            this.notificationControl1.Padding = new System.Windows.Forms.Padding(4);
            this.notificationControl1.Size = new System.Drawing.Size(411, 63);
            this.notificationControl1.TabIndex = 4;
            // 
            // ReportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 334);
            this.Controls.Add(this.controlsPanel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ReportDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Application error";
            this.Load += new System.EventHandler(this.ReportDialog_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.controlsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private ErrorDescription errorDescription1;
        private NotificationControl notificationControl1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblErrorMessage;
        private System.Windows.Forms.Panel controlsPanel;
    }
}