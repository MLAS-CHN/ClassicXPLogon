using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace LogonShell
{
    static class Program
    {
        public static Stopwatch sw;

        [STAThread]
        static void Main(string[] args)
        {
            sw = new Stopwatch();
            sw.Start();
            Config.Initialize(args);
            if (args.Length == 0)
                Application.Run(new BackgroundForm());
            else
                switch(args[0])
                {
                    case "/shutdown":
                        Application.Run(new ShutdownBackgroundForm());
                        break;
                    case "-tray":
                        Application.Run(new UserTray());
                        break;
                    default:
                        Application.Run(new BackgroundForm());
                        break;
                }
        }
    }
}
