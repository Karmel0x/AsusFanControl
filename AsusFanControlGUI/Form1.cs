using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AsusFanControl;
using AsusFanControlGUI.Controls;
using AsusFanControlGUI.Forms;
using AsusFanControlGUI.Models;
using AsusFanControlGUI.Services;

namespace AsusFanControlGUI
{
    public partial class Form1 : Form
    {
        // ── Existing Manual-tab state ────────────────────────────────────────────
        AsusControl asusControl = new AsusControl();
        int fanSpeed = 0;
        Timer timer;
        NotifyIcon trayIcon;

        // ── Fan Curves tab state ─────────────────────────────────────────────────
        private ProfileManager    _profileManager = new ProfileManager();
        private FanCurveDaemon    _daemon;
        private FanCurveProfile   _selectedProfile;
        private bool              _fanCurvesTabShownOnce = false;

        // ── Constructor ──────────────────────────────────────────────────────────
        public Form1()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            // Restore settings → Manual tab
            toolStripMenuItemTurnOffControlOnExit.Checked  = Properties.Settings.Default.turnOffControlOnExit;
            toolStripMenuItemForbidUnsafeSettings.Checked  = Properties.Settings.Default.forbidUnsafeSettings;
            toolStripMenuItemMinimizeToTrayOnClose.Checked = Properties.Settings.Default.minimizeToTrayOnClose;
            toolStripMenuItemAutoRefreshStats.Checked      = Properties.Settings.Default.autoRefreshStats;
            trackBarFanSpeed.Value = Properties.Settings.Default.fanSpeed;

            // Dark theme across the whole form
            ApplyDarkTheme();
        }

        // ── Form load ────────────────────────────────────────────────────────────
        private void Form1_Load(object sender, EventArgs e)
        {
            timerRefreshStats();
            LoadProfiles();
        }

        private void ApplyDarkTheme()
        {
            // Form itself
            BackColor = DarkTheme.Background;
            ForeColor = DarkTheme.TextPrimary;

            // Menu strip
            menuStrip1.BackColor = Color.FromArgb(18, 18, 36);
            menuStrip1.ForeColor = DarkTheme.TextPrimary;
            foreach (ToolStripItem item in menuStrip1.Items)
            {
                item.BackColor = Color.FromArgb(18, 18, 36);
                item.ForeColor = DarkTheme.TextPrimary;
            }

            // Tab control
            mainTabControl.BackColor = DarkTheme.Background;
            foreach (TabPage tp in mainTabControl.TabPages)
            {
                tp.BackColor = DarkTheme.Background;
                tp.ForeColor = DarkTheme.TextPrimary;
            }

            // Manual tab controls
            foreach (Control c in tabPageManual.Controls)
            {
                c.BackColor = DarkTheme.Background;
                c.ForeColor = DarkTheme.TextPrimary;
                if (c is Button b) DarkTheme.StyleButton(b);
                if (c is TrackBar tb) { tb.BackColor = DarkTheme.Background; }
                if (c is CheckBox cb) { cb.BackColor = Color.Transparent; }
            }

            // Fan Curves tab
            DarkTheme.Apply(tabPageFanCurves);

            // Style specific buttons
            DarkTheme.StyleButton(btnSaveProfile);
            DarkTheme.StyleButton(btnNewProfile);
            DarkTheme.StyleButton(btnDeleteProfile);
            DarkTheme.StyleButton(btnStartCurve);
            DarkTheme.StyleButton(btnStopCurve);
            DarkTheme.StyleComboBox(profileComboBox);
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  MANUAL TAB — existing logic (unchanged)
        // ═════════════════════════════════════════════════════════════════════════

        private void OnProcessExit(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.turnOffControlOnExit)
                asusControl.SetFanSpeeds(0);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Properties.Settings.Default.minimizeToTrayOnClose && Visible)
            {
                if (trayIcon == null)
                {
                    trayIcon = new NotifyIcon()
                    {
                        Icon = Icon,
                        ContextMenu = new ContextMenu(new MenuItem[]
                        {
                            new MenuItem("Show", (s1, e1) => { trayIcon.Visible = false; Show(); }),
                            new MenuItem("Exit", (s1, e1) => { Close(); trayIcon.Visible = false; Application.Exit(); }),
                        }),
                    };
                    trayIcon.MouseClick += (s1, e1) =>
                    {
                        if (e1.Button != MouseButtons.Left) return;
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
            if (timer != null) { timer.Stop(); timer = null; }
            if (!Properties.Settings.Default.autoRefreshStats) return;

            timer = new Timer { Interval = 2000 };
            timer.Tick += TimerEventProcessor;
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

            if (!checkBoxTurnOn.Checked) value = 0;

            labelValue.Text = value == 0 ? "turned off" : value.ToString();

            if (fanSpeed == value) return;
            fanSpeed = value;
            asusControl.SetFanSpeeds(value);
        }

        private void checkBoxTurnOn_CheckedChanged(object sender, EventArgs e) => setFanSpeed();

        private void trackBarFanSpeed_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.forbidUnsafeSettings)
            {
                if (trackBarFanSpeed.Value < 40) trackBarFanSpeed.Value = 40;
                else if (trackBarFanSpeed.Value > 99) trackBarFanSpeed.Value = 99;
            }
            setFanSpeed();
        }

