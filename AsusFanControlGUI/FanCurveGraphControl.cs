using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace AsusFanControlGUI
{
    internal sealed class FanCurveGraphControl : Control
    {
        public const int MinimumTemperature = 20;
        public const int MaximumTemperature = 110;
        public const int MaximumFanSpeed = 100;

        private const int PointRadius = 6;
        private const int HitRadius = 10;

        private readonly List<FanCurvePoint> points = new List<FanCurvePoint>();

        private int draggedPointIndex = -1;
        private int hoveredPointIndex = -1;
        private int minimumFanSpeed;

        public FanCurveGraphControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

            BackColor = Color.White;
            Size = new Size(314, 132);
            MinimumFanSpeed = 0;
            Curve = FanCurve.Default;
        }

        public event EventHandler CurveChanged;

        public int MinimumFanSpeed
        {
            get => minimumFanSpeed;
            set
            {
                var normalizedValue = Math.Max(0, Math.Min(MaximumFanSpeed, value));
                if (minimumFanSpeed == normalizedValue)
                    return;

                minimumFanSpeed = normalizedValue;

                if (points.Count > 0)
                {
                    var normalizedCurve = NormalizeCurve(points);
                    points.Clear();
                    points.AddRange(normalizedCurve.Points.Select(point => new FanCurvePoint(point.Temperature, point.FanSpeed)));
                }

                Invalidate();
            }
        }

        public FanCurve Curve
        {
            get => new FanCurve(points.Select(point => new FanCurvePoint(point.Temperature, point.FanSpeed)));
            set
            {
                var normalizedCurve = NormalizeCurve(value?.Points ?? FanCurve.Default.Points);
                points.Clear();
                points.AddRange(normalizedCurve.Points.Select(point => new FanCurvePoint(point.Temperature, point.FanSpeed)));
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var plotRectangle = GetPlotRectangle();
            using (var backgroundBrush = new SolidBrush(Color.FromArgb(248, 250, 252)))
            using (var gridPen = new Pen(Color.FromArgb(214, 222, 233)))
            using (var borderPen = new Pen(Color.FromArgb(150, 164, 179)))
            using (var linePen = new Pen(Color.FromArgb(49, 99, 191), 2.25f))
            using (var pointBrush = new SolidBrush(Color.FromArgb(31, 78, 161)))
            using (var pointBorderPen = new Pen(Color.White, 1.5f))
            using (var highlightPen = new Pen(Color.FromArgb(255, 160, 0), 2f))
            {
                e.Graphics.FillRectangle(backgroundBrush, plotRectangle);

                for (var index = 1; index < 5; index++)
                {
                    var horizontalY = plotRectangle.Top + ((plotRectangle.Height * index) / 5.0f);
                    var verticalX = plotRectangle.Left + ((plotRectangle.Width * index) / 5.0f);

                    e.Graphics.DrawLine(gridPen, plotRectangle.Left, horizontalY, plotRectangle.Right, horizontalY);
                    e.Graphics.DrawLine(gridPen, verticalX, plotRectangle.Top, verticalX, plotRectangle.Bottom);
                }

                e.Graphics.DrawRectangle(borderPen, plotRectangle);

                var pathPoints = new List<PointF>
                {
                    CurveToClientPoint(MinimumTemperature, MinimumFanSpeed, plotRectangle)
                };
                pathPoints.AddRange(points.Select(point => CurveToClientPoint(point.Temperature, point.FanSpeed, plotRectangle)));
                pathPoints.Add(CurveToClientPoint(MaximumTemperature, MaximumFanSpeed, plotRectangle));

                if (pathPoints.Count > 1)
                    e.Graphics.DrawLines(linePen, pathPoints.ToArray());

                for (var index = 0; index < points.Count; index++)
                {
                    var point = points[index];
                    var screenPoint = CurveToClientPoint(point.Temperature, point.FanSpeed, plotRectangle);
                    var pointRectangle = new RectangleF(screenPoint.X - PointRadius, screenPoint.Y - PointRadius, PointRadius * 2, PointRadius * 2);

                    e.Graphics.FillEllipse(pointBrush, pointRectangle);
                    e.Graphics.DrawEllipse(pointBorderPen, pointRectangle);

                    if (index == draggedPointIndex || index == hoveredPointIndex)
                        e.Graphics.DrawEllipse(highlightPen, pointRectangle);
                }
            }

            DrawLabels(e.Graphics, plotRectangle);
            DrawActivePointLabel(e.Graphics, plotRectangle);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button != MouseButtons.Left)
                return;

            draggedPointIndex = FindPointIndex(e.Location);
            if (draggedPointIndex < 0)
                return;

            Capture = true;
            UpdatePointFromMouse(e.Location);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (draggedPointIndex >= 0)
            {
                UpdatePointFromMouse(e.Location);
                return;
            }

            var hoveredIndex = FindPointIndex(e.Location);
            if (hoveredPointIndex != hoveredIndex)
            {
                hoveredPointIndex = hoveredIndex;
                Invalidate();
            }

            Cursor = hoveredIndex >= 0 ? Cursors.Hand : Cursors.Default;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (draggedPointIndex < 0)
                return;

            draggedPointIndex = -1;
            Capture = false;
            hoveredPointIndex = FindPointIndex(e.Location);
            Cursor = hoveredPointIndex >= 0 ? Cursors.Hand : Cursors.Default;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (draggedPointIndex >= 0)
                return;

            hoveredPointIndex = -1;
            Cursor = Cursors.Default;
            Invalidate();
        }

        private void DrawLabels(Graphics graphics, Rectangle plotRectangle)
        {
            TextRenderer.DrawText(graphics, "Fan (%)", Font, new Point(0, 0), Color.FromArgb(58, 69, 80));
            TextRenderer.DrawText(graphics, string.Format("{0}%", MaximumFanSpeed), Font, new Point(0, plotRectangle.Top - 6), Color.FromArgb(58, 69, 80));
            TextRenderer.DrawText(graphics, string.Format("{0}%", MinimumFanSpeed), Font, new Point(0, plotRectangle.Bottom - 8), Color.FromArgb(58, 69, 80));
            TextRenderer.DrawText(graphics, string.Format("{0}C", MinimumTemperature), Font, new Point(plotRectangle.Left - 8, plotRectangle.Bottom + 6), Color.FromArgb(58, 69, 80));

            var maximumTemperatureText = string.Format("{0}C", MaximumTemperature);
            var maximumTemperatureSize = TextRenderer.MeasureText(maximumTemperatureText, Font);
            TextRenderer.DrawText(graphics, maximumTemperatureText, Font, new Point(plotRectangle.Right - maximumTemperatureSize.Width + 6, plotRectangle.Bottom + 6), Color.FromArgb(58, 69, 80));

            var axisTitle = "Temperature (C)";
            var axisTitleSize = TextRenderer.MeasureText(axisTitle, Font);
            TextRenderer.DrawText(graphics, axisTitle, Font, new Point(plotRectangle.Left + ((plotRectangle.Width - axisTitleSize.Width) / 2), plotRectangle.Bottom + 6), Color.FromArgb(58, 69, 80));
        }

        private void DrawActivePointLabel(Graphics graphics, Rectangle plotRectangle)
        {
            var activePointIndex = draggedPointIndex >= 0 ? draggedPointIndex : hoveredPointIndex;
            if (activePointIndex < 0 || activePointIndex >= points.Count)
                return;

            var point = points[activePointIndex];
            var screenPoint = CurveToClientPoint(point.Temperature, point.FanSpeed, plotRectangle);
            var labelText = string.Format("P{0}: {1}C / {2}%", activePointIndex + 1, point.Temperature, point.FanSpeed);
            var labelSize = TextRenderer.MeasureText(labelText, Font);
            var labelLocation = new Point(
                Math.Max(plotRectangle.Left, Math.Min((int)screenPoint.X - (labelSize.Width / 2), plotRectangle.Right - labelSize.Width)),
                Math.Max(0, (int)screenPoint.Y - labelSize.Height - 12));

            var labelRectangle = new Rectangle(labelLocation, labelSize + new Size(8, 4));
            using (var labelBrush = new SolidBrush(Color.FromArgb(245, 248, 255)))
            using (var labelBorderPen = new Pen(Color.FromArgb(182, 197, 215)))
            {
                graphics.FillRectangle(labelBrush, labelRectangle);
                graphics.DrawRectangle(labelBorderPen, labelRectangle);
            }

            TextRenderer.DrawText(graphics, labelText, Font, new Point(labelRectangle.Left + 4, labelRectangle.Top + 2), Color.FromArgb(41, 53, 65));
        }

        private Rectangle GetPlotRectangle()
        {
            return new Rectangle(38, 18, Math.Max(120, Width - 56), Math.Max(80, Height - 42));
        }

        private PointF CurveToClientPoint(int temperature, int fanSpeed, Rectangle plotRectangle)
        {
            var clampedTemperature = Math.Max(MinimumTemperature, Math.Min(MaximumTemperature, temperature));
            var clampedFanSpeed = Math.Max(MinimumFanSpeed, Math.Min(MaximumFanSpeed, fanSpeed));
            var normalizedTemperature = (clampedTemperature - MinimumTemperature) / (float)(MaximumTemperature - MinimumTemperature);
            var normalizedFanSpeed = (clampedFanSpeed - MinimumFanSpeed) / (float)Math.Max(1, MaximumFanSpeed - MinimumFanSpeed);

            return new PointF(
                plotRectangle.Left + (plotRectangle.Width * normalizedTemperature),
                plotRectangle.Bottom - (plotRectangle.Height * normalizedFanSpeed));
        }

        private int FindPointIndex(Point clientPoint)
        {
            var plotRectangle = GetPlotRectangle();
            for (var index = 0; index < points.Count; index++)
            {
                var screenPoint = CurveToClientPoint(points[index].Temperature, points[index].FanSpeed, plotRectangle);
                var deltaX = clientPoint.X - screenPoint.X;
                var deltaY = clientPoint.Y - screenPoint.Y;
                if ((deltaX * deltaX) + (deltaY * deltaY) <= HitRadius * HitRadius)
                    return index;
            }

            return -1;
        }

        private void UpdatePointFromMouse(Point clientPoint)
        {
            if (draggedPointIndex < 0 || draggedPointIndex >= points.Count)
                return;

            var plotRectangle = GetPlotRectangle();
            var minimumTemperature = draggedPointIndex == 0 ? MinimumTemperature : points[draggedPointIndex - 1].Temperature + 1;
            var maximumTemperature = draggedPointIndex == points.Count - 1 ? MaximumTemperature : points[draggedPointIndex + 1].Temperature - 1;

            var newTemperature = ClientToTemperature(clientPoint.X, plotRectangle);
            var newFanSpeed = ClientToFanSpeed(clientPoint.Y, plotRectangle);

            newTemperature = Math.Max(minimumTemperature, Math.Min(maximumTemperature, newTemperature));
            newFanSpeed = Math.Max(MinimumFanSpeed, Math.Min(MaximumFanSpeed, newFanSpeed));

            var existingPoint = points[draggedPointIndex];
            if (existingPoint.Temperature == newTemperature && existingPoint.FanSpeed == newFanSpeed)
                return;

            points[draggedPointIndex] = new FanCurvePoint(newTemperature, newFanSpeed);
            Invalidate();
            CurveChanged?.Invoke(this, EventArgs.Empty);
        }

        private int ClientToTemperature(int x, Rectangle plotRectangle)
        {
            var normalized = (x - plotRectangle.Left) / (double)Math.Max(1, plotRectangle.Width);
            return MinimumTemperature + (int)Math.Round((MaximumTemperature - MinimumTemperature) * normalized, MidpointRounding.AwayFromZero);
        }

        private int ClientToFanSpeed(int y, Rectangle plotRectangle)
        {
            var normalized = (plotRectangle.Bottom - y) / (double)Math.Max(1, plotRectangle.Height);
            return MinimumFanSpeed + (int)Math.Round((MaximumFanSpeed - MinimumFanSpeed) * normalized, MidpointRounding.AwayFromZero);
        }

        private FanCurve NormalizeCurve(IEnumerable<FanCurvePoint> sourcePoints)
        {
            var rawPoints = (sourcePoints ?? FanCurve.Default.Points)
                .OrderBy(point => point.Temperature)
                .ToList();

            if (rawPoints.Count != FanCurve.Default.Points.Count)
                rawPoints = FanCurve.Default.Points.ToList();

            var normalizedPoints = new List<FanCurvePoint>(rawPoints.Count);
            for (var index = 0; index < rawPoints.Count; index++)
            {
                var remainingPoints = rawPoints.Count - index - 1;
                var minimumTemperature = index == 0 ? MinimumTemperature : normalizedPoints[index - 1].Temperature + 1;
                var maximumTemperature = MaximumTemperature - remainingPoints;
                var normalizedTemperature = Math.Max(minimumTemperature, Math.Min(maximumTemperature, rawPoints[index].Temperature));
                var normalizedFanSpeed = Math.Max(MinimumFanSpeed, Math.Min(MaximumFanSpeed, rawPoints[index].FanSpeed));

                normalizedPoints.Add(new FanCurvePoint(normalizedTemperature, normalizedFanSpeed));
            }

            return new FanCurve(normalizedPoints);
        }
    }
}