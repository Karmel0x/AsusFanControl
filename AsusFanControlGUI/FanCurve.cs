using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AsusFanControlGUI
{
    internal sealed class FanCurve
    {
        public static FanCurve Default => new FanCurve(new[]
        {
            new FanCurvePoint(45, 40),
            new FanCurvePoint(60, 55),
            new FanCurvePoint(70, 70),
            new FanCurvePoint(80, 85),
            new FanCurvePoint(90, 99),
        });

        public FanCurve(IEnumerable<FanCurvePoint> points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            var orderedPoints = points.OrderBy(point => point.Temperature).ToList();
            if (orderedPoints.Count < 2)
                throw new ArgumentException("A fan curve needs at least two points.", nameof(points));

            for (var index = 0; index < orderedPoints.Count; index++)
            {
                var point = orderedPoints[index];
                if (point.Temperature < 0 || point.Temperature > 120)
                    throw new ArgumentOutOfRangeException(nameof(points), "Curve temperatures must be between 0 and 120 C.");

                if (point.FanSpeed < 0 || point.FanSpeed > 100)
                    throw new ArgumentOutOfRangeException(nameof(points), "Curve fan speeds must be between 0 and 100 percent.");

                if (index > 0 && point.Temperature <= orderedPoints[index - 1].Temperature)
                    throw new ArgumentException("Curve temperatures must be strictly increasing.", nameof(points));
            }

            Points = orderedPoints;
        }

        public IReadOnlyList<FanCurvePoint> Points { get; }

        public int GetFanSpeed(int temperature)
        {
            if (temperature <= Points[0].Temperature)
                return Points[0].FanSpeed;

            for (var index = 1; index < Points.Count; index++)
            {
                var lowerPoint = Points[index - 1];
                var upperPoint = Points[index];
                if (temperature <= upperPoint.Temperature)
                {
                    var temperatureDelta = upperPoint.Temperature - lowerPoint.Temperature;
                    if (temperatureDelta <= 0)
                        return upperPoint.FanSpeed;

                    var progress = (temperature - lowerPoint.Temperature) / (double)temperatureDelta;
                    var interpolatedSpeed = lowerPoint.FanSpeed + ((upperPoint.FanSpeed - lowerPoint.FanSpeed) * progress);
                    return (int)Math.Round(interpolatedSpeed, MidpointRounding.AwayFromZero);
                }
            }

            return Points[Points.Count - 1].FanSpeed;
        }

        public int GetFanSpeed(int temperature, int minimumTemperature, int maximumTemperature, int minimumFanSpeed, int maximumFanSpeed)
        {
            var normalizedMinimumTemperature = Math.Max(0, minimumTemperature);
            var normalizedMaximumTemperature = Math.Max(normalizedMinimumTemperature, maximumTemperature);
            var normalizedMinimumFanSpeed = Math.Max(0, Math.Min(100, minimumFanSpeed));
            var normalizedMaximumFanSpeed = Math.Max(normalizedMinimumFanSpeed, Math.Min(100, maximumFanSpeed));

            var curvePoints = new List<FanCurvePoint>
            {
                new FanCurvePoint(normalizedMinimumTemperature, normalizedMinimumFanSpeed)
            };
            curvePoints.AddRange(Points);
            curvePoints.Add(new FanCurvePoint(normalizedMaximumTemperature, normalizedMaximumFanSpeed));

            if (temperature <= curvePoints[0].Temperature)
                return curvePoints[0].FanSpeed;

            for (var index = 1; index < curvePoints.Count; index++)
            {
                var lowerPoint = curvePoints[index - 1];
                var upperPoint = curvePoints[index];
                if (temperature <= upperPoint.Temperature)
                {
                    var temperatureDelta = upperPoint.Temperature - lowerPoint.Temperature;
                    if (temperatureDelta <= 0)
                        return upperPoint.FanSpeed;

                    var progress = (temperature - lowerPoint.Temperature) / (double)temperatureDelta;
                    var interpolatedSpeed = lowerPoint.FanSpeed + ((upperPoint.FanSpeed - lowerPoint.FanSpeed) * progress);
                    return (int)Math.Round(interpolatedSpeed, MidpointRounding.AwayFromZero);
                }
            }

            return curvePoints[curvePoints.Count - 1].FanSpeed;
        }

        public string Serialize()
        {
            return string.Join(";", Points.Select(point =>
                string.Format(CultureInfo.InvariantCulture, "{0}:{1}", point.Temperature, point.FanSpeed)));
        }

        public static FanCurve ParseOrDefault(string serializedCurve)
        {
            if (TryParse(serializedCurve, out var fanCurve))
                return fanCurve;

            return Default;
        }

        public static bool TryParse(string serializedCurve, out FanCurve fanCurve)
        {
            fanCurve = null;

            if (string.IsNullOrWhiteSpace(serializedCurve))
                return false;

            var points = new List<FanCurvePoint>();
            foreach (var rawPoint in serializedCurve.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var segments = rawPoint.Split(':');
                if (segments.Length != 2)
                    return false;

                if (!int.TryParse(segments[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var temperature))
                    return false;

                if (!int.TryParse(segments[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var fanSpeed))
                    return false;

                points.Add(new FanCurvePoint(temperature, fanSpeed));
            }

            try
            {
                fanCurve = new FanCurve(points);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }

    internal sealed class FanCurvePoint
    {
        public FanCurvePoint(int temperature, int fanSpeed)
        {
            Temperature = temperature;
            FanSpeed = fanSpeed;
        }

        public int Temperature { get; }

        public int FanSpeed { get; }
    }
}