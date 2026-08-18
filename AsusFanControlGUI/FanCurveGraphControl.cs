using AsusFanControl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace AsusFanControlGUI
{
    public class FanCurveGraphControl : Control
    {
        private FanCurve _fanCurve;
        private int _selectedPointIndex = -1;
        private int _hoveredPointIndex = -1;
        private bool _isDragging = false;
        private int _currentTemperature = -1;
        private bool _forbidUnsafeSettings = true;

        public event EventHandler CurveChanged;

        public FanCurve FanCurve
        {
            get => _fanCurve;
            set
            {
                _fanCurve = value;
                Invalidate();
            }
        }

        public int CurrentTemperature
        {
            get => _currentTemperature;
            set
            {
                _currentTemperature = value;
                Invalidate();
            }
        }

        public bool ForbidUnsafeSettings
        {
            get => _forbidUnsafeSettings;
            set
            {
                _forbidUnsafeSettings = value;
                Invalidate();
            }
        }

        public int SelectedPointIndex
        {
            get => _selectedPointIndex;
            set
            {
                _selectedPointIndex = value;
                Invalidate();
            }
        }

        private const int LeftMargin = 45;
        private const int RightMargin = 25;
        private const int TopMargin = 25;
        private const int BottomMargin = 40;
        private const int NodeRadius = 6;

        public FanCurveGraphControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            UpdateStyles();
            Cursor = Cursors.Default;
        }

        private Rectangle PlotBounds => new Rectangle(
            LeftMargin,
            TopMargin,
            Math.Max(10, Width - LeftMargin - RightMargin),
            Math.Max(10, Height - TopMargin - BottomMargin)
        );

        private PointF TempSpeedToPixel(int temp, int speed)
        {
            var bounds = PlotBounds;
            float x = bounds.Left + (temp / 100.0f) * bounds.Width;
            float y = bounds.Bottom - (speed / 100.0f) * bounds.Height;
            return new PointF(x, y);
        }

        private (int temp, int speed) PixelToTempSpeed(Point pixel)
        {
            var bounds = PlotBounds;
            float clampedX = Math.Max(bounds.Left, Math.Min(bounds.Right, pixel.X));
            float clampedY = Math.Max(bounds.Top, Math.Min(bounds.Bottom, pixel.Y));

            int temp = (int)Math.Round(((clampedX - bounds.Left) / (float)bounds.Width) * 100);
            int speed = (int)Math.Round(((bounds.Bottom - clampedY) / (float)bounds.Height) * 100);

            temp = Math.Max(0, Math.Min(100, temp));
            speed = Math.Max(0, Math.Min(100, speed));

            if (_forbidUnsafeSettings && speed > 0)
            {
                speed = Math.Max(40, Math.Min(99, speed));
            }

            return (temp, speed);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var bounds = PlotBounds;

            // Background
            using (var bgBrush = new LinearGradientBrush(ClientRectangle, Color.FromArgb(24, 24, 32), Color.FromArgb(32, 32, 44), 90f))
            {
                g.FillRectangle(bgBrush, ClientRectangle);
            }

            // Draw Safety Zone Shading if enabled
            if (_forbidUnsafeSettings)
            {
                float unsafeTop = bounds.Bottom - (40 / 100.0f) * bounds.Height;
                var unsafeRect = new RectangleF(bounds.Left, unsafeTop, bounds.Width, bounds.Bottom - unsafeTop);
                using (var unsafeBrush = new SolidBrush(Color.FromArgb(35, 220, 50, 50)))
                {
                    g.FillRectangle(unsafeBrush, unsafeRect);
                }
                using (var pen = new Pen(Color.FromArgb(120, 240, 80, 80), 1) { DashStyle = DashStyle.Dash })
                {
                    g.DrawLine(pen, bounds.Left, unsafeTop, bounds.Right, unsafeTop);
                }
                using (var font = new Font("Segoe UI", 7.5f, FontStyle.Italic))
                using (var textBrush = new SolidBrush(Color.FromArgb(180, 240, 100, 100)))
                {
                    g.DrawString("Unsafe Zone (<40%)", font, textBrush, bounds.Left + 5, unsafeTop + 3);
                }
            }

            // Grid Lines & Labels
            using (var gridPen = new Pen(Color.FromArgb(45, 45, 60), 1))
            using (var axisPen = new Pen(Color.FromArgb(90, 90, 110), 1))
            using (var labelFont = new Font("Segoe UI", 8f))
            using (var labelBrush = new SolidBrush(Color.FromArgb(160, 160, 180)))
            {
                // Vertical grid lines (Temperature 0 - 100 °C, step 10)
                for (int t = 0; t <= 100; t += 10)
                {
                    float x = bounds.Left + (t / 100.0f) * bounds.Width;
                    g.DrawLine(gridPen, x, bounds.Top, x, bounds.Bottom);

                    if (t % 20 == 0)
                    {
                        string txt = $"{t}°C";
                        SizeF size = g.MeasureString(txt, labelFont);
                        g.DrawString(txt, labelFont, labelBrush, x - size.Width / 2, bounds.Bottom + 5);
                    }
                }

                // Horizontal grid lines (Fan Speed 0 - 100 %, step 10)
                for (int s = 0; s <= 100; s += 10)
                {
                    float y = bounds.Bottom - (s / 100.0f) * bounds.Height;
                    g.DrawLine(gridPen, bounds.Left, y, bounds.Right, y);

                    if (s % 20 == 0)
                    {
                        string txt = $"{s}%";
                        SizeF size = g.MeasureString(txt, labelFont);
                        g.DrawString(txt, labelFont, labelBrush, bounds.Left - size.Width - 4, y - size.Height / 2);
                    }
                }

                // Axis borders
                g.DrawRectangle(axisPen, bounds);

                // Axis Title Labels
                using (var titleFont = new Font("Segoe UI", 8.5f, FontStyle.Bold))
                using (var titleBrush = new SolidBrush(Color.FromArgb(200, 200, 220)))
                {
                    g.DrawString("Temperature (°C)", titleFont, titleBrush, bounds.Left + bounds.Width / 2 - 45, bounds.Bottom + 20);

                    // Vertical Y-axis title
                    var state = g.Save();
                    g.TranslateTransform(12, bounds.Top + bounds.Height / 2 + 35);
                    g.RotateTransform(-90);
                    g.DrawString("Fan Speed (%)", titleFont, titleBrush, 0, 0);
                    g.Restore(state);
                }
            }

            if (_fanCurve == null || _fanCurve.Points == null || _fanCurve.Points.Count == 0)
                return;

            var points = _fanCurve.Points.OrderBy(p => p.Temperature).ToList();
            List<PointF> pixelPoints = points.Select(p => TempSpeedToPixel(p.Temperature, p.FanSpeed)).ToList();

            // Fill area under curve
            if (pixelPoints.Count >= 2)
            {
                List<PointF> fillPolygon = new List<PointF>(pixelPoints);
                fillPolygon.Add(new PointF(pixelPoints.Last().X, bounds.Bottom));
                fillPolygon.Add(new PointF(pixelPoints.First().X, bounds.Bottom));

                using (var fillBrush = new LinearGradientBrush(bounds, Color.FromArgb(70, 0, 160, 255), Color.FromArgb(10, 0, 100, 200), 90f))
                {
                    g.FillPolygon(fillBrush, fillPolygon.ToArray());
                }
            }

            // Draw Curve Line
            using (var linePen = new Pen(Color.FromArgb(0, 180, 255), 2.5f))
            {
                g.DrawLines(linePen, pixelPoints.ToArray());
            }

            // Draw Current Temperature Line if active
            if (_currentTemperature >= 0 && _currentTemperature <= 100)
            {
                float curX = bounds.Left + (_currentTemperature / 100.0f) * bounds.Width;
                using (var curPen = new Pen(Color.FromArgb(255, 180, 0), 1.5f) { DashStyle = DashStyle.Dash })
                {
                    g.DrawLine(curPen, curX, bounds.Top, curX, bounds.Bottom);
                }

                int activeSpeed = _fanCurve.GetFanSpeedForTemperature(_currentTemperature);
                if (_forbidUnsafeSettings && activeSpeed > 0)
                    activeSpeed = Math.Max(40, Math.Min(99, activeSpeed));

                string curTxt = $"Current: {_currentTemperature}°C ({activeSpeed}%)";
                using (var curFont = new Font("Segoe UI", 8f, FontStyle.Bold))
                using (var badgeBrush = new SolidBrush(Color.FromArgb(255, 180, 0)))
                using (var textBrush = new SolidBrush(Color.FromArgb(20, 20, 20)))
                {
                    SizeF sz = g.MeasureString(curTxt, curFont);
                    RectangleF badgeRect = new RectangleF(Math.Min(bounds.Right - sz.Width - 6, Math.Max(bounds.Left + 2, curX - sz.Width / 2)), bounds.Top + 4, sz.Width + 6, sz.Height + 2);
                    g.FillRectangle(badgeBrush, badgeRect);
                    g.DrawString(curTxt, curFont, textBrush, badgeRect.X + 3, badgeRect.Y + 1);
                }
            }

            // Draw Control Point Nodes
            using (var nodeFont = new Font("Segoe UI", 7.5f, FontStyle.Bold))
            {
                for (int i = 0; i < points.Count; i++)
                {
                    PointF pt = pixelPoints[i];
                    bool isSelected = (i == _selectedPointIndex);
                    bool isHovered = (i == _hoveredPointIndex);

                    int radius = (isSelected || isHovered) ? NodeRadius + 3 : NodeRadius;
                    RectangleF nodeRect = new RectangleF(pt.X - radius, pt.Y - radius, radius * 2, radius * 2);

                    Color fillCol = isSelected ? Color.FromArgb(255, 220, 0) : (isHovered ? Color.FromArgb(0, 230, 255) : Color.FromArgb(0, 160, 255));
                    Color borderCol = isSelected ? Color.White : Color.FromArgb(10, 30, 50);

                    using (var nodeBrush = new SolidBrush(fillCol))
                    using (var borderPen = new Pen(borderCol, 2))
                    {
                        g.FillEllipse(nodeBrush, nodeRect);
                        g.DrawEllipse(borderPen, nodeRect);
                    }

                    // Node coordinate tag
                    string labelStr = $"{points[i].Temperature}°C, {points[i].FanSpeed}%";
                    SizeF tagSize = g.MeasureString(labelStr, nodeFont);
                    float labelX = Math.Max(bounds.Left, Math.Min(bounds.Right - tagSize.Width, pt.X - tagSize.Width / 2));
                    float labelY = (pt.Y - radius - tagSize.Height - 3 < bounds.Top) ? pt.Y + radius + 3 : pt.Y - radius - tagSize.Height - 2;

                    using (var tagBg = new SolidBrush(Color.FromArgb(180, 20, 25, 35)))
                    using (var tagTxt = new SolidBrush(isSelected ? Color.FromArgb(255, 230, 100) : Color.FromArgb(220, 220, 240)))
                    {
                        g.FillRectangle(tagBg, labelX - 2, labelY - 1, tagSize.Width + 4, tagSize.Height + 2);
                        g.DrawString(labelStr, nodeFont, tagTxt, labelX, labelY);
                    }
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (_fanCurve == null || _fanCurve.Points == null)
                return;

            Focus();

            var points = _fanCurve.Points.OrderBy(p => p.Temperature).ToList();
            int clickedIdx = FindPointIndexAt(e.Location, points);

            if (e.Button == MouseButtons.Left)
            {
                if (clickedIdx >= 0)
                {
                    _selectedPointIndex = clickedIdx;
                    _isDragging = true;
                    Invalidate();
                    CurveChanged?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    // Clicked empty area - check if double click or check if within plot bounds to deselect
                    _selectedPointIndex = -1;
                    Invalidate();
                    CurveChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (clickedIdx >= 0 && points.Count > 2)
                {
                    _fanCurve.Points.Remove(points[clickedIdx]);
                    _selectedPointIndex = -1;
                    Invalidate();
                    CurveChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (_fanCurve == null || e.Button != MouseButtons.Left)
                return;

            if (PlotBounds.Contains(e.Location))
            {
                var (temp, speed) = PixelToTempSpeed(e.Location);
                // Ensure no duplicate temperature point
                if (!_fanCurve.Points.Any(p => p.Temperature == temp))
                {
                    var newPoint = new FanCurvePoint(temp, speed);
                    _fanCurve.Points.Add(newPoint);
                    _fanCurve.Points.Sort((a, b) => a.Temperature.CompareTo(b.Temperature));
                    _selectedPointIndex = _fanCurve.Points.IndexOf(newPoint);
                    Invalidate();
                    CurveChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_fanCurve == null || _fanCurve.Points == null)
                return;

            var points = _fanCurve.Points.OrderBy(p => p.Temperature).ToList();

            if (_isDragging && _selectedPointIndex >= 0 && _selectedPointIndex < points.Count)
            {
                var (newTemp, newSpeed) = PixelToTempSpeed(e.Location);

                // Enforce neighbor temperature bounds so points don't cross
                int minTemp = (_selectedPointIndex > 0) ? points[_selectedPointIndex - 1].Temperature + 1 : 0;
                int maxTemp = (_selectedPointIndex < points.Count - 1) ? points[_selectedPointIndex + 1].Temperature - 1 : 100;

                newTemp = Math.Max(minTemp, Math.Min(maxTemp, newTemp));

                points[_selectedPointIndex].Temperature = newTemp;
                points[_selectedPointIndex].FanSpeed = newSpeed;

                Invalidate();
                CurveChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                int hoverIdx = FindPointIndexAt(e.Location, points);
                if (hoverIdx != _hoveredPointIndex)
                {
                    _hoveredPointIndex = hoverIdx;
                    Cursor = (_hoveredPointIndex >= 0) ? Cursors.Hand : Cursors.Default;
                    Invalidate();
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_isDragging)
            {
                _isDragging = false;
                CurveChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private int FindPointIndexAt(Point pixel, List<FanCurvePoint> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                PointF nodePixel = TempSpeedToPixel(points[i].Temperature, points[i].FanSpeed);
                float dx = pixel.X - nodePixel.X;
                float dy = pixel.Y - nodePixel.Y;
                if (dx * dx + dy * dy <= (NodeRadius + 6) * (NodeRadius + 6))
                {
                    return i;
                }
            }
            return -1;
        }

        public void RemoveSelectedPoint()
        {
            if (_fanCurve != null && _selectedPointIndex >= 0 && _fanCurve.Points.Count > 2)
            {
                var points = _fanCurve.Points.OrderBy(p => p.Temperature).ToList();
                if (_selectedPointIndex < points.Count)
                {
                    _fanCurve.Points.Remove(points[_selectedPointIndex]);
                    _selectedPointIndex = -1;
                    Invalidate();
                    CurveChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
