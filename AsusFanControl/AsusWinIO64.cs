using System.Runtime.InteropServices;

namespace AsusSystemAnalysis
{
    public class AsusWinIO64
    {
        [DllImport("AsusWinIO64.dll")]
        public static extern void InitializeWinIo();
        [DllImport("AsusWinIO64.dll")]
        public static extern void ShutdownWinIo();
        [DllImport("AsusWinIO64.dll")]
        public static extern int HealthyTable_FanCounts();
        [DllImport("AsusWinIO64.dll")]
        public static extern void HealthyTable_SetFanIndex(byte index);
        [DllImport("AsusWinIO64.dll")]
        public static extern int HealthyTable_FanRPM();
        [DllImport("AsusWinIO64.dll")]
        public static extern void HealthyTable_SetFanTestMode(char mode);
        [DllImport("AsusWinIO64.dll")]
        public static extern void HealthyTable_SetFanPwmDuty(short duty);
    }
}
