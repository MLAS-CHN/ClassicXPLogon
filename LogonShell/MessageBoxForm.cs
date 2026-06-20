using System;
using System.Drawing;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LogonShell
{
    public partial class MessageBoxForm : Form
	{
        [DllImport("uxtheme.dll", SetLastError = true)]
		static extern int SetWindowTheme(IntPtr window, string app, string theme);

        public MessageBoxForm(string Title, string Message, int icon)
		{
			InitializeComponent();
			this.Text = Title;
			labelMessage.MaximumSize = new Size(400, 56);
			labelMessage.Text = Message;
			if (labelMessage.Height < 18)
				labelMessage.Location = new Point(55, 21);
			this.Size = new Size(83 + labelMessage.Width, (labelMessage.Height + 89 > 118)? labelMessage.Height + 89: 118 );
			buttonOK.Location = new Point((this.Width - 75) / 2, this.Height - 70);
			buttonOK.Text = Config.BtnOK;

			if (icon == 0)
                pictureBoxIcon.Image = SystemIcons.Error.ToBitmap();
            else
                pictureBoxIcon.Image = SystemIcons.Warning.ToBitmap();
        }
        private void ShutDownForm_Load(object sender, EventArgs e)
		{
			if (Config.Theme.ToLower() == "classic")
				SetWindowTheme(Handle, " ", " ");
			SystemSounds.Hand.Play();
        }
        private void buttonOK_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
