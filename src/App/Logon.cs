using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;

namespace LogonShell
{
    internal class Logon
    {
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);
        [DllImport("user32.dll")]
        static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        static void Log(string msg) { File.AppendAllText(Environment.CurrentDirectory + "\\login.log", DateTime.Now.ToString("HH:mm:ss.fff") + " [Logon] " + msg + "\n"); }

        public static void Login(string user, string password, bool IsLastUser)
        {
            Process p = null;
            var procs = Process.GetProcessesByName("LogonUI");
            Log("Found " + procs.Length + " LogonUI processes");
            foreach (var proc in procs)
            {
                Log("  PID=" + proc.Id + " MainWindowHandle=" + proc.MainWindowHandle);
                if (proc.MainWindowHandle != IntPtr.Zero)
                    p = proc;
            }
            if (p == null && procs.Length > 0)
            {
                Log("No LogonUI with valid handle, waiting...");
                for (int w = 0; w < 50; w++)
                {
                    Thread.Sleep(100);
                    procs = Process.GetProcessesByName("LogonUI");
                    foreach (var proc in procs)
                    {
                        if (proc.MainWindowHandle != IntPtr.Zero)
                        { p = proc; break; }
                    }
                    if (p != null) break;
                }
            }
            Log("Selected: " + (p != null ? "PID=" + p.Id + " handle=" + p.MainWindowHandle : "NONE"));
            if (p != null)
            {
                BackgroundForm.form.StopWorker();
                string pwd = new string(password.Where(c => !char.IsControl(c)).ToArray());
                Log("Attaching FlaUI to PID=" + p.Id);
                var app = FlaUI.Core.Application.Attach(p);
                Log("Attached, handle=" + app.MainWindowHandle);
                using (var automation = new UIA3Automation())
                {
                    for (int attempt = 0; attempt < 2; attempt++)
                    {
                        Log("--- Attempt " + (attempt + 1) + " ---");
                        try
                        {
                            if (p.HasExited) { Log("Process already exited, breaking"); break; }
                        }
                        catch { Log("HasExited threw, breaking"); break; }

                        IntPtr h = app.MainWindowHandle;
                        Log("MainWindowHandle=" + h);
                        SetForegroundWindow(h);
                        Log("SetForegroundWindow done, sending BACKSPACE");
                        SendKeys.SendWait("{BACKSPACE}");
                        Thread.Sleep(100);
                        Log("Getting main window via FlaUI");
                        Window window = app.GetMainWindow(automation);
                        Log("Got window: " + window.Title);
                        SetForegroundWindow(h);
                        var edits = window.FindAllDescendants(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit));
                        bool dontShowLastUser = edits.Count() > 1;
                        Log("Edit controls: " + edits.Count() + " dontShowLastUser=" + dontShowLastUser + " IsLastUser=" + IsLastUser);
                        if (!IsLastUser)
                        {
                            if (dontShowLastUser)
                                throw new Exception("Mode not supported!");
                            Log("Searching for user element: " + user);
                            var items = window.FindAllDescendants();
                            Log("Total descendants: " + items.Count());
                            foreach (var item in items)
                            {
                                try
                                {
                                    if (item.Name.Contains(user))
                                    {
                                        Log("Found element: " + item.Name + " clicking...");
                                        BackgroundForm.form.MakeTransparent();
                                        item.Click();
                                        BackgroundForm.form.RemoveTransparent();
                                        Log("Clicked OK");
                                    }
                                    Thread.Sleep(200);
                                }
                                catch (Exception ex) { Log("Click error: " + ex.Message); }
                            }
                        }
                        AutomationElement[] textboxes = window.FindAllDescendants(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit));
                        Log("Textboxes found: " + textboxes.Count());
                        if (textboxes.Count() > 0)
                        {
                            if (dontShowLastUser)
                            {
                                if (textboxes.First().Name.ToLower() != "password")
                                    textboxes.First().Focus();
                                else
                                    textboxes.Last().Focus();
                                Log("Sending username: " + user);
                                SendKeys.SendWait(user);
                            }
                            if (!string.IsNullOrEmpty(pwd))
                            {
                                if (dontShowLastUser && textboxes.First().Name.ToLower() != "password")
                                    textboxes.Last().Focus();
                                else
                                    textboxes.First().Focus();
                                Log("Sending password (length=" + pwd.Length + ")");
                                SendKeys.SendWait(pwd);
                            }
                        }
                        Log("Sending ENTER");
                        SendKeys.SendWait("{ENTER}");
                        Log("Attempt " + (attempt + 1) + " complete");
                    }
                }
                Log("Login loop finished");
                BackgroundForm.form.StartWorker();
            }
            else
                throw new Exception("LogonUI not found!");
        }
    }
}
