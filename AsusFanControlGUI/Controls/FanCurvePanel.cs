using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using AsusFanControlGUI.Models;
using AsusFanControlGUI.Services;

namespace AsusFanControlGUI.Controls
{
    /// <summary>
    /// A fully custom-painted WinForms UserControl that renders an interactive
    /// fan-speed vs. temperature curve. Nodes are draggable when IsReadOnly = false.
    /// </summary>
    public class FanCurvePanel : UserControl
    {
        // ── Public API ───────────────────────────────────────────────────────────

        private List<CurvePoint> _points = new List<CurvePoint>();
        public List<CurvePoint> Points
        {
            get => _points;
            set { _points = value ?? new List<CurvePoint>(); Invalidate(); }
        }

        public bool IsReadOnly  { get; set; } = false;
        public bool ClampToGrid { get; set; } = false;
        public string PanelTitle { get; set; } = "Fan Profile, RPM/°C";
        public Color CurveColor  { get; set; } = Color.FromArgb(79, 195, 247);
        public float MaxRpm      { get; set; } = 7000f;

        /// <summary>Fired after any node is dragged so parent can react.</summary>
        public event EventHandler PointsChanged;

        // ── Drawing constants ────────────────────────────────────────────────────
        private const float TempMin  = 20f;
        private const float TempMax  = 110f;
        private const int   NodeRadius        = 6;
        private const int   ActiveNodeRadius  = 8;
        private const int   HitRadius         = 12;

        private readonly int PadLeft   = 58;
        private readonly int PadRight  = 14;
        private readonly int PadTop    = 30;
        private readonly int PadBottom = 36;

        // ── Drag state ───────────────────────────────────────────────────────────
        private int _dragIndex = -1;   // index into _points currently being dragged

        // ── Constructor ──────────────────────────────────────────────────────────
        public FanCurvePanel()
        {
            DoubleBuffered = true;
            ResizeRedraw   = true;
            BackColor      = Color.FromArgb(22, 22, 46);
            MinimumSize    = new Size(200, 140);

            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp   += OnMouseUp;
        }

        // ── Canvas geometry ──────────────────────────────────────────────────────

        private RectangleF PlotArea => new RectangleF(
            PadLeft,
            PadTop,
            Math.Max(10, Width  - PadLeft - PadRight),
            Math.Max(10, Height - PadTop  - PadBottom));

        private PointF CurvePointToCanvas(CurvePoint pt)
        {
            var pa = PlotArea;
            float xFrac = (pt.TempC - TempMin) / (TempMax - TempMin);
            float yFrac = pt.Rpm / MaxRpm;
            return new PointF(
                pa.Left + xFrac * pa.Width,
                pa.Bottom - yFrac * pa.Height);   // Y inverted: high RPM = top
        }

        private CurvePoint CanvasToCurvePoint(PointF px)
        {
            var pa = PlotArea;
            float xFrac = (px.X - pa.Left) / pa.Width;
            float yFrac = (pa.Bottom - px.Y) / pa.Height;

            float tempC = TempMin + xFrac * (TempMax - TempMin);
            float rpm   = yFrac * MaxRpm;

            // Clamp
            tempC = Math.Max(TempMin, Math.Min(TempMax, tempC));
            rpm   = Math.Max(0,       Math.Min(MaxRpm,  rpm));

            if (ClampToGrid)
                tempC = (float)(Math.Round(tempC / 5.0) * 5.0);

            return new CurvePoint(tempC, rpm);
        }

        // ── Interpolation (static, usable by FanCurveDaemon too) ────────────────
        public float GetTargetRpm(float tempC)
        {
            return FanCurveDaemon.InterpolateRpm(_points, tempC);
        }

        // ── Painting ─────────────────────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var pa = PlotArea;

            // Background
            g.FillRectangle(new SolidBrush(Color.FromArgb(20, 20, 40)), 0, 0, Width, Height);
            g.FillRectangle(new SolidBrush(Color.FromArgb(22, 22, 46)), pa.Left, pa.Top, pa.Width, pa.Height);

