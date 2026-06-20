using FlaUI.UIA3;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace LogonShell
{
    public partial class LogOnForm : Form
	{
		#region P/Invoke
		public enum WTS_CONNECTSTATE_CLASS
		{
			WTSActive, WTSConnected, WTSConnectQuery, WTSShadow, WTSDisconnected,
			WTSIdle, WTSListen, WTSReset, WTSDown, WTSInit
		}

		public enum WTS_INFO_CLASS
		{
			WTSInitialProgram, WTSApplicationName, WTSWorkingDirectory, WTSOEMId,
			WTSSessionId, WTSUserName, WTSWinStationName, WTSDomainName, WTSConnectState,
			WTSClientBuildNumber, WTSClientName, WTSClientDirectory, WTSClientProductId,
			WTSClientHardwareId, WTSClientAddress, WTSClientDisplay, WTSClientProtocolType,
			WTSIdleTime, WTSLogonTime, WTSIncomingBytes, WTSOutgoingBytes,
			WTSIncomingFrames, WTSOutgoingFrames, WTSClientInfo, WTSSessionInfo,
			WTSSessionInfoEx, WTSConfigInfo, WTSValidationInfo, WTSSessionAddressV4,
			WTSIsRemoteSession
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct WTS_SESSION_INFO
		{
			public Int32 SessionID;
			[MarshalAs(UnmanagedType.LPStr)]
			public String pWinStationName;
			public WTS_CONNECTSTATE_CLASS State;
		}

		[DllImport("wtsapi32.dll", SetLastError = true)]
		static extern IntPtr WTSOpenServer(string pServerName);
		[DllImport("wtsapi32.dll", SetLastError = true)]
		static extern int WTSEnumerateSessions(IntPtr hServer, int Reserved, int Version, ref IntPtr ppSessionInfo, ref int pCount);
		[DllImport("wtsapi32.dll", SetLastError = true)]
		static extern bool WTSQuerySessionInformationW(IntPtr hServer, int sessionId, WTS_INFO_CLASS wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);
		[DllImport("wtsapi32.dll", SetLastError = true)]
		static extern void WTSFreeMemory(IntPtr value);
		[DllImport("wtsapi32.dll", SetLastError = true)]
		static extern int WTSConnectSession(int LogonId, int TargetLogonId, string pPassword, bool bWait);
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern int WTSGetActiveConsoleSessionId();
		[DllImport("uxtheme.dll", SetLastError = true)]
		static extern int SetWindowTheme(IntPtr window, string app, string theme);
		[DllImport("user32.dll", SetLastError = true)]
		static extern bool ExitWindowsEx(uint uFlags, uint dwReason);
		[DllImport("advapi32.dll", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool LogonUser([MarshalAs(UnmanagedType.LPStr)] string pszUserName, [MarshalAs(UnmanagedType.LPStr)] string pszDomain, [MarshalAs(UnmanagedType.LPStr)] string pszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

		readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);
		#endregion

        Control UsernameBox;
        SystemMenuManager menuManager;
		List<string> UsernameList = new List<string>();
		List<string> UserSIDList = new List<string>();

        public LogOnForm()
		{
			InitializeComponent();
			Load += Form1_Load;
			LoadUI();
			if (Config.Args.Length > 0 && Config.Args[0] == "-startup" && File.Exists(Environment.CurrentDirectory + "\\updates"))
			{
				File.Delete(Environment.CurrentDirectory + "\\updates");
				Thread.Sleep(250);
				Environment.Exit(-1);
			}
			else if (Config.Args.Length > 0 && Config.Args[0] == "-startup" && AutologinEnabled())
			{
				ShowLoading();
				backgroundWorkerCloseOnLogon.RunWorkerAsync();
			}
			else if (Config.Args.Length > 0 && Config.Args[0] == "-shutdown")
			{
				ShowLoading(true);
			}
			else
			{
				LoadUsernameBox();
				backgroundWorkerCapsLockDetect.RunWorkerAsync();
				Normalize();
				if (Config.PasswordHint.ToLower() == "always" && (Config.LoginType.ToLower() == "last" || Config.LoginType.ToLower() == "listlast"))
					GetUserPasswordHint();
			}
			Program.sw.Stop();
		}

		#region Form Events
		private void Form1_Load(object sender, EventArgs e)
		{
			if (Config.Theme.ToLower() == "classic")
				SetWindowTheme(Handle, " ", " ");
		}

		private void Form1_Shown(object sender, EventArgs e)
        {
            Location = new Point(Location.X, 166);
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
		#endregion

		#region Control Events

		private void buttonOK_Click(object sender, EventArgs e)
		{
            IntPtr user = new IntPtr();
            bool succes = LogonUser(UsernameBox.Text, Environment.MachineName, textBoxPassword.Text, 3, 0, ref user);
            if (!succes)
            {
                if (Marshal.GetLastWin32Error() == 1326)
                {
                    new MessageBoxForm(Config.LoginErrorTitle, Config.LoginErrorText, 1).ShowDialog();
                    if (Config.PasswordHint.ToLower() == "tryfirst")
                        GetUserPasswordHint();
					textBoxPassword.Text = "";
					textBoxPassword.Focus();
					return;
				}
                else
				{
					new MessageBoxForm(Config.ErrorTitle, Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()).Message, 0).ShowDialog();
					return;
				}
            }

            try
            {
                Logon.Login(UsernameBox.Text, textBoxPassword.Text, String.Compare(GetLastUserLoggedOn(), UsernameBox.Text, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols) == 0);
            }
            catch (Exception ex)
            {
                new MessageBoxForm(Config.ErrorTitle, ex.Message, 0).ShowDialog();
				Application.Exit();
			}
			ShowLoading();
			backgroundWorkerCapsLockDetect.CancelAsync();
			backgroundWorkerCloseOnLogon.RunWorkerAsync();
        }

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void buttonShutdown_Click(object sender, EventArgs e)
		{
			string shortcut = Environment.CurrentDirectory + "\\shutdown.lnk";
			if (File.Exists(shortcut))
				Process.Start(shortcut);
        }

        private void ComboBoxUsername_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetUserPasswordHint();
		}
		#endregion

		#region Get User Accounts
		private void PopulateUserList(ComboBox userComboBox)
		{
			userComboBox.Items.Clear();
			SelectQuery query = new SelectQuery("SELECT * FROM Win32_UserAccount WHERE Disabled=False");
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
			foreach (ManagementObject envVar in searcher.Get())
			{
				UsernameList.Add((string)envVar["Name"]);
				UserSIDList.Add((string)envVar["SID"]);
            }
			userComboBox.Items.AddRange(UsernameList.ToArray());
		}

		private string GetLastUserLoggedOn()
		{
			string location = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI";
			var registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
			using (var hive = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
			{
				using (var key = hive.OpenSubKey(location))
				{
					var item = key.GetValue("LastLoggedOnUser");
					string itemValue = item == null ? "No Logon Found" : item.ToString().Remove(0, 2);
					return itemValue;
				}
			}
		}

		private void GetUserPasswordHint()
		{
			try
			{
				string sid = UserSIDList.ElementAt(UsernameList.IndexOf(UsernameBox.Text));
				if (sid != null)
				{
					var rid = sid.Split('-').Last();
					var hexValue = int.Parse(rid).ToString("X").PadLeft(8, '0');
					RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SAM\SAM\Domains\Account\Users\" + hexValue);
					byte[] KeyValue = (byte[])key.GetValue("UserPasswordHint");
					string hint = System.Text.Encoding.UTF8.GetString(KeyValue, 0, KeyValue.Length).Replace("\0", "");
					if (!string.IsNullOrEmpty(hint) && hint != "null")
					{
						labelHint.Text = hint;
						labelHint.Visible = true;
					}
				}
			}
			catch (Exception)
			{
			}
		}
		#endregion

		#region Background Workers

		private void backgroundWorkerCloseOnLogon_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			Process p;
			do
			{
				p = Process.GetProcessesByName("LogonUI").FirstOrDefault();
				Thread.Sleep(100);
			} while (p != null);
			Application.Exit();
		}

		private void backgroundWorkerInteractiveUpdate_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			Thread.Sleep(200);
			Process p = Process.GetProcessesByName("LogonUI").FirstOrDefault();
			if (p != null)
			{
				var app = FlaUI.Core.Application.Attach(p);
				using (var automation = new UIA3Automation())
				{
					var lines = app.GetMainWindow(automation).FindAllDescendants(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Text));
					while (!p.HasExited && !backgroundWorkerInteractiveUpdate.CancellationPending)
					{
						if (lines.First().Name.Contains("%"))
							this.Invoke((MethodInvoker)delegate { labelShutdown.Text = lines.First().Name; });
						else if (lines.ElementAt(1).Name.Contains("%"))
							this.Invoke((MethodInvoker)delegate { labelShutdown.Text = lines.ElementAt(0).Name + " " + lines.ElementAt(1).Name; });
						else if (lines.ElementAtOrDefault(2) != null)
							this.Invoke((MethodInvoker)delegate { labelShutdown.Text = lines.ElementAt(0).Name + " " + lines.ElementAt(1).Name + " " + lines.ElementAt(2).Name; });
						else if (lines.ElementAtOrDefault(1) != null)
							this.Invoke((MethodInvoker)delegate { labelShutdown.Text = lines.ElementAt(0).Name + " " + lines.ElementAt(1).Name; });
						else
							this.Invoke((MethodInvoker)delegate { labelShutdown.Text = lines.ElementAt(0).Name; });
						if (labelShutdown.Height > 18)
							this.Invoke((MethodInvoker)delegate { this.Height = this.Height + 15; });
						Thread.Sleep(200);
						lines = app.GetMainWindow(automation).FindAllDescendants(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Text));
					}
				}
			}
		}

		private void backgroundWorkerCapsLockDetect_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			bool isCapsLockOn = false;
			while (!backgroundWorkerCapsLockDetect.CancellationPending)
            {
				Thread.Sleep(100);
				if (Control.IsKeyLocked(Keys.CapsLock))
				{
					this.Invoke((MethodInvoker)delegate { labelCapsLock.Visible = true; });
					isCapsLockOn = true;
				} else if (isCapsLockOn) {
					this.Invoke((MethodInvoker)delegate { labelCapsLock.Visible = false; });
					isCapsLockOn = false;
				}
            }
		}

		#endregion

		#region Miscellaneous

		private void HideLogin()
		{
			textBoxPassword.Visible = false;
			comboBoxUsername.Visible = false;
			textBoxUsername.Visible = false;
			var labels = this.Controls.OfType<Label>();
			foreach (var lbl in labels)
				lbl.Visible = false;
			var buttons = this.Controls.OfType<Button>();
			foreach (var btn in buttons)
				btn.Visible = false;
			this.ClientSize = new Size(409, 152);
		}

		private void ShowLoading(bool isShutdown = false)
		{
			HideLogin();
			this.Text = Config.LoadingTitle;
			if (isShutdown)
			{
				if (File.Exists(Environment.CurrentDirectory + "\\updates"))
				{
					if (Config.CloseOnUpdate)
						Application.Exit();
                    else
						backgroundWorkerInteractiveUpdate.RunWorkerAsync();
				}
				labelShutdown.Visible = true;
			}
			else
			{
				labelLoading.Visible = true;
			}
		}

		private void LoadUI()
		{
			if (Config.IsCancelEnabled)
				buttonCancel.Enabled = true;
			if (Config.ButtonXStyle.ToLower() == "enabled")
				ControlBox = true;
			else if (Config.ButtonXStyle.ToLower() == "disabled")
			{
				ControlBox = true;
                this.menuManager = new SystemMenuManager(this, SystemMenuManager.MenuItemState.Greyed);
            }
			this.Text = Config.LoginTitle;
			buttonOK.Text = Config.BtnOK;
			buttonCancel.Text = Config.BtnCancel;
			buttonShutdown.Text = Config.BtnShutdown;
			labelPassword.Text = Config.LblPassword;
			labelUsername.Text = Config.LblUsername;
			labelLoading.Text = Config.LoadingText;
			labelShutdown.Text = Config.ShuttingDownText;
			labelCapsLock.Text = Config.CapsLockNotice;
		}

		private void Normalize()
        {
			if (labelUsername.Width > 70 || labelPassword.Width > 70)
            {
				int newWidth = (labelUsername.Width > labelPassword.Width) ? labelUsername.Width : labelPassword.Width;
				newWidth = (newWidth > 142) ? 142 : newWidth;
				UsernameBox.Left = newWidth + 16;
				textBoxPassword.Left = newWidth + 16;
				labelHint.Left = newWidth + 16;
				labelCapsLock.Left = newWidth + 16;
			}
        }

		private void LoadUsernameBox()
		{
			var usernameOption = Config.LoginType.ToLower();
			switch (usernameOption)
			{
				case "simple":
					textBoxUsername.Visible = true;
					comboBoxUsername.Visible = false;
					this.ActiveControl = textBoxUsername;
					textBoxUsername.Focus();
					UsernameBox = textBoxUsername;
					break;
				case "last":
					textBoxUsername.Visible = true;
					comboBoxUsername.Visible = false;
					textBoxUsername.Text = GetLastUserLoggedOn();
					UsernameBox = textBoxUsername;
					this.ActiveControl = textBoxPassword;
					textBoxPassword.Focus();
					break;
				case "list":
					textBoxUsername.Visible = false;
					comboBoxUsername.Visible = true;
					PopulateUserList(comboBoxUsername);
					this.ActiveControl = comboBoxUsername;
                    comboBoxUsername.Focus();
					if (Config.PasswordHint.ToLower() == "always")
                        comboBoxUsername.SelectedIndexChanged += ComboBoxUsername_SelectedIndexChanged;
					UsernameBox = comboBoxUsername;
					break;
				case "listlast":
					textBoxUsername.Visible = false;
					comboBoxUsername.Visible = true;
					PopulateUserList(comboBoxUsername);
					comboBoxUsername.Text = GetLastUserLoggedOn();
					if (Config.PasswordHint.ToLower() == "always")
						comboBoxUsername.SelectedIndexChanged += ComboBoxUsername_SelectedIndexChanged;
					UsernameBox = comboBoxUsername;
					this.ActiveControl = textBoxPassword;
					textBoxPassword.Focus();
					break;
			}
		}

        private bool AutologinEnabled()
        {
            IntPtr phToken = new IntPtr();
            bool loggedIn = LogonUser(GetLastUserLoggedOn(), Environment.MachineName, "", 3, 0, ref phToken);
			bool autologon = false;
			bool displayLastUsername = true;
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon");
            if (key.GetValue("AutoAdminLogon") != null)
                autologon = (key.GetValue("AutoAdminLogon").ToString()).Contains("1");
            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System");
            if (key.GetValue("DontDisplayLastUserName") != null)
                displayLastUsername = (key.GetValue("DontDisplayLastUserName").ToString()).Contains("0");
            return (loggedIn || autologon) && displayLastUsername;
		}

		#endregion
	}
}
