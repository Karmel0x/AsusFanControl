using System;
using System.Windows.Forms;
using AsusFanControl;

namespace AsusFanControlGUI
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            args = args ?? new string[0];

            bool isPsExecRelaunch = false;
            foreach (var a in args)
                if (string.Equals(a, SystemRelaunch.PsExecRelaunchFlag, StringComparison.OrdinalIgnoreCase))
                    isPsExecRelaunch = true;

            if (!isPsExecRelaunch && Environment.UserInteractive && !SystemRelaunch.IsRunningAsSystem() && !SystemRelaunch.CanAccessFanDriver())
            {
                if (SystemRelaunch.TryRelaunchAsSystem(args))
                    return;

                MessageBox.Show(
                    "Fan driver access requires running as SYSTEM (LocalSystem) on some newer ASUS drivers.\n\n" +
                    "Place PsExec64.exe / PsExec.exe next to AsusFanControlGUI.exe to enable auto-relaunch.",
                    "Asus Fan Control",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
