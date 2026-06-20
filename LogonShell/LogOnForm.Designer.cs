
namespace LogonShell
{
	partial class LogOnForm
	{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelUsername = new System.Windows.Forms.Label();
            this.labelPassword = new System.Windows.Forms.Label();
            this.buttonShutdown = new System.Windows.Forms.Button();
            this.labelLoading = new System.Windows.Forms.Label();
            this.comboBoxUsername = new System.Windows.Forms.ComboBox();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.labelShutdown = new System.Windows.Forms.Label();
            this.labelHint = new System.Windows.Forms.Label();
            this.backgroundWorkerCloseOnLogon = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerInteractiveUpdate = new System.ComponentModel.BackgroundWorker();
            this.labelCapsLock = new System.Windows.Forms.Label();
            this.backgroundWorkerCapsLockDetect = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(84, 135);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(243, 20);
            this.textBoxPassword.TabIndex = 2;
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Enabled = false;
            this.buttonCancel.Location = new System.Drawing.Point(245, 171);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(164, 171);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // labelUsername
            // 
            this.labelUsername.AutoSize = true;
            this.labelUsername.Location = new System.Drawing.Point(12, 111);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(61, 13);
            this.labelUsername.TabIndex = 6;
            this.labelUsername.Text = "User name:";
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(12, 138);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(56, 13);
            this.labelPassword.TabIndex = 7;
            this.labelPassword.Text = "Password:";
            // 
            // buttonShutdown
            // 
            this.buttonShutdown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonShutdown.Location = new System.Drawing.Point(326, 171);
            this.buttonShutdown.Name = "buttonShutdown";
            this.buttonShutdown.Size = new System.Drawing.Size(75, 23);
            this.buttonShutdown.TabIndex = 5;
            this.buttonShutdown.Text = "Shutdown...";
            this.buttonShutdown.UseVisualStyleBackColor = true;
            this.buttonShutdown.Click += new System.EventHandler(this.buttonShutdown_Click);
            // 
            // labelLoading
            // 
            this.labelLoading.AutoSize = true;
            this.labelLoading.Location = new System.Drawing.Point(12, 104);
            this.labelLoading.Name = "labelLoading";
            this.labelLoading.Size = new System.Drawing.Size(161, 13);
            this.labelLoading.TabIndex = 11;
            this.labelLoading.Text = "Applying your personal settings...";
            this.labelLoading.Visible = false;
            // 
            // comboBoxUsername
            // 
            this.comboBoxUsername.FormattingEnabled = true;
            this.comboBoxUsername.Location = new System.Drawing.Point(84, 107);
            this.comboBoxUsername.Name = "comboBoxUsername";
            this.comboBoxUsername.Size = new System.Drawing.Size(243, 21);
            this.comboBoxUsername.TabIndex = 13;
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(84, 107);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(243, 20);
            this.textBoxUsername.TabIndex = 14;
            this.textBoxUsername.Visible = false;
            // 
            // labelShutdown
            // 
            this.labelShutdown.AutoSize = true;
            this.labelShutdown.Location = new System.Drawing.Point(12, 104);
            this.labelShutdown.MaximumSize = new System.Drawing.Size(385, 0);
            this.labelShutdown.Name = "labelShutdown";
            this.labelShutdown.Size = new System.Drawing.Size(84, 13);
            this.labelShutdown.TabIndex = 17;
            this.labelShutdown.Text = "Shutting down...";
            this.labelShutdown.Visible = false;
            // 
            // labelHint
            // 
            this.labelHint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelHint.AutoSize = true;
            this.labelHint.BackColor = System.Drawing.SystemColors.Control;
            this.labelHint.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelHint.Location = new System.Drawing.Point(81, 158);
            this.labelHint.Name = "labelHint";
            this.labelHint.Size = new System.Drawing.Size(26, 13);
            this.labelHint.TabIndex = 18;
            this.labelHint.Text = "Hint";
            this.labelHint.Visible = false;
            // 
            // backgroundWorkerCloseOnLogon
            // 
            this.backgroundWorkerCloseOnLogon.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerCloseOnLogon_DoWork);
            // 
            // backgroundWorkerInteractiveUpdate
            // 
            this.backgroundWorkerInteractiveUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerInteractiveUpdate_DoWork);
            // 
            // labelCapsLock
            // 
            this.labelCapsLock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelCapsLock.AutoSize = true;
            this.labelCapsLock.BackColor = System.Drawing.SystemColors.Control;
            this.labelCapsLock.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelCapsLock.Location = new System.Drawing.Point(84, 157);
            this.labelCapsLock.Name = "labelCapsLock";
            this.labelCapsLock.Size = new System.Drawing.Size(86, 13);
            this.labelCapsLock.TabIndex = 24;
            this.labelCapsLock.Text = "Caps Lock is on.";
            this.labelCapsLock.Visible = false;
            // 
            // backgroundWorkerCapsLockDetect
            // 
            this.backgroundWorkerCapsLockDetect.WorkerSupportsCancellation = true;
            this.backgroundWorkerCapsLockDetect.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerCapsLockDetect_DoWork);
            // 
            // LogOnForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 225);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonShutdown);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.labelCapsLock);
            this.Controls.Add(this.labelShutdown);
            this.Controls.Add(this.textBoxUsername);
            this.Controls.Add(this.comboBoxUsername);
            this.Controls.Add(this.labelLoading);
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.labelUsername);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.labelHint);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogOnForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Log On to Windows";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxPassword;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Label labelUsername;
		private System.Windows.Forms.Label labelPassword;
		private System.Windows.Forms.Button buttonShutdown;
        private System.Windows.Forms.Label labelLoading;
        private System.Windows.Forms.ComboBox comboBoxUsername;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.Label labelShutdown;
        private System.Windows.Forms.Label labelHint;
        private System.ComponentModel.BackgroundWorker backgroundWorkerCloseOnLogon;
        private System.ComponentModel.BackgroundWorker backgroundWorkerInteractiveUpdate;
        private System.Windows.Forms.Label labelCapsLock;
        private System.ComponentModel.BackgroundWorker backgroundWorkerCapsLockDetect;
    }
}
