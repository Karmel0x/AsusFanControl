using AsusSystemAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsusFanControl
{
    public class AsusControl
    {
        public AsusControl()
        {
            AsusWinIO64.InitializeWinIo();
        }

        ~AsusControl()
        {
            AsusWinIO64.ShutdownWinIo();
        }

        public void SetFanSpeed(byte value, byte fanIndex = 0)
        {
            AsusWinIO64.HealthyTable_SetFanIndex(fanIndex);
            AsusWinIO64.HealthyTable_SetFanPwmDuty(value);
            AsusWinIO64.HealthyTable_SetFanTestMode((char)(value > 0 ? 0x01 : 0x00));
        }

        public void SetFanSpeed(int percent)
        {
            var value = (byte)(percent / 100.0f * 255);
            SetFanSpeed(value);
        }

        public void SetFansSpeed(byte value)
        {
            var fanCount = AsusWinIO64.HealthyTable_FanCounts();
            for(byte fanIndex = 0; fanIndex < fanCount; fanIndex++)
            {
                SetFanSpeed(value, fanIndex);
            }
        }

        public void SetFansSpeed(int percent)
        {
            var value = (byte)(percent / 100.0f * 255);
            SetFansSpeed(value);
        }

        public int GetFanSpeed(byte fanIndex = 0)
        {
            AsusWinIO64.HealthyTable_SetFanIndex(fanIndex);
            var fanSpeed = AsusWinIO64.HealthyTable_FanRPM();
            return fanSpeed;
        }

        public List<int> GetFanSpeeds()
        {
            var fanSpeeds = new List<int>();

            var fanCount = AsusWinIO64.HealthyTable_FanCounts();
            for (byte fanIndex = 0; fanIndex < fanCount; fanIndex++)
            {
                var fanSpeed = GetFanSpeed(fanIndex);
                fanSpeeds.Add(fanSpeed);
            }

            return fanSpeeds;
        }
    }
}
