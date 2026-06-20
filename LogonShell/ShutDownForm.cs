using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LogonShell
{
	public partial class ShutDownForm : Form
	{
		[DllImport("uxtheme.dll", SetLastError = true)]
		static extern int SetWindowTheme(IntPtr window, string app, string theme);

		string[] actionDescriptions;
		int selectedIndex;

        public ShutDownForm(int shutdownType = 0)
		{
			InitializeComponent();
			this.Text = Config.ShutdownTitle;
			pictureBoxBanner.Visible = false;
			pictureBoxLoading.Visible = false;
			comboBox1.Items.Clear();
			comboBox1.Items.Add(Config.ShutdownName);
			comboBox1.Items.Add(Config.RestartName);
			comboBox1.Items.Add(Config.StandbyName);
			actionDescriptions = new string[] { Config.ShutdownDesc, Config.RestartDesc, Config.StandbyDesc };
			labelWdyw.Text = Config.ShutdownQuestion;
			buttonOK.Text = Config.BtnOK;
			buttonCancel.Text = Config.BtnCancel;
			buttonHelp.Text = Config.BtnHelp;
			selectedIndex = shutdownType;
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboBox1.SelectedIndex != -1)
				labelDescription.Text = actionDescriptions[comboBox1.SelectedIndex];
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void ShutDownForm_Load(object sender, EventArgs e)
		{
			comboBox1.SelectedIndex = selectedIndex;
			if (Config.Theme.ToLower() == "classic")
				SetWindowTheme(Handle, " ", " ");
		}
		private void buttonOK_Click(object sender, EventArgs e)
		{
			ProcessStartInfo psi = null;
			var LoadAnim = true;
			switch (comboBox1.SelectedIndex)
			{
				case 0:
					psi = new ProcessStartInfo("shutdown", "/s /t 0");
					break;
				case 1:
					psi = new ProcessStartInfo("shutdown", "/r /t 0");
					break;
				case 2:
					Application.SetSuspendState(PowerState.Suspend, true, true);
					LoadAnim = false;
					break;
				default:
					LoadAnim = false;
					break;
			}
			if (psi != null)
			{
				psi.CreateNoWindow = true;
				psi.UseShellExecute = false;
				Process.Start(psi);
			}
			if (LoadAnim)
				this.DialogResult = DialogResult.OK;
			this.Close();
			return;
		}

        private void buttonHelp_Click(object sender, EventArgs e)
        {
        }
    }
}