            DrawGrid(g, pa);
            DrawAxes(g, pa);
            DrawCurve(g, pa);
            DrawTitle(g);
        }

        private void DrawGrid(Graphics g, RectangleF pa)
        {
            using (var gridPen = new Pen(Color.FromArgb(38, 38, 60), 1f))
            {
                // Vertical lines every 10°C
                for (float t = TempMin; t <= TempMax; t += 10f)
                {
                    float x = pa.Left + (t - TempMin) / (TempMax - TempMin) * pa.Width;
                    g.DrawLine(gridPen, x, pa.Top, x, pa.Bottom);
                }
                // Horizontal lines every 500 RPM
                for (float r = 0; r <= MaxRpm; r += 500f)
                {
                    float y = pa.Bottom - r / MaxRpm * pa.Height;
                    g.DrawLine(gridPen, pa.Left, y, pa.Right, y);
                }
            }
        }

        private void DrawAxes(Graphics g, RectangleF pa)
        {
            using (var axisFont = new Font("Segoe UI", 7.5f))
            using (var labelBrush = new SolidBrush(Color.FromArgb(140, 140, 180)))
            using (var axisPen    = new Pen(Color.FromArgb(60, 60, 90), 1.2f))
            {
                // Draw axis border
                g.DrawRectangle(axisPen, pa.X, pa.Y, pa.Width, pa.Height);

                // X axis labels (temperature)
                for (float t = TempMin; t <= TempMax; t += 10f)
                {
                    float x = pa.Left + (t - TempMin) / (TempMax - TempMin) * pa.Width;
                    string lbl = $"{(int)t}°";
                    var sz = g.MeasureString(lbl, axisFont);
                    g.DrawString(lbl, axisFont, labelBrush, x - sz.Width / 2, pa.Bottom + 4);
                }
                // X axis name
                using (var boldFont = new Font("Segoe UI", 7.5f, FontStyle.Regular))
                {
                    string xlabel = "Temperature (°C)";
                    var xsz = g.MeasureString(xlabel, boldFont);
                    g.DrawString(xlabel, boldFont, labelBrush,
                        pa.Left + (pa.Width - xsz.Width) / 2, pa.Bottom + 18);
                }

                // Y axis labels (RPM)
                for (float r = 0; r <= MaxRpm; r += 500f)
                {
                    float y = pa.Bottom - r / MaxRpm * pa.Height;
                    string lbl = r == 0 ? "OFF" : $"{(int)r}";
                    var sz = g.MeasureString(lbl, axisFont);
                    g.DrawString(lbl, axisFont, labelBrush,
                        pa.Left - sz.Width - 4, y - sz.Height / 2);
                }
            }
        }

