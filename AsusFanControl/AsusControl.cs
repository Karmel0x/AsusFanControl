using AsusSystemAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsusFanControl
{
    public class FanCurvePoint
    {
        public int Temperature { get; set; }
        public int FanSpeed { get; set; }
        
        public FanCurvePoint(int temperature, int fanSpeed)
        {
            Temperature = temperature;
            FanSpeed = fanSpeed;
        }
    }

    public class FanCurve
    {
        public List<FanCurvePoint> Points { get; set; }
        public bool IsEnabled { get; set; }
        
        public FanCurve()
        {
            Points = new List<FanCurvePoint>
            {
                new FanCurvePoint(30, 30),
                new FanCurvePoint(50, 50),
                new FanCurvePoint(70, 80),
                new FanCurvePoint(90, 100)
            };
            IsEnabled = false;
        }
        
        public int GetFanSpeedForTemperature(int temperature)
        {
            if (!IsEnabled || Points.Count == 0)
                return 0;
                
            Points.Sort((a, b) => a.Temperature.CompareTo(b.Temperature));
            
            if (temperature <= Points[0].Temperature)
                return Points[0].FanSpeed;
                
            if (temperature >= Points[Points.Count - 1].Temperature)
                return Points[Points.Count - 1].FanSpeed;
                
            for (int i = 0; i < Points.Count - 1; i++)
            {
                if (temperature >= Points[i].Temperature && temperature <= Points[i + 1].Temperature)
                {
                    double ratio = (double)(temperature - Points[i].Temperature) / 
                                   (Points[i + 1].Temperature - Points[i].Temperature);
                    return (int)(Points[i].FanSpeed + ratio * (Points[i + 1].FanSpeed - Points[i].FanSpeed));
                }
            }
            
            return 0;
        }
        
        public string SerializeToString()
        {
            Points.Sort((a, b) => a.Temperature.CompareTo(b.Temperature));
            return string.Join(";", Points.Select(p => $"{p.Temperature},{p.FanSpeed}"));
        }
        
        public static FanCurve DeserializeFromString(string data)
        {
            var curve = new FanCurve();
            curve.Points.Clear();
            
            if (string.IsNullOrEmpty(data))
                return curve;
                
            try
            {
                var points = data.Split(';');
                foreach (var point in points)
                {
                    var parts = point.Split(',');
                    if (parts.Length == 2 && 
                        int.TryParse(parts[0], out int temp) && 
                        int.TryParse(parts[1], out int speed))
                    {
                        curve.Points.Add(new FanCurvePoint(temp, speed));
                    }
                }
            }
            catch
            {
                curve.Points.Clear();
                curve.Points.Add(new FanCurvePoint(30, 30));
                curve.Points.Add(new FanCurvePoint(50, 50));
                curve.Points.Add(new FanCurvePoint(70, 80));
                curve.Points.Add(new FanCurvePoint(90, 100));
            }
            
            if (curve.Points.Count == 0)
            {
                curve.Points.Add(new FanCurvePoint(30, 30));
                curve.Points.Add(new FanCurvePoint(90, 100));
            }
            
            return curve;
        }
    }

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
            AsusWinIO64.HealthyTable_SetFanTestMode((char)(value > 0 ? 0x01 : 0x00));
            AsusWinIO64.HealthyTable_SetFanPwmDuty(value);
        }

        public void SetFanSpeed(int percent, byte fanIndex = 0)
        {
            var value = (byte)(percent / 100.0f * 255);
            SetFanSpeed(value, fanIndex);
        }

        public async void SetFanSpeeds(byte value)
        {
            var fanCount = AsusWinIO64.HealthyTable_FanCounts();
            for(byte fanIndex = 0; fanIndex < fanCount; fanIndex++)
            {
                SetFanSpeed(value, fanIndex);
                await Task.Delay(20);
            }
        }

        public void SetFanSpeeds(int percent)
        {
            var value = (byte)(percent / 100.0f * 255);
            SetFanSpeeds(value);
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

        public int HealthyTable_FanCounts()
        {
            return AsusWinIO64.HealthyTable_FanCounts();
        }

        public ulong Thermal_Read_Cpu_Temperature()
        {
            return AsusWinIO64.Thermal_Read_Cpu_Temperature();
        }
    }
}
