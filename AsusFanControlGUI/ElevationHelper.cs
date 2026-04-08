using System.Security.Principal;
using System.Drawing;
using System.Windows.Forms;

namespace AsusFanControlGUI
{
    public static class ElevationHelper
    {
        public static bool IsRunningAsSystem()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                return identity.IsSystem;
            }
        }

        public static void ShowElevationWarning(Label statusLabel)
        {
            if (!IsRunningAsSystem())
            {
                statusLabel.ForeColor = DarkTheme.WarningOrange;
                statusLabel.Text = "Warning: Not running as SYSTEM. Fan control may fail on driver v3.1.41.0+. Use run.bat.";
            }
            else
            {
                statusLabel.ForeColor = DarkTheme.SuccessGreen;
                statusLabel.Text = "Status: Ready (running as SYSTEM)";
            }
        }
    }
}
