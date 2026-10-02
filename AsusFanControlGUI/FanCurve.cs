using System;

namespace AsusFanControlGUI
{
    /// <summary>
    /// One step on a fan curve: at this temperature, this fan percent turns on.
    /// </summary>
    public struct FanSetpoint
    {
        public int TemperatureC;
        public int FanPercent;

        public FanSetpoint(int temperatureC, int fanPercent)
        {
            TemperatureC = temperatureC;
            FanPercent = fanPercent;
        }
    }

    /// <summary>
    /// Step curve used by Auto mode. Below the first setpoint the fans stay off.
    /// From each setpoint up to the next, that setpoint's percent is held.
    /// Defaults match the previous fixed ranges: 35°C→45%, 55°C→80%, 75°C→100%.
    /// </summary>
    public static class FanCurve
    {
        public const int MinSetpoints = 2;
        public const int MaxSetpoints = 3;
        public const int MinTemperatureC = 0;
        public const int MaxTemperatureC = 110;
        public const int MinFanPercent = 0;
        public const int MaxFanPercent = 100;

        public static int ClampCount(int count)
        {
            return count == MinSetpoints ? MinSetpoints : MaxSetpoints;
        }

        public static int ClampTemperature(int temperatureC)
        {
            if (temperatureC < MinTemperatureC) return MinTemperatureC;
            if (temperatureC > MaxTemperatureC) return MaxTemperatureC;
            return temperatureC;
        }

        public static int ClampPercent(int percent)
        {
            if (percent < MinFanPercent) return MinFanPercent;
            if (percent > MaxFanPercent) return MaxFanPercent;
            return percent;
        }

        /// <summary>
        /// Makes the first <paramref name="count"/> temperatures strictly increasing
        /// and clamps each percent into 0–100.
        /// </summary>
        public static void Normalize(FanSetpoint[] points, int count)
        {
            if (points == null) return;
            count = ClampCount(count);
            if (points.Length < count) return;

            for (int i = 0; i < count; i++)
            {
                points[i].TemperatureC = ClampTemperature(points[i].TemperatureC);
                points[i].FanPercent = ClampPercent(points[i].FanPercent);
            }

            for (int i = 1; i < count; i++)
            {
                FanSetpoint current = points[i];
                int j = i - 1;
                while (j >= 0 && points[j].TemperatureC > current.TemperatureC)
                {
                    points[j + 1] = points[j];
                    j--;
                }
                points[j + 1] = current;
            }

            for (int i = 1; i < count; i++)
            {
                if (points[i].TemperatureC <= points[i - 1].TemperatureC)
                    points[i].TemperatureC = Math.Min(MaxTemperatureC, points[i - 1].TemperatureC + 1);
            }

            if (points[count - 1].TemperatureC > MaxTemperatureC)
                points[count - 1].TemperatureC = MaxTemperatureC;

            for (int i = count - 2; i >= 0; i--)
            {
                if (points[i].TemperatureC >= points[i + 1].TemperatureC)
                    points[i].TemperatureC = points[i + 1].TemperatureC - 1;
            }
        }

        public static FanSetpoint Midpoint(FanSetpoint low, FanSetpoint high)
        {
            return new FanSetpoint(
                (low.TemperatureC + high.TemperatureC) / 2,
                (low.FanPercent + high.FanPercent) / 2);
        }

        /// <summary>
        /// Fan percent for a temperature, or -1 when the temperature is invalid.
        /// </summary>
        public static int GetFanPercent(int temperatureC, FanSetpoint[] points, int count, bool clampUnsafe)
        {
            if (temperatureC < 0 || points == null)
                return -1;

            count = ClampCount(count);
            if (points.Length < count)
                return -1;

            var ordered = new FanSetpoint[count];
            Array.Copy(points, ordered, count);
            Array.Sort(ordered, delegate (FanSetpoint a, FanSetpoint b)
            {
                return a.TemperatureC.CompareTo(b.TemperatureC);
            });

            if (temperatureC < ordered[0].TemperatureC)
                return clampUnsafe ? ClampUnsafe(0) : 0;

            int percent = ClampPercent(ordered[0].FanPercent);
            for (int i = 1; i < ordered.Length; i++)
            {
                if (temperatureC >= ordered[i].TemperatureC)
                    percent = ClampPercent(ordered[i].FanPercent);
            }

            return clampUnsafe ? ClampUnsafe(percent) : percent;
        }

        /// <summary>
        /// Higher request wins. A negative percent means that curve has no reading.
        /// </summary>
        public static int Combine(int cpuPercent, int gpuPercent)
        {
            if (cpuPercent < 0) return gpuPercent;
            if (gpuPercent < 0) return cpuPercent;
            return Math.Max(cpuPercent, gpuPercent);
        }

        private static int ClampUnsafe(int percent)
        {
            if (percent <= 0) return 0;
            return Math.Max(40, Math.Min(99, percent));
        }
    }
}
