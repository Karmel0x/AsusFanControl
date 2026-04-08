using System;
using System.Windows.Forms;

namespace AsusFanControlGUI
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        static void RunForm()
        {
            System.IO.File.AppendAllText("trace.log", "Before Form1 instantiation...\n");
            var form = new Form1();
            System.IO.File.AppendAllText("trace.log", "Form1 instantiated. Running...\n");
            Application.Run(form);
            System.IO.File.AppendAllText("trace.log", "Application Run finished legally.\n");
        }

        [STAThread]
        static void Main()
        {
            try 
            {
                Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                System.IO.File.WriteAllText("trace.log", "Starting application...\n");

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += (s, e) => {
                    System.IO.File.AppendAllText("trace.log", "Thread Error: " + e.Exception.ToString() + "\n");
                    MessageBox.Show("Thread Error: " + e.Exception.ToString(), "Crash", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };
                AppDomain.CurrentDomain.UnhandledException += (s, e) => {
                    System.IO.File.AppendAllText("trace.log", "AppDomain Error: " + e.ExceptionObject.ToString() + "\n");
                    MessageBox.Show("AppDomain Error: " + e.ExceptionObject.ToString(), "Crash", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };

                RunForm();
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("trace.log", "Main Exception: " + ex.ToString() + "\n");
                MessageBox.Show("Main Exception: " + ex.ToString(), "Crash", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
