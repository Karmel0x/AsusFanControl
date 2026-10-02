using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace AsusFanControlGUI
{
    /// <summary>
    /// Registers the GUI to launch at logon. The app requires administrator, so the
    /// preferred path is a per-user scheduled task with highest privileges. If Task
    /// Scheduler refuses the task, the current user's Run key is used instead.
    /// </summary>
    static class WindowsStartup
    {
        public const string TaskName = "AsusFanControlGUI";
        const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        const string RunValueName = "AsusFanControlGUI";

        public static bool Enable(out string error)
        {
            error = null;

            string exe = Application.ExecutablePath;
            if (string.IsNullOrEmpty(exe) || !File.Exists(exe))
            {
                error = "Could not find AsusFanControlGUI.exe.";
                return false;
            }

            // schtasks keeps the quotes around the executable as part of /TR.
            string arguments = "/Create /F /SC ONLOGON /RL HIGHEST /TN \"" + TaskName
                + "\" /TR \"\\\"" + exe + "\\\"\"";
            if (RunSchtasks(arguments, out string output))
            {
                // The task replaces a Run-key entry so the app does not start twice.
                RemoveRunValue();
                return true;
            }

            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RunKeyPath))
                {
                    if (key == null)
                    {
                        error = string.IsNullOrWhiteSpace(output)
                            ? "Could not open the startup registry key."
                            : output;
                        return false;
                    }
                    key.SetValue(RunValueName, "\"" + exe + "\"");
                }
                return true;
            }
            catch (Exception ex)
            {
                error = string.IsNullOrWhiteSpace(output) ? ex.Message : output;
                return false;
            }
        }

        public static void Disable()
        {
            RunSchtasks("/Delete /F /TN \"" + TaskName + "\"", out _);
            RemoveRunValue();
        }

        static void RemoveRunValue()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
                {
                    if (key != null)
                        key.DeleteValue(RunValueName, false);
                }
            }
            catch (Exception)
            {
            }
        }

        static bool RunSchtasks(string arguments, out string output)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "schtasks.exe",
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        output = "Could not start schtasks.exe.";
                        return false;
                    }

                    var stdoutTask = process.StandardOutput.ReadToEndAsync();
                    var stderrTask = process.StandardError.ReadToEndAsync();
                    if (!process.WaitForExit(8000))
                    {
                        try { process.Kill(); }
                        catch (Exception) { }
                        output = "Timed out while talking to Task Scheduler.";
                        return false;
                    }

                    output = (stdoutTask.Result + "\n" + stderrTask.Result).Trim();
                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                output = ex.Message;
                return false;
            }
        }
    }
}
