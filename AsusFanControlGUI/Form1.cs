using AsusFanControl;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AsusFanControlGUI
{
    // ── Custom renderer: black background, cyan text/highlight for MenuStrip ──
    internal class NeonMenuRenderer : ToolStripProfessionalRenderer
    {
        static readonly Color clrBack   = Color.FromArgb(10, 10, 10);
        static readonly Color clrNeon   = Color.FromArgb(0, 255, 255);
        static readonly Color clrHover  = Color.FromArgb(0, 40, 40);
        static readonly Color clrBorder = Color.FromArgb(0, 120, 120);

        public NeonMenuRenderer() : base(new NeonColorTable()) { }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            var g = e.Graphics;
            var rect = new Rectangle(Point.Empty, e.Item.Size);
            var bg = e.Item.Selected ? clrHover : clrBack;
            using var brush = new SolidBrush(bg);
            g.FillRectangle(brush, rect);
            if (e.Item.Selected)
            {
                using var pen = new Pen(clrBorder);
                g.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
            }
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            using var brush = new SolidBrush(clrBack);
            e.Graphics.FillRectangle(brush, e.AffectedBounds);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            using var pen = new Pen(clrBorder);
            var r = e.AffectedBounds;
            e.Graphics.DrawRectangle(pen, r.X, r.Y, r.Width - 1, r.Height - 1);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = clrNeon;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = clrNeon;
            base.OnRenderArrow(e);
        }

        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
        {
            // Draw a neon ✓ for checked menu items instead of the default glyph
            if (e.Item is ToolStripMenuItem mi && mi.Checked)
            {
                var g = e.Graphics;
                var r = e.ImageRectangle;
                using var pen = new Pen(clrNeon, 2f);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawLines(pen, new Point[]
                {
                    new Point(r.Left + 2,  r.Top + r.Height / 2),
                    new Point(r.Left + r.Width / 3, r.Bottom - 3),
                    new Point(r.Right - 2, r.Top + 2)
                });
                return;
            }
            base.OnRenderItemImage(e);
        }
    }

    internal class NeonColorTable : ProfessionalColorTable
    {
        static readonly Color clrBack  = Color.FromArgb(10, 10, 10);
        static readonly Color clrBorder = Color.FromArgb(0, 120, 120);

        public override Color MenuStripGradientBegin   => clrBack;
        public override Color MenuStripGradientEnd     => clrBack;
        public override Color MenuBorder               => clrBorder;
        public override Color MenuItemBorder           => clrBorder;
        public override Color MenuItemSelected         => Color.FromArgb(0, 40, 40);
        public override Color MenuItemSelectedGradientBegin => Color.FromArgb(0, 40, 40);
        public override Color MenuItemSelectedGradientEnd   => Color.FromArgb(0, 40, 40);
        public override Color MenuItemPressedGradientBegin  => Color.FromArgb(0, 60, 60);
        public override Color MenuItemPressedGradientEnd    => Color.FromArgb(0, 60, 60);
        public override Color ToolStripDropDownBackground   => clrBack;
        public override Color ImageMarginGradientBegin => clrBack;
        public override Color ImageMarginGradientMiddle => clrBack;
        public override Color ImageMarginGradientEnd   => clrBack;
        public override Color SeparatorDark            => clrBorder;
        public override Color SeparatorLight           => clrBorder;
    }

    // ── Main form ─────────────────────────────────────────────────────────────
    public partial class Form1 : Form
    {
        static readonly Color clrBack  = Color.Black;
        static readonly Color clrNeon  = Color.FromArgb(0, 255, 255);
        static readonly Color clrMenuBg = Color.FromArgb(10, 10, 10);

        AsusControl asusControl = new AsusControl();
        int fanSpeed = 0;
        Timer timer;
        NotifyIcon trayIcon;

        public Form1()
        {
            InitializeComponent();
            menuStrip1.Renderer = new NeonMenuRenderer();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            toolStripMenuItemTurnOffControlOnExit.Checked = Properties.Settings.Default.turnOffControlOnExit;
            toolStripMenuItemForbidUnsafeSettings.Checked = Properties.Settings.Default.forbidUnsafeSettings;
            toolStripMenuItemMinimizeToTrayOnClose.Checked = Properties.Settings.Default.minimizeToTrayOnClose;
            toolStripMenuItemAutoRefreshStats.Checked = Properties.Settings.Default.autoRefreshStats;
            trackBarFanSpeed.Value = Properties.Settings.Default.fanSpeed;
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.turnOffControlOnExit)
                asusControl.SetFanSpeeds(0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timerRefreshStats();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Properties.Settings.Default.minimizeToTrayOnClose && Visible)
            {
                if (trayIcon == null)
                {
                    var contextMenu = new ContextMenuStrip();
                    contextMenu.BackColor = clrMenuBg;
                    contextMenu.ForeColor = clrNeon;
                    contextMenu.Renderer = new NeonMenuRenderer();
                    contextMenu.Font = new Font("Consolas", 9F);

                    var itemShow = new ToolStripMenuItem("Show");
                    itemShow.ForeColor = clrNeon;
                    itemShow.BackColor = clrMenuBg;
                    itemShow.Click += (s1, e1) => { trayIcon.Visible = false; Show(); };

                    var itemExit = new ToolStripMenuItem("Exit");
                    itemExit.ForeColor = clrNeon;
                    itemExit.BackColor = clrMenuBg;
                    itemExit.Click += (s1, e1) => { Close(); trayIcon.Visible = false; Application.Exit(); };

                    contextMenu.Items.Add(itemShow);
                    contextMenu.Items.Add(itemExit);

                    trayIcon = new NotifyIcon()
                    {
                        Icon = Icon,
                        ContextMenuStrip = contextMenu,
                    };
                    trayIcon.MouseClick += (s1, e1) =>
                    {
                        if (e1.Button != MouseButtons.Left)
                            return;
                        trayIcon.Visible = false;
                        Show();
                    };
                }

                trayIcon.Visible = true;
                e.Cancel = true;
                Hide();
            }
        }

        private void timerRefreshStats()
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            if (!Properties.Settings.Default.autoRefreshStats)
                return;

            timer = new Timer();
            timer.Interval = 2000;
            timer.Tick += new EventHandler(TimerEventProcessor);
            timer.Start();
        }

        private void TimerEventProcessor(object sender, EventArgs e)
        {
            buttonRefreshRPM_Click(sender, e);
            buttonRefreshCPUTemp_Click(sender, e);
        }

        private void toolStripMenuItemTurnOffControlOnExit_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.turnOffControlOnExit = toolStripMenuItemTurnOffControlOnExit.Checked;
            Properties.Settings.Default.Save();
        }

        private void toolStripMenuItemForbidUnsafeSettings_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.forbidUnsafeSettings = toolStripMenuItemForbidUnsafeSettings.Checked;
            Properties.Settings.Default.Save();
        }

        private void toolStripMenuItemMinimizeToTrayOnClose_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.minimizeToTrayOnClose = toolStripMenuItemMinimizeToTrayOnClose.Checked;
            Properties.Settings.Default.Save();
        }

        private void toolStripMenuItemAutoRefreshStats_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.autoRefreshStats = toolStripMenuItemAutoRefreshStats.Checked;
            Properties.Settings.Default.Save();
            timerRefreshStats();
        }

        private void toolStripMenuItemCheckForUpdates_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Karmel0x/AsusFanControl/releases");
        }

        private void setFanSpeed()
        {
            var value = trackBarFanSpeed.Value;
            Properties.Settings.Default.fanSpeed = value;
            Properties.Settings.Default.Save();

            if (!checkBoxTurnOn.Checked)
                value = 0;

            if (value == 0)
                labelValue.Text = "OFF";
            else
                labelValue.Text = value.ToString() + "%";

            if (fanSpeed == value)
                return;

            fanSpeed = value;
            asusControl.SetFanSpeeds(value);
        }

        private void checkBoxTurnOn_CheckedChanged(object sender, EventArgs e)
        {
            setFanSpeed();
        }

        private void trackBarFanSpeed_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.forbidUnsafeSettings)
            {
                if (trackBarFanSpeed.Value < 40)
                    trackBarFanSpeed.Value = 40;
                else if (trackBarFanSpeed.Value > 99)
                    trackBarFanSpeed.Value = 99;
            }

            setFanSpeed();
        }

        private void trackBarFanSpeed_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Left && e.KeyCode != Keys.Right)
                return;

            trackBarFanSpeed_MouseCaptureChanged(sender, e);
        }

        private void buttonRefreshRPM_Click(object sender, EventArgs e)
        {
            labelRPM.Text = string.Join(" ", asusControl.GetFanSpeeds());
        }

        private void buttonRefreshCPUTemp_Click(object sender, EventArgs e)
        {
            labelCPUTemp.Text = $"{asusControl.Thermal_Read_Cpu_Temperature()}°C";
        }
    }
}