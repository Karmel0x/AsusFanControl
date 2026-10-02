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
        readonly object _ioLock = new object();
        int _controlGeneration;

        public AsusControl()
        {
            AsusWinIO64.InitializeWinIo();
        }

        ~AsusControl()
        {
            lock (_ioLock)
                AsusWinIO64.ShutdownWinIo();
        }

        public void SetFanSpeed(byte value, byte fanIndex = 0)
        {
            lock (_ioLock)
                ApplyFanSpeed(value, fanIndex);
        }

        public void SetFanSpeed(int percent, byte fanIndex = 0)
        {
            var value = (byte)(percent / 100.0f * 255);
            SetFanSpeed(value, fanIndex);
        }

        public async void SetFanSpeeds(byte value)
        {
            int generation;
            int fanCount;
            lock (_ioLock)
            {
                generation = ++_controlGeneration;
                fanCount = AsusWinIO64.HealthyTable_FanCounts();
            }

            for (byte fanIndex = 0; fanIndex < fanCount; fanIndex++)
            {
                lock (_ioLock)
                {
                    // A newer request, including sleep releasing test mode, wins.
                    if (generation != _controlGeneration)
                        return;
                    ApplyFanSpeed(value, fanIndex);
                }
                await Task.Delay(20);
            }
        }

        public void SetFanSpeeds(int percent)
        {
            var value = (byte)(percent / 100.0f * 255);
            SetFanSpeeds(value);
        }

        /// <summary>
        /// Sets every fan immediately. Used when the machine is entering sleep so
        /// test mode is cleared before Windows suspends the process.
        /// </summary>
        public void SetFanSpeedsNow(int percent)
        {
            var value = (byte)(percent / 100.0f * 255);
            lock (_ioLock)
            {
                _controlGeneration++;
                var fanCount = AsusWinIO64.HealthyTable_FanCounts();
                for (byte fanIndex = 0; fanIndex < fanCount; fanIndex++)
                    ApplyFanSpeed(value, fanIndex);
            }
        }

        void ApplyFanSpeed(byte value, byte fanIndex)
        {
            AsusWinIO64.HealthyTable_SetFanIndex(fanIndex);
            AsusWinIO64.HealthyTable_SetFanTestMode((char)(value > 0 ? 0x01 : 0x00));
            AsusWinIO64.HealthyTable_SetFanPwmDuty(value);
        }

        public int GetFanSpeed(byte fanIndex = 0)
        {
            lock (_ioLock)
            {
                AsusWinIO64.HealthyTable_SetFanIndex(fanIndex);
                return AsusWinIO64.HealthyTable_FanRPM();
            }
        }

        public List<int> GetFanSpeeds()
        {
            var fanSpeeds = new List<int>();

            var fanCount = HealthyTable_FanCounts();
            for (byte fanIndex = 0; fanIndex < fanCount; fanIndex++)
            {
                var fanSpeed = GetFanSpeed(fanIndex);
                fanSpeeds.Add(fanSpeed);
            }

            return fanSpeeds;
        }

        public int HealthyTable_FanCounts()
        {
            lock (_ioLock)
                return AsusWinIO64.HealthyTable_FanCounts();
        }

        public ulong Thermal_Read_Cpu_Temperature()
        {
            lock (_ioLock)
                return AsusWinIO64.Thermal_Read_Cpu_Temperature();
        }

        /// <summary>
        /// Returns CPU temperature in Celsius, or -1 if invalid/unavailable.
        /// </summary>
        public int GetCpuTemperatureCelsius()
        {
            return ConvertReportedTemperature(Thermal_Read_Cpu_Temperature());
        }

        /// <summary>
        /// GPU temperature from the ASUS GPU thermal sensors, or -1 if neither
        /// sensor is present. TS1L is preferred; TS1R is used only when TS1L
        /// does not report a usable value.
        /// </summary>
        public int GetGpuTemperatureCelsius()
        {
            try
            {
                ulong leftRaw;
                ulong rightRaw;
                lock (_ioLock)
                {
                    leftRaw = AsusWinIO64.Thermal_Read_GpuTS1L_Temperature();
                    rightRaw = AsusWinIO64.Thermal_Read_GpuTS1R_Temperature();
                }

                int left = ReadGpuSensor(leftRaw);
                if (left >= 0)
                    return left;
                return ReadGpuSensor(rightRaw);
            }
            catch (EntryPointNotFoundException)
            {
                return -1;
            }
            catch (DllNotFoundException)
            {
                return -1;
            }
        }

        int ReadGpuSensor(ulong raw)
        {
            // A missing sensor comes back as 0. A laptop GPU does not sit at freezing.
            int celsius = ConvertReportedTemperature(raw & 0xFFFFFFFF);
            if (celsius <= 0)
                return -1;
            return celsius;
        }

        static int ConvertReportedTemperature(ulong raw)
        {
            if (raw > 100000 || raw == 0x7FFFFFFF) return -1;
            if (raw < 200) return (int)Math.Min(raw, 150);
            int celsius = (int)Math.Round((raw / 10.0) - 273.15);
            if (celsius < -50 || celsius > 150) return -1;
            return celsius;
        }
    }
}
