using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using AsusSystemAnalysis;

namespace AsusFanControl
{
    public static class SystemRelaunch
    {
        public const string PsExecRelaunchFlag = "--psexec-relaunch";
        public const string AutoRelaunchFlag = "--relaunch-system";

        public static bool IsRunningAsSystem()
        {
            try
            {
                using (var identity = WindowsIdentity.GetCurrent())
                {
                    return identity != null && identity.IsSystem;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool CanAccessFanDriver()
        {
            try
            {
                var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AsusWinIO64.dll");
                if (!File.Exists(dllPath))
                    return false;

                AsusWinIO64.InitializeWinIo();
                var fanCount = AsusWinIO64.HealthyTable_FanCounts();
                AsusWinIO64.ShutdownWinIo();

                return fanCount > 0;
            }
            catch
            {
                try { AsusWinIO64.ShutdownWinIo(); } catch { }
                return false;
            }
        }

        public static string FindPsExecPath()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var candidates = new[]
            {
                "PsExec64.exe",
                "PsExec.exe",
                "psexec64.exe",
                "psexec.exe",
                "PsExec64",
                "PsExec",
                "psexec64",
                "psexec"
            };

            foreach (var candidate in candidates)
            {
                var path = Path.Combine(baseDir, candidate);
                if (File.Exists(path)) return path;
            }

            return null;
        }

        public static bool TryRelaunchAsSystem(string[] args)
        {
            if (!Environment.UserInteractive) return false;
            if (IsRunningAsSystem()) return false;

            if (args != null && args.Any(a => string.Equals(a, PsExecRelaunchFlag, StringComparison.OrdinalIgnoreCase)))
                return false;

            var psExecPath = FindPsExecPath();
            if (psExecPath == null) return false;

            string exePath;
            try { exePath = Process.GetCurrentProcess().MainModule.FileName; }
            catch { return false; }

            if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
                return false;

            try
            {
                var forwardArgs = (args ?? new string[0])
                    .Where(a => !string.Equals(a, PsExecRelaunchFlag, StringComparison.OrdinalIgnoreCase))
                    .Concat(new[] { PsExecRelaunchFlag })
                    .ToArray();

                var forwarded = string.Join(" ", forwardArgs.Select(QuoteArg));
                var sessionId = Process.GetCurrentProcess().SessionId;

                var psi = new ProcessStartInfo
                {
                    FileName = psExecPath,
                    Arguments = string.Format("-accepteula -d -s -i {0} \"{1}\" {2}", sessionId, exePath, forwarded),
                    UseShellExecute = true,
                    Verb = "runas",
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                };

                Process.Start(psi);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string QuoteArg(string arg)
        {
            if (string.IsNullOrEmpty(arg)) return "\"\"";
            if (!arg.Contains(" ") && !arg.Contains("\"")) return arg;
            return "\"" + arg.Replace("\"", "\\\"") + "\"";
        }
    }
}
