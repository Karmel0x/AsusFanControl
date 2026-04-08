using System.Collections.Generic;
using AsusFanControlGUI.Models;

namespace AsusFanControlGUI.Services
{
    public static class BuiltInProfiles
    {
        public static readonly FanCurveProfile Turbo = new FanCurveProfile
        {
            Name = "Turbo",
            IsBuiltIn = true,
            CpuCurve = new List<CurvePoint>
            {
                new CurvePoint(30, 2500),
                new CurvePoint(45, 3800),
                new CurvePoint(50, 4300),
                new CurvePoint(60, 4800),
                new CurvePoint(70, 5200),
                new CurvePoint(80, 5600),
                new CurvePoint(90, 5800),
                new CurvePoint(100, 6100),
            },
            GpuCurve = new List<CurvePoint>
            {
                new CurvePoint(30, 2800),
                new CurvePoint(45, 4000),
                new CurvePoint(50, 4600),
                new CurvePoint(60, 4300),
                new CurvePoint(65, 5200),
                new CurvePoint(70, 5300),
                new CurvePoint(80, 5800),
                new CurvePoint(100, 6400),
            }
        };

        public static readonly FanCurveProfile Balanced = new FanCurveProfile
        {
            Name = "Balanced",
            IsBuiltIn = true,
            CpuCurve = new List<CurvePoint>
            {
                new CurvePoint(30,    0),
                new CurvePoint(60, 2200),
                new CurvePoint(65, 2600),
                new CurvePoint(70, 3300),
                new CurvePoint(75, 4200),
                new CurvePoint(80, 4300),
                new CurvePoint(100, 4900),
            },
            GpuCurve = new List<CurvePoint>
            {
                new CurvePoint(30,    0),
                new CurvePoint(60, 2200),
                new CurvePoint(65, 2800),
                new CurvePoint(70, 3800),
                new CurvePoint(80, 4200),
                new CurvePoint(90, 4300),
                new CurvePoint(100, 5200),
            }
        };

        public static readonly FanCurveProfile Silent = new FanCurveProfile
        {
            Name = "Silent",
            IsBuiltIn = true,
            CpuCurve = new List<CurvePoint>
            {
                new CurvePoint(30,    0),
                new CurvePoint(60,    0),
                new CurvePoint(70, 1800),
                new CurvePoint(80, 2800),
                new CurvePoint(90, 3600),
                new CurvePoint(100, 4200),
            },
            GpuCurve = new List<CurvePoint>
            {
                new CurvePoint(30,    0),
                new CurvePoint(60,    0),
                new CurvePoint(70, 1800),
                new CurvePoint(80, 2800),
                new CurvePoint(90, 3600),
                new CurvePoint(100, 4200),
            }
        };

        public static List<FanCurveProfile> All => new List<FanCurveProfile> { Turbo, Balanced, Silent };
    }
}
