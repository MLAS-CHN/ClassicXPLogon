using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace LogonShell
{
    public partial class BackgroundForm : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        const int GWL_EXSTYLE = -20;
        const int WS_EX_LAYERED = 0x80000;
        const int WS_EX_TRANSPARENT = 0x20;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        LogOnForm f1;
        int Long;
		public static BackgroundForm form { get; set; }

		public BackgroundForm()
		{
			InitializeComponent();
			Rectangle screen = Screen.PrimaryScreen.Bounds;
			Size = screen.Size;
			this.Opacity = 0;
            f1 = new LogOnForm();
            Shown += BackgroundForm_Shown;
			form = this;
            if (WindowsIdentity.GetCurrent().Name == @"NT AUTHORITY\SYSTEM")
                backgroundWorker1.DoWork += backgroundWorker1_DoWork;
        }

		private void BackgroundForm_Shown(object sender, EventArgs e)
		{
			f1.Show();
            backgroundWorker1.RunWorkerAsync();
        }

		private void BackgroundForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
				e.Cancel = true;
		}

		private void BackgroundForm_Activated(object sender, EventArgs e)
		{
			f1.Activate();
		}
        public void MakeTransparent()
        {
            Long = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, Long | WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }
        public void RemoveTransparent()
        {
            SetWindowLong(this.Handle, GWL_EXSTYLE, Long);
        }
        public void StopWorker()
        {
            backgroundWorker1.CancelAsync();
        }
        public void StartWorker()
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (!backgroundWorker1.CancellationPending)
            {
                Thread.Sleep(50);
                foreach (Process p in Process.GetProcesses())
                {
                    if (p.ProcessName == "LockApp" || p.ProcessName == "LockScreen" || p.ProcessName == "LockAppHost" || p.ProcessName == "LockScreenContentServer") 
                        p.Kill();
                }
            }
        }
    }
}
