using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace LogonShell
{
    static class Program
    {
        public static Stopwatch sw;

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                sw = new Stopwatch();
                sw.Start();
                Config.Initialize(args);
                if (args.Length == 0)
                    Application.Run(new BackgroundForm());
                else
                    switch (args[0])
                    {
                        case "-tray":
                            Application.Run(new UserTray());
                            break;
                        default:
                            Application.Run(new BackgroundForm());
                            break;
                    }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Environment.CurrentDirectory + "\\crash.log",
                    DateTime.Now + " " + ex.ToString());
            }
        }
    }
}