        private void trackBarFanSpeed_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Left && e.KeyCode != Keys.Right) return;
            trackBarFanSpeed_MouseCaptureChanged(sender, e);
        }

        private void buttonRefreshRPM_Click(object sender, EventArgs e)
            => labelRPM.Text = string.Join(" ", asusControl.GetFanSpeeds());

        private void buttonRefreshCPUTemp_Click(object sender, EventArgs e)
            => labelCPUTemp.Text = $"{asusControl.Thermal_Read_Cpu_Temperature()}";

        // ═════════════════════════════════════════════════════════════════════════
        //  FAN CURVES TAB
        // ═════════════════════════════════════════════════════════════════════════

        private void mainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mainTabControl.SelectedTab == tabPageFanCurves && !_fanCurvesTabShownOnce)
            {
                _fanCurvesTabShownOnce = true;
                ElevationHelper.ShowElevationWarning(lblCurveStatus);
            }
        }

        // ── Profile loading ──────────────────────────────────────────────────────
        private void LoadProfiles()
        {
            try
            {
                var profiles = _profileManager.LoadAll();
                profileComboBox.Items.Clear();
                foreach (var p in profiles)
                    profileComboBox.Items.Add(p);

                if (profileComboBox.Items.Count > 0)
                    profileComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                SetStatus($"Error loading profiles: {ex.Message}", DarkTheme.ErrorRed);
            }
        }

        private void profileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedProfile = profileComboBox.SelectedItem as FanCurveProfile;
            if (_selectedProfile == null) return;

            bool isBuiltIn = _selectedProfile.IsBuiltIn;

            fanCurvePanelCpu.Points    = new List<CurvePoint>(_selectedProfile.CpuCurve);
            fanCurvePanelCpu.IsReadOnly = isBuiltIn;

            fanCurvePanelGpu.Points    = new List<CurvePoint>(_selectedProfile.GpuCurve);
            fanCurvePanelGpu.IsReadOnly = isBuiltIn;

            btnSaveProfile.Enabled   = !isBuiltIn;
            btnDeleteProfile.Enabled = !isBuiltIn;

            // Update daemon if running
            if (_daemon != null && _daemon.IsRunning)
                _daemon.ActiveProfile = _selectedProfile;

            SetStatus($"Profile: {_selectedProfile.Name}{(isBuiltIn ? " (read-only)" : " (custom)")}",
                      isBuiltIn ? DarkTheme.TextSecondary : DarkTheme.AccentBlue);
        }

        // ── Clamp to Grid ────────────────────────────────────────────────────────
        private void checkBoxClampToGrid_CheckedChanged(object sender, EventArgs e)
        {
            fanCurvePanelCpu.ClampToGrid = checkBoxClampToGrid.Checked;
            fanCurvePanelGpu.ClampToGrid = checkBoxClampToGrid.Checked;
        }

        // ── Save Profile ─────────────────────────────────────────────────────────
        private void btnSaveProfile_Click(object sender, EventArgs e)
        {
            if (_selectedProfile == null || _selectedProfile.IsBuiltIn) return;
            try
            {
                // Sync points from panels back to profile
                _selectedProfile.CpuCurve = new List<CurvePoint>(fanCurvePanelCpu.Points);
                _selectedProfile.GpuCurve = new List<CurvePoint>(fanCurvePanelGpu.Points);
                _profileManager.Save(_selectedProfile);
                SetStatus($"Saved: {_selectedProfile.Name}", DarkTheme.SuccessGreen);
            }
            catch (Exception ex)
            {
                SetStatus($"Save error: {ex.Message}", DarkTheme.ErrorRed);
            }
        }

        // ── New Profile ──────────────────────────────────────────────────────────
        private void btnNewProfile_Click(object sender, EventArgs e)
        {
            string name = ProfileNameDialog.Show("New Profile", "Enter a name for the new profile:", this);
            if (name == null) return;

            // Check for name collision
            foreach (var item in profileComboBox.Items)
            {
                if (item is FanCurveProfile p && p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("A profile with that name already exists.", "Duplicate Name",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Clone from current selection or Balanced
            var source = _selectedProfile ?? BuiltInProfiles.Balanced;
            var newProfile = source.Clone(name);

            try
            {
                _profileManager.Save(newProfile);
                profileComboBox.Items.Add(newProfile);
                profileComboBox.SelectedItem = newProfile;
                SetStatus($"Created: {name}", DarkTheme.SuccessGreen);
            }
            catch (Exception ex)
            {
                SetStatus($"Create error: {ex.Message}", DarkTheme.ErrorRed);
            }
        }

        // ── Delete Profile ───────────────────────────────────────────────────────
        private void btnDeleteProfile_Click(object sender, EventArgs e)
        {
            if (_selectedProfile == null || _selectedProfile.IsBuiltIn) return;

            var confirm = MessageBox.Show(
                $"Delete profile '{_selectedProfile.Name}'?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                _profileManager.Delete(_selectedProfile);
                int idx = profileComboBox.SelectedIndex;
                profileComboBox.Items.Remove(_selectedProfile);
                if (profileComboBox.Items.Count > 0)
                    profileComboBox.SelectedIndex = Math.Max(0, idx - 1);
                SetStatus("Profile deleted.", DarkTheme.TextSecondary);
            }
            catch (Exception ex)
            {
                SetStatus($"Delete error: {ex.Message}", DarkTheme.ErrorRed);
            }
        }

        // ── Start Curve Control ──────────────────────────────────────────────────
        private void btnStartCurve_Click(object sender, EventArgs e)
        {
            if (_selectedProfile == null)
            {
                SetStatus("Select a profile first.", DarkTheme.WarningOrange);
                return;
            }
            if (_daemon != null && _daemon.IsRunning)
                return;

            try
            {
                _daemon = new FanCurveDaemon(asusControl)
                {
                    ActiveProfile = _selectedProfile,
                    LetAsusControlAtLowTemps = true
                };
                _daemon.StatsUpdated += OnDaemonStatsUpdated;
                _daemon.ErrorOccurred += OnDaemonError;
                _daemon.Start();

                btnStartCurve.Enabled = false;
                btnStopCurve.Enabled  = true;
                SetStatus($"▶ Running — {_selectedProfile.Name}", DarkTheme.SuccessGreen);
            }
            catch (Exception ex)
            {
                SetStatus($"Start error: {ex.Message}", DarkTheme.ErrorRed);
            }
        }

        // ── Stop Curve Control ───────────────────────────────────────────────────
        private void btnStopCurve_Click(object sender, EventArgs e)
        {
            _daemon?.Stop();
            btnStartCurve.Enabled = true;
            btnStopCurve.Enabled  = false;
            lblLiveCpuTemp.Text   = "CPU: --°C";
            lblLiveRpm.Text       = "Fan: --%";
            SetStatus("■ Stopped", DarkTheme.TextSecondary);
        }

        // ── Daemon callbacks (thread-pool → UI thread via Invoke) ────────────────
        private void OnDaemonStatsUpdated(float cpuTempC, float fanPercent, float fanRpm)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnDaemonStatsUpdated(cpuTempC, fanPercent, fanRpm)));
                return;
            }
            lblLiveCpuTemp.Text = $"CPU: {cpuTempC:F0}°C";
            lblLiveRpm.Text     = $"Fan: {fanPercent:F0}%  ({fanRpm:F0} RPM)";
        }

        private void OnDaemonError(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnDaemonError(message)));
                return;
            }
            SetStatus($"Error: {message}", DarkTheme.ErrorRed);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────
        private void SetStatus(string text, Color color)
        {
            lblCurveStatus.Text      = text;
            lblCurveStatus.ForeColor = color;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _daemon?.Stop();
            base.OnFormClosed(e);
        }
    }
}
