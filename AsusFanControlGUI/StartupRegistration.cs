using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace AsusFanControlGUI
{
    internal static class StartupRegistration
    {
        private const string LegacyRunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string LegacyValueName = "AsusFanControl";
        private const string TaskName = "AsusFanControl";
        private const string LauncherFileName = "run.bat";
        private const string PsExecFileName = "PsExec.exe";
        private const string StartMinimizedArgument = "--minimized";

        public static bool IsEnabled()
        {
            return HasScheduledTask() || HasLegacyRunKeyEntry();
        }

        public static void SetEnabled(bool enabled)
        {
            if (enabled)
                CreateOrUpdateScheduledTask();
            else
                RemoveScheduledTask();

            RemoveLegacyRunKeyEntry();
        }

        public static void RefreshCurrentExecutablePath()
        {
            if (!IsEnabled())
                return;

            CreateOrUpdateScheduledTask();
            RemoveLegacyRunKeyEntry();
        }

        public static bool CanUseSystemLauncher(out string errorMessage)
        {
            var launcherPath = GetLauncherPath();
            if (!File.Exists(launcherPath))
            {
                errorMessage = string.Format("{0} was not found next to AsusFanControlGUI.exe.", LauncherFileName);
                return false;
            }

            if (ResolvePsExecPath() == null)
            {
                errorMessage = string.Format("{0} was not found next to the app or in %ProgramFiles%\\ASUS\\ASUS Fan Control.", PsExecFileName);
                return false;
            }

            errorMessage = null;
            return true;
        }

        public static string GetLaunchRequirementMessage()
        {
            return "This machine only responds when AsusFanControlGUI.exe is started through run.bat, which elevates and then uses PsExec to launch the GUI as SYSTEM. Launch the app through run.bat, not by opening AsusFanControlGUI.exe directly.";
        }

        private static bool HasScheduledTask()
        {
            return RunSchtasks(string.Format("/Query /TN \"{0}\"", TaskName)).ExitCode == 0;
        }

        private static void CreateOrUpdateScheduledTask()
        {
            if (!CanUseSystemLauncher(out var errorMessage))
                throw new InvalidOperationException(errorMessage);

            var result = RunPowerShell(BuildCreateScheduledTaskScript());
            if (result.ExitCode != 0)
                throw new InvalidOperationException(string.Format("Unable to create the Windows sign-in task. {0}", result.Output).Trim());
        }

        private static void RemoveScheduledTask()
        {
            var result = RunSchtasks(string.Format("/Delete /TN \"{0}\" /F", TaskName));
            if (result.ExitCode != 0 && !ContainsTaskNotFoundMessage(result.Output))
                throw new InvalidOperationException(string.Format("Unable to remove the Windows sign-in task. {0}", result.Output).Trim());
        }

        private static string BuildTaskCommandLine(string launcherPath)
        {
            return string.Format("\"{0}\" {1}", launcherPath, StartMinimizedArgument);
        }

        private static string BuildCreateScheduledTaskScript()
        {
            var targetUserName = EscapeForPowerShellSingleQuotedLiteral(GetTargetUserName());
            var actionArguments = EscapeForPowerShellSingleQuotedLiteral(string.Format("/c \"\"{0}\"\" {1}", GetLauncherPath(), StartMinimizedArgument));
            var taskName = EscapeForPowerShellSingleQuotedLiteral(TaskName);

            return string.Join("; ", new[]
            {
                string.Format("$action = New-ScheduledTaskAction -Execute 'cmd.exe' -Argument '{0}'", actionArguments),
                string.Format("$trigger = New-ScheduledTaskTrigger -AtLogOn -User '{0}'", targetUserName),
                "$trigger.Delay = 'PT30S'",
                string.Format("$principal = New-ScheduledTaskPrincipal -UserId '{0}' -LogonType Interactive -RunLevel Highest", targetUserName),
                "$settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries -StartWhenAvailable",
                string.Format("Register-ScheduledTask -TaskName '{0}' -Action $action -Trigger $trigger -Principal $principal -Settings $settings -Force | Out-Null", taskName)
            });
        }

        private static string EscapeForPowerShellSingleQuotedLiteral(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }

        private static string GetLauncherPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LauncherFileName);
        }

        private static string ResolvePsExecPath()
        {
            var localPsExecPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PsExecFileName);
            if (File.Exists(localPsExecPath))
                return localPsExecPath;

            var installedPsExecPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "ASUS",
                "ASUS Fan Control",
                PsExecFileName);
            if (File.Exists(installedPsExecPath))
                return installedPsExecPath;

            return null;
        }

        private static string GetTargetUserName()
        {
            var currentUserName = WindowsIdentity.GetCurrent().Name;
            if (!string.Equals(currentUserName, "NT AUTHORITY\\SYSTEM", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(currentUserName, "SYSTEM", StringComparison.OrdinalIgnoreCase))
            {
                return currentUserName;
            }

            var interactiveUserName = GetActiveConsoleUserName();
            if (!string.IsNullOrWhiteSpace(interactiveUserName))
                return interactiveUserName;

            throw new InvalidOperationException("Unable to determine the interactive Windows user for startup registration.");
        }

        private static bool HasLegacyRunKeyEntry()
        {
            return !string.IsNullOrWhiteSpace(ReadLegacyRunKeyValue());
        }

        private static void RemoveLegacyRunKeyEntry()
        {
            var runKeyPath = GetInteractiveUserRegistryPath();
            if (string.IsNullOrWhiteSpace(runKeyPath))
                return;

            using (var runKey = Microsoft.Win32.Registry.Users.OpenSubKey(runKeyPath, writable: true))
            {
                runKey?.DeleteValue(LegacyValueName, throwOnMissingValue: false);
            }
        }

        private static string ReadLegacyRunKeyValue()
        {
            var runKeyPath = GetInteractiveUserRegistryPath();
            if (string.IsNullOrWhiteSpace(runKeyPath))
                return null;

            using (var runKey = Microsoft.Win32.Registry.Users.OpenSubKey(runKeyPath, writable: false))
            {
                return runKey?.GetValue(LegacyValueName) as string;
            }
        }

        private static string GetInteractiveUserRegistryPath()
        {
            try
            {
                var sid = GetTargetUserSid();
                if (string.IsNullOrWhiteSpace(sid))
                    return null;

                return string.Format(@"{0}\{1}", sid, LegacyRunKeyPath);
            }
            catch
            {
                return null;
            }
        }

        private static string GetTargetUserSid()
        {
            var targetUserName = GetTargetUserName();
            var ntAccount = new NTAccount(targetUserName);
            var securityIdentifier = (SecurityIdentifier)ntAccount.Translate(typeof(SecurityIdentifier));
            return securityIdentifier.Value;
        }

        private static bool ContainsTaskNotFoundMessage(string output)
        {
            return output.IndexOf("cannot find", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   output.IndexOf("cannot find the file specified", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   output.IndexOf("does not exist", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static SchtasksResult RunSchtasks(string arguments)
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo()
                {
                    FileName = "schtasks.exe",
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };

                process.Start();
                var standardOutput = process.StandardOutput.ReadToEnd();
                var standardError = process.StandardError.ReadToEnd();
                process.WaitForExit();

                return new SchtasksResult(process.ExitCode, (standardOutput + Environment.NewLine + standardError).Trim());
            }
        }

        private static SchtasksResult RunPowerShell(string command)
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    Arguments = string.Format("-NoProfile -NonInteractive -Command \"{0}\"", command.Replace("\"", "\\\"")),
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };

                process.Start();
                var standardOutput = process.StandardOutput.ReadToEnd();
                var standardError = process.StandardError.ReadToEnd();
                process.WaitForExit();

                return new SchtasksResult(process.ExitCode, (standardOutput + Environment.NewLine + standardError).Trim());
            }
        }

        private static string GetActiveConsoleUserName()
        {
            var sessionId = WTSGetActiveConsoleSessionId();
            var userName = QuerySessionInformation((int)sessionId, WtsInfoClass.WTSUserName);
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            var domainName = QuerySessionInformation((int)sessionId, WtsInfoClass.WTSDomainName);
            if (string.IsNullOrWhiteSpace(domainName))
                return userName;

            return string.Format("{0}\\{1}", domainName, userName);
        }

        private static string QuerySessionInformation(int sessionId, WtsInfoClass infoClass)
        {
            IntPtr buffer = IntPtr.Zero;

            try
            {
                if (!WTSQuerySessionInformation(IntPtr.Zero, sessionId, infoClass, out buffer, out var bytesReturned) || buffer == IntPtr.Zero || bytesReturned <= 1)
                    return null;

                return Marshal.PtrToStringAuto(buffer);
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                    WTSFreeMemory(buffer);
            }
        }

        [DllImport("kernel32.dll")]
        private static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("Wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WtsInfoClass wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);

        [DllImport("Wtsapi32.dll")]
        private static extern void WTSFreeMemory(IntPtr pMemory);

        private enum WtsInfoClass
        {
            WTSUserName = 5,
            WTSDomainName = 7,
        }

        private sealed class SchtasksResult
        {
            public SchtasksResult(int exitCode, string output)
            {
                ExitCode = exitCode;
                Output = output ?? string.Empty;
            }

            public int ExitCode { get; }

            public string Output { get; }
        }
    }
}