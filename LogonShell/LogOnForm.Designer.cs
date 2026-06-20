
namespace LogonShell
{
	partial class LogOnForm
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
            this.components = new System.ComponentModel.Container();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.buttonOptions = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelUsername = new System.Windows.Forms.Label();
            this.labelPassword = new System.Windows.Forms.Label();
            this.buttonShutdown = new System.Windows.Forms.Button();
            this.buttonDummy = new System.Windows.Forms.Button();
            this.labelLoading = new System.Windows.Forms.Label();
            this.comboBoxUsername = new System.Windows.Forms.ComboBox();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.labelKeySequence = new System.Windows.Forms.Label();
            this.labelShutdown = new System.Windows.Forms.Label();
            this.labelHint = new System.Windows.Forms.Label();
            this.pictureBoxKeySequence = new System.Windows.Forms.PictureBox();
            this.pictureBoxLoading = new System.Windows.Forms.PictureBox();
            this.pictureBoxBanner = new System.Windows.Forms.PictureBox();
            this.backgroundWorkerCloseOnLogon = new System.ComponentModel.BackgroundWorker();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.comboBoxDomain = new System.Windows.Forms.ComboBox();
            this.checkBoxDialUp = new System.Windows.Forms.CheckBox();
            this.labelDomain = new System.Windows.Forms.Label();
            this.labelHelp = new System.Windows.Forms.Label();
            this.labelHelpLink = new System.Windows.Forms.Label();
            this.backgroundWorkerInteractiveUpdate = new System.ComponentModel.BackgroundWorker();
            this.labelCapsLock = new System.Windows.Forms.Label();
            this.pictureBoxCapsLock = new System.Windows.Forms.PictureBox();
            this.backgroundWorkerCapsLockDetect = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxKeySequence)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLoading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBanner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCapsLock)).BeginInit();
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
            // buttonOptions
            // 
            this.buttonOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOptions.Location = new System.Drawing.Point(326, 171);
            this.buttonOptions.Name = "buttonOptions";
            this.buttonOptions.Size = new System.Drawing.Size(75, 23);
            this.buttonOptions.TabIndex = 5;
            this.buttonOptions.Text = "Options > >";
            this.buttonOptions.UseVisualStyleBackColor = true;
            this.buttonOptions.Click += new System.EventHandler(this.buttonOptions_Click);
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
            this.buttonShutdown.Location = new System.Drawing.Point(245, 171);
            this.buttonShutdown.Name = "buttonShutdown";
            this.buttonShutdown.Size = new System.Drawing.Size(75, 23);
            this.buttonShutdown.TabIndex = 9;
            this.buttonShutdown.Text = "Shutdown...";
            this.buttonShutdown.UseVisualStyleBackColor = true;
            this.buttonShutdown.Visible = false;
            this.buttonShutdown.Click += new System.EventHandler(this.buttonShutdown_Click);
            // 
            // buttonDummy
            // 
            this.buttonDummy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDummy.Enabled = false;
            this.buttonDummy.Location = new System.Drawing.Point(83, 171);
            this.buttonDummy.Name = "buttonDummy";
            this.buttonDummy.Size = new System.Drawing.Size(75, 23);
            this.buttonDummy.TabIndex = 10;
            this.buttonDummy.Text = "Dummy";
            this.buttonDummy.UseVisualStyleBackColor = true;
            this.buttonDummy.Visible = false;
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
            // labelKeySequence
            // 
            this.labelKeySequence.AutoSize = true;
            this.labelKeySequence.Location = new System.Drawing.Point(61, 106);
            this.labelKeySequence.Name = "labelKeySequence";
            this.labelKeySequence.Size = new System.Drawing.Size(156, 13);
            this.labelKeySequence.TabIndex = 15;
            this.labelKeySequence.Text = "Press the Windows key to start.";
            this.labelKeySequence.Visible = false;
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
            // pictureBoxKeySequence
            // 
            this.pictureBoxKeySequence.Location = new System.Drawing.Point(12, 95);
            this.pictureBoxKeySequence.Name = "pictureBoxKeySequence";
            this.pictureBoxKeySequence.Size = new System.Drawing.Size(48, 32);
            this.pictureBoxKeySequence.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxKeySequence.TabIndex = 16;
            this.pictureBoxKeySequence.TabStop = false;
            this.pictureBoxKeySequence.Visible = false;
            // 
            // pictureBoxLoading
            // 
            this.pictureBoxLoading.Location = new System.Drawing.Point(0, 87);
            this.pictureBoxLoading.Name = "pictureBoxLoading";
            this.pictureBoxLoading.Size = new System.Drawing.Size(409, 6);
            this.pictureBoxLoading.TabIndex = 12;
            this.pictureBoxLoading.TabStop = false;
            // 
            // pictureBoxBanner
            // 
            this.pictureBoxBanner.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxBanner.Name = "pictureBoxBanner";
            this.pictureBoxBanner.Size = new System.Drawing.Size(409, 93);
            this.pictureBoxBanner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxBanner.TabIndex = 0;
            this.pictureBoxBanner.TabStop = false;
            // 
            // backgroundWorkerCloseOnLogon
            // 
            this.backgroundWorkerCloseOnLogon.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerCloseOnLogon_DoWork);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // comboBoxDomain
            // 
            this.comboBoxDomain.FormattingEnabled = true;
            this.comboBoxDomain.Location = new System.Drawing.Point(84, 161);
            this.comboBoxDomain.Name = "comboBoxDomain";
            this.comboBoxDomain.Size = new System.Drawing.Size(243, 21);
            this.comboBoxDomain.TabIndex = 19;
            this.comboBoxDomain.Visible = false;
            // 
            // checkBoxDialUp
            // 
            this.checkBoxDialUp.AutoSize = true;
            this.checkBoxDialUp.Location = new System.Drawing.Point(84, 161);
            this.checkBoxDialUp.Name = "checkBoxDialUp";
            this.checkBoxDialUp.Size = new System.Drawing.Size(80, 17);
            this.checkBoxDialUp.TabIndex = 20;
            this.checkBoxDialUp.Text = "checkBox1";
            this.checkBoxDialUp.UseVisualStyleBackColor = true;
            this.checkBoxDialUp.Visible = false;
            // 
            // labelDomain
            // 
            this.labelDomain.AutoSize = true;
            this.labelDomain.Location = new System.Drawing.Point(12, 165);
            this.labelDomain.Name = "labelDomain";
            this.labelDomain.Size = new System.Drawing.Size(55, 13);
            this.labelDomain.TabIndex = 21;
            this.labelDomain.Text = "Log on to:";
            this.labelDomain.Visible = false;
            // 
            // labelHelp
            // 
            this.labelHelp.AutoSize = true;
            this.labelHelp.Location = new System.Drawing.Point(14, 131);
            this.labelHelp.Name = "labelHelp";
            this.labelHelp.Size = new System.Drawing.Size(361, 13);
            this.labelHelp.TabIndex = 22;
            this.labelHelp.Text = "Windows helps keep your password secure. Click Help for more information";
            this.labelHelp.Visible = false;
            // 
            // labelHelpLink
            // 
            this.labelHelpLink.AutoSize = true;
            this.labelHelpLink.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelHelpLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHelpLink.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.labelHelpLink.Location = new System.Drawing.Point(368, 144);
            this.labelHelpLink.Name = "labelHelpLink";
            this.labelHelpLink.Size = new System.Drawing.Size(29, 13);
            this.labelHelpLink.TabIndex = 23;
            this.labelHelpLink.Text = "Help";
            this.labelHelpLink.Visible = false;
            this.labelHelpLink.Click += new System.EventHandler(this.labelHelpLink_Click);
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
            this.labelCapsLock.Location = new System.Drawing.Point(99, 157);
            this.labelCapsLock.Name = "labelCapsLock";
            this.labelCapsLock.Size = new System.Drawing.Size(86, 13);
            this.labelCapsLock.TabIndex = 24;
            this.labelCapsLock.Text = "Caps Lock is on.";
            this.labelCapsLock.Visible = false;
            // 
            // pictureBoxCapsLock
            // 
            this.pictureBoxCapsLock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBoxCapsLock.Location = new System.Drawing.Point(83, 155);
            this.pictureBoxCapsLock.Name = "pictureBoxCapsLock";
            this.pictureBoxCapsLock.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxCapsLock.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCapsLock.TabIndex = 25;
            this.pictureBoxCapsLock.TabStop = false;
            this.pictureBoxCapsLock.Visible = false;
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
            this.Controls.Add(this.pictureBoxCapsLock);
            this.Controls.Add(this.buttonShutdown);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOptions);
            this.Controls.Add(this.labelCapsLock);
            this.Controls.Add(this.labelHelpLink);
            this.Controls.Add(this.labelHelp);
            this.Controls.Add(this.labelDomain);
            this.Controls.Add(this.checkBoxDialUp);
            this.Controls.Add(this.comboBoxDomain);
            this.Controls.Add(this.labelShutdown);
            this.Controls.Add(this.pictureBoxKeySequence);
            this.Controls.Add(this.labelKeySequence);
            this.Controls.Add(this.textBoxUsername);
            this.Controls.Add(this.comboBoxUsername);
            this.Controls.Add(this.pictureBoxLoading);
            this.Controls.Add(this.labelLoading);
            this.Controls.Add(this.buttonDummy);
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.labelUsername);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.pictureBoxBanner);
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxKeySequence)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLoading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBanner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCapsLock)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBoxBanner;
		private System.Windows.Forms.TextBox textBoxPassword;
		private System.Windows.Forms.Button buttonOptions;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Label labelUsername;
		private System.Windows.Forms.Label labelPassword;
		private System.Windows.Forms.Button buttonShutdown;
		private System.Windows.Forms.Button buttonDummy;
        private System.Windows.Forms.Label labelLoading;
        private System.Windows.Forms.PictureBox pictureBoxLoading;
        private System.Windows.Forms.ComboBox comboBoxUsername;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.PictureBox pictureBoxKeySequence;
        private System.Windows.Forms.Label labelKeySequence;
        private System.Windows.Forms.Label labelShutdown;
        private System.Windows.Forms.Label labelHint;
        private System.ComponentModel.BackgroundWorker backgroundWorkerCloseOnLogon;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ComboBox comboBoxDomain;
        private System.Windows.Forms.CheckBox checkBoxDialUp;
        private System.Windows.Forms.Label labelDomain;
        private System.Windows.Forms.Label labelHelp;
        private System.Windows.Forms.Label labelHelpLink;
        private System.ComponentModel.BackgroundWorker backgroundWorkerInteractiveUpdate;
        private System.Windows.Forms.Label labelCapsLock;
        private System.Windows.Forms.PictureBox pictureBoxCapsLock;
        private System.ComponentModel.BackgroundWorker backgroundWorkerCapsLockDetect;
    }
}

