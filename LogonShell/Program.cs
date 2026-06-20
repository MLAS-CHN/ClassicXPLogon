using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace LogonShell
{
    static class Program
    {
        public static Stopwatch sw;

        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "-tray")
            {
                Config.Initialize(args);
                Application.Run(new UserTray());
                return;
            }

            bool created;
            using (var mutex = new Mutex(true, "ClassicLogonShell_SingleInstance", out created))
            {
                if (!created) return;
                try
                {
                    sw = new Stopwatch();
                    sw.Start();
                    Config.Initialize(args);
                    Application.Run(new BackgroundForm());
                }
                catch (Exception ex)
                {
                    File.WriteAllText(Environment.CurrentDirectory + "\\crash.log",
                        DateTime.Now + " " + ex.ToString());
                }
            }
        }
    }
}