        private void DrawCurve(Graphics g, RectangleF pa)
        {
            if (_points == null || _points.Count == 0) return;

            // Sort by temp for drawing
            var sorted = new List<CurvePoint>(_points);
            sorted.Sort((a, b) => a.TempC.CompareTo(b.TempC));

            // Build pixel points
            var pixelPts = new List<PointF>();
            foreach (var pt in sorted)
                pixelPts.Add(CurvePointToCanvas(pt));

            // Draw filled area under curve
            if (pixelPts.Count >= 2)
            {
                var fillPts = new List<PointF>(pixelPts);
                fillPts.Add(new PointF(pixelPts[pixelPts.Count - 1].X, pa.Bottom));
                fillPts.Add(new PointF(pixelPts[0].X, pa.Bottom));
                using (var fillBrush = new SolidBrush(Color.FromArgb(30,
                    CurveColor.R, CurveColor.G, CurveColor.B)))
                {
                    g.FillPolygon(fillBrush, fillPts.ToArray());
                }

                // Draw curve line
                using (var curvePen = new Pen(CurveColor, 2.2f) { LineJoin = LineJoin.Round })
                {
                    g.DrawLines(curvePen, pixelPts.ToArray());
                }
            }

            // Draw nodes
            for (int i = 0; i < sorted.Count; i++)
            {
                var px = CurvePointToCanvas(sorted[i]);
                bool isDragged = !IsReadOnly && _dragIndex >= 0 &&
                                 Math.Abs(sorted[i].TempC - _points[_dragIndex].TempC) < 0.5f &&
                                 Math.Abs(sorted[i].Rpm   - _points[_dragIndex].Rpm)   < 0.5f;

                int r = isDragged ? ActiveNodeRadius : NodeRadius;
                var nodeColor = isDragged
                    ? Color.White
                    : CurveColor;

                var rect = new RectangleF(px.X - r, px.Y - r, r * 2, r * 2);

                using (var fill = new SolidBrush(nodeColor))
                    g.FillEllipse(fill, rect);
                using (var border = new Pen(Color.White, 1.2f))
                    g.DrawEllipse(border, rect);

                // Tooltip: show temp + rpm values
                using (var tipFont = new Font("Segoe UI", 7f))
                using (var tipBrush = new SolidBrush(Color.FromArgb(200, 200, 200)))
                {
                    string rpmLabel = sorted[i].Rpm <= 0 ? "OFF" : $"{(int)sorted[i].Rpm}";
                    string tipText  = $"{(int)sorted[i].TempC}°/{rpmLabel}";
                    g.DrawString(tipText, tipFont, tipBrush, px.X + r + 2, px.Y - 8);
                }
            }
        }

        private void DrawTitle(Graphics g)
        {
            using (var titleFont = new Font("Segoe UI", 9f, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(CurveColor))
            {
                var sz = g.MeasureString(PanelTitle, titleFont);
                g.DrawString(PanelTitle, titleFont, titleBrush,
                    (Width - sz.Width) / 2, 6);
            }
        }

        // ── Mouse interaction ────────────────────────────────────────────────────
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (IsReadOnly || e.Button != MouseButtons.Left) return;

            int closest = FindNearestNode(e.Location);
            if (closest >= 0)
            {
                _dragIndex = closest;
                Cursor = Cursors.SizeAll;
                Capture = true;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (IsReadOnly) return;

            if (_dragIndex < 0)
            {
                // Change cursor when hovering over a node
                Cursor = FindNearestNode(e.Location) >= 0 ? Cursors.SizeAll : Cursors.Default;
                return;
            }

            // Move the dragged node
            var newPt = CanvasToCurvePoint(e.Location);

            // Prevent two nodes sharing the same temp (clamp to ±1°C of neighbours)
            var sorted = new List<CurvePoint>(_points);
            sorted.Sort((a, b) => a.TempC.CompareTo(b.TempC));
            int sortedIdx = sorted.IndexOf(_points[_dragIndex]);

            float minT = (sortedIdx > 0) ? sorted[sortedIdx - 1].TempC + 1f : TempMin;
            float maxT = (sortedIdx < sorted.Count - 1) ? sorted[sortedIdx + 1].TempC - 1f : TempMax;
            newPt.TempC = Math.Max(minT, Math.Min(maxT, newPt.TempC));

            _points[_dragIndex] = newPt;
            Invalidate();
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (_dragIndex >= 0)
            {
                // Sort points by temp after drag
                _points.Sort((a, b) => a.TempC.CompareTo(b.TempC));
                _dragIndex = -1;
                Cursor  = Cursors.Default;
                Capture = false;
                Invalidate();
                PointsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>Returns the index of the nearest Point within HitRadius, or -1.</summary>
        private int FindNearestNode(Point mousePos)
        {
            int best = -1;
            float bestDist = HitRadius * HitRadius;
            for (int i = 0; i < _points.Count; i++)
            {
                var px = CurvePointToCanvas(_points[i]);
                float dx = px.X - mousePos.X;
                float dy = px.Y - mousePos.Y;
                float dist = dx * dx + dy * dy;
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = i;
                }
            }
            return best;
        }
    }
}
