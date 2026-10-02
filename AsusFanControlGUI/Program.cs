using System;
using System.Threading;
using System.Windows.Forms;

namespace AsusFanControlGUI
{
    internal static class Program
    {
        static Mutex _singleInstance;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createdNew = true;
            try
            {
                _singleInstance = new Mutex(true, @"Local\AsusFanControlGUI", out createdNew);
            }
            catch (AbandonedMutexException)
            {
                createdNew = true;
            }
            catch (UnauthorizedAccessException)
            {
                createdNew = true;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!createdNew)
            {
                MessageBox.Show(
                    "Asus Fan Control is already running.",
                    "Asus Fan Control",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            Application.Run(new Form1());
            GC.KeepAlive(_singleInstance);
        }
    }
}
