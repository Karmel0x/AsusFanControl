using AsusFanControl;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AsusFanControlGUI
{
    public partial class Form1 : Form
    {
        private const int CurvePointCount = 5;

        private readonly AsusControl asusControl = new AsusControl();
        private readonly bool startMinimized;
        private readonly CheckBox checkBoxUseTemperatureCurve = new CheckBox();
        private readonly GroupBox groupBoxTemperatureCurve = new GroupBox();
        private readonly FanCurveGraphControl fanCurveGraph = new FanCurveGraphControl();
        private readonly Label labelCurveHint = new Label();
        private ToolStripMenuItem toolStripMenuItemLaunchOnWindowsSignIn;

        private int fanSpeed = -1;
        private Timer timer;
        private NotifyIcon trayIcon;
        private bool hardwareAccessAvailable = true;
        private bool launchContextWarningShown;
        private bool isLoadingSettings;
        private bool isSyncingCurveEditorValues;
        private bool isSyncingStartupRegistration;

        public Form1(bool startMinimized = false)
        {
            this.startMinimized = startMinimized;

            InitializeComponent();
            InitializeDynamicControls();
            InitializeCurveEditors();
            InitializeTrayIcon();

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            LoadSettingsIntoControls();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (startMinimized)
                BeginInvoke(new Action(HideToTray));
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.turnOffControlOnExit)
                asusControl.SetFanSpeeds(0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SyncStartupRegistrationState();
            ProbeHardwareAccess();
            ApplyControlState();

            if (Properties.Settings.Default.autoRefreshStats)
                RefreshFanSpeedLabel();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Properties.Settings.Default.minimizeToTrayOnClose && Visible)
            {
                e.Cancel = true;
                HideToTray();
                return;
            }

            if (trayIcon != null)
                trayIcon.Visible = false;
        }

        private void InitializeDynamicControls()
        {
            ClientSize = new Size(360, 430);

            checkBoxUseTemperatureCurve.AutoSize = true;
            checkBoxUseTemperatureCurve.Location = new Point(12, 194);
            checkBoxUseTemperatureCurve.Name = "checkBoxUseTemperatureCurve";
            checkBoxUseTemperatureCurve.Size = new Size(130, 17);
            checkBoxUseTemperatureCurve.TabIndex = 11;
            checkBoxUseTemperatureCurve.Text = "Use temperature curve";
            checkBoxUseTemperatureCurve.UseVisualStyleBackColor = true;
            checkBoxUseTemperatureCurve.CheckedChanged += checkBoxUseTemperatureCurve_CheckedChanged;

            groupBoxTemperatureCurve.Location = new Point(12, 220);
            groupBoxTemperatureCurve.Name = "groupBoxTemperatureCurve";
            groupBoxTemperatureCurve.Size = new Size(336, 198);
            groupBoxTemperatureCurve.TabIndex = 12;
            groupBoxTemperatureCurve.TabStop = false;
            groupBoxTemperatureCurve.Text = "Temperature curve";

            fanCurveGraph.Location = new Point(10, 20);
            fanCurveGraph.Name = "fanCurveGraph";
            fanCurveGraph.Size = new Size(316, 140);
            fanCurveGraph.TabIndex = 0;
            fanCurveGraph.CurveChanged += curveEditor_ValueChanged;

            labelCurveHint.Location = new Point(10, 164);
            labelCurveHint.Name = "labelCurveHint";
            labelCurveHint.Size = new Size(316, 28);
            labelCurveHint.TabIndex = 1;
            labelCurveHint.Text = "Drag the five points to set temperature (horizontal) and fan speed (vertical).";

            groupBoxTemperatureCurve.Controls.Add(fanCurveGraph);
            groupBoxTemperatureCurve.Controls.Add(labelCurveHint);
            Controls.Add(checkBoxUseTemperatureCurve);
            Controls.Add(groupBoxTemperatureCurve);

            toolStripMenuItemLaunchOnWindowsSignIn = new ToolStripMenuItem();
            toolStripMenuItemLaunchOnWindowsSignIn.CheckOnClick = true;
            toolStripMenuItemLaunchOnWindowsSignIn.Name = "toolStripMenuItemLaunchOnWindowsSignIn";
            toolStripMenuItemLaunchOnWindowsSignIn.Size = new Size(207, 22);
            toolStripMenuItemLaunchOnWindowsSignIn.Text = "Launch on Windows sign-in";
            toolStripMenuItemLaunchOnWindowsSignIn.CheckedChanged += toolStripMenuItemLaunchOnWindowsSignIn_CheckedChanged;

            toolStripMenuItem1.DropDownItems.Insert(0, toolStripMenuItemLaunchOnWindowsSignIn);
        }

        private void InitializeCurveEditors()
        {
            UpdateCurveEditorValues(FanCurve.Default);
        }

        private void InitializeTrayIcon()
        {
            trayIcon = new NotifyIcon()
            {
                Icon = Icon,
                Text = "Asus Fan Control",
                ContextMenu = new ContextMenu(new[]
                {
                    new MenuItem("Show", (sender, e) => ShowMainWindow()),
                    new MenuItem("Exit", (sender, e) =>
                    {
                        trayIcon.Visible = false;
                        Close();
                        Application.Exit();
                    }),
                }),
                Visible = false,
            };

            components?.Add(trayIcon);

            trayIcon.MouseClick += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                    ShowMainWindow();
            };
        }

        private void LoadSettingsIntoControls()
        {
            isLoadingSettings = true;

            try
            {
                var settingsChanged = false;

                toolStripMenuItemTurnOffControlOnExit.Checked = Properties.Settings.Default.turnOffControlOnExit;
                toolStripMenuItemForbidUnsafeSettings.Checked = Properties.Settings.Default.forbidUnsafeSettings;
                toolStripMenuItemMinimizeToTrayOnClose.Checked = Properties.Settings.Default.minimizeToTrayOnClose;
                toolStripMenuItemAutoRefreshStats.Checked = Properties.Settings.Default.autoRefreshStats;

                var safeManualFanSpeed = ClampFanPercentForSafety(Properties.Settings.Default.fanSpeed);
                trackBarFanSpeed.Value = safeManualFanSpeed;
                if (Properties.Settings.Default.fanSpeed != safeManualFanSpeed)
                {
                    Properties.Settings.Default.fanSpeed = safeManualFanSpeed;
                    settingsChanged = true;
                }

                checkBoxTurnOn.Checked = Properties.Settings.Default.fanControlEnabled;
                checkBoxUseTemperatureCurve.Checked = Properties.Settings.Default.temperatureCurveEnabled;

                fanCurveGraph.MinimumFanSpeed = GetMinimumFanSpeed();

                var savedCurve = FanCurve.ParseOrDefault(Properties.Settings.Default.temperatureCurve);
                UpdateCurveEditorValues(savedCurve);

                var normalizedCurve = fanCurveGraph.Curve;
                var serializedCurve = normalizedCurve.Serialize();
                if (!string.Equals(Properties.Settings.Default.temperatureCurve, serializedCurve, StringComparison.Ordinal))
                {
                    Properties.Settings.Default.temperatureCurve = serializedCurve;
                    settingsChanged = true;
                }

                if (settingsChanged)
                    Properties.Settings.Default.Save();
            }
            finally
            {
                isLoadingSettings = false;
                UpdateCurveUiState();
            }
        }

        private void SyncStartupRegistrationState()
        {
            try
            {
                var isEnabled = StartupRegistration.IsEnabled();
                if (isEnabled)
                    StartupRegistration.RefreshCurrentExecutablePath();

                isSyncingStartupRegistration = true;
                toolStripMenuItemLaunchOnWindowsSignIn.Checked = isEnabled;
            }
            catch (Exception ex)
            {
                isSyncingStartupRegistration = true;
                toolStripMenuItemLaunchOnWindowsSignIn.Checked = false;
                MessageBox.Show(this, ex.Message, "Startup registration error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                isSyncingStartupRegistration = false;
            }
        }

        private void ProbeHardwareAccess()
        {
            var fanCount = asusControl.HealthyTable_FanCounts();
            var cpuTemperature = asusControl.Thermal_Read_Cpu_Temperature();

            hardwareAccessAvailable = fanCount > 0 || cpuTemperature > 0;
            if (hardwareAccessAvailable)
                return;

            labelRPM.Text = "launch via run.bat";
            labelCPUTemp.Text = "launch via run.bat";
            labelValue.Text = "launcher required";

            if (!launchContextWarningShown)
            {
                launchContextWarningShown = true;
                BeginInvoke(new Action(() =>
                    MessageBox.Show(this, StartupRegistration.GetLaunchRequirementMessage(), "Launch required", MessageBoxButtons.OK, MessageBoxIcon.Warning)));
            }
        }

        private void ShowMainWindow()
        {
            trayIcon.Visible = false;
            ShowInTaskbar = true;
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void HideToTray()
        {
            trayIcon.Visible = true;
            ShowInTaskbar = false;
            Hide();
        }

        private void RefreshPollingTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= TimerEventProcessor;
                timer.Dispose();
                timer = null;
            }

            if (!hardwareAccessAvailable)
                return;

            if (!Properties.Settings.Default.autoRefreshStats && !IsAutomaticCurveActive())
                return;

            timer = new Timer();
            timer.Interval = 2000;
            timer.Tick += TimerEventProcessor;
            timer.Start();
        }

        private void TimerEventProcessor(object sender, EventArgs e)
        {
            RunMonitoringCycle();
        }

        private void toolStripMenuItemTurnOffControlOnExit_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingSettings)
                return;

            Properties.Settings.Default.turnOffControlOnExit = toolStripMenuItemTurnOffControlOnExit.Checked;
            Properties.Settings.Default.Save();
        }

        private void toolStripMenuItemForbidUnsafeSettings_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingSettings)
                return;

            Properties.Settings.Default.forbidUnsafeSettings = toolStripMenuItemForbidUnsafeSettings.Checked;
            trackBarFanSpeed.Value = ClampFanPercentForSafety(trackBarFanSpeed.Value);
            Properties.Settings.Default.fanSpeed = trackBarFanSpeed.Value;
            UpdateCurveEditorMinimumFanSpeed();
            SaveCurveSettings();
            Properties.Settings.Default.Save();

            ApplyControlState();
        }

        private void toolStripMenuItemMinimizeToTrayOnClose_Click(object sender, EventArgs e)
        {
            if (isLoadingSettings)
                return;

            Properties.Settings.Default.minimizeToTrayOnClose = toolStripMenuItemMinimizeToTrayOnClose.Checked;
            Properties.Settings.Default.Save();
        }

        private void toolStripMenuItemAutoRefreshStats_Click(object sender, EventArgs e)
        {
            if (isLoadingSettings)
                return;

            Properties.Settings.Default.autoRefreshStats = toolStripMenuItemAutoRefreshStats.Checked;
            Properties.Settings.Default.Save();

            RefreshPollingTimer();

            if (Properties.Settings.Default.autoRefreshStats)
                RunMonitoringCycle();
        }

        private void toolStripMenuItemCheckForUpdates_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Karmel0x/AsusFanControl/releases");
        }

        private void toolStripMenuItemLaunchOnWindowsSignIn_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingSettings || isSyncingStartupRegistration)
                return;

            try
            {
                if (toolStripMenuItemLaunchOnWindowsSignIn.Checked && !StartupRegistration.CanUseSystemLauncher(out var errorMessage))
                    throw new InvalidOperationException(errorMessage);

                StartupRegistration.SetEnabled(toolStripMenuItemLaunchOnWindowsSignIn.Checked);
                SyncStartupRegistrationState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Startup registration error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SyncStartupRegistrationState();
            }
        }

        private void checkBoxUseTemperatureCurve_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingSettings)
                return;

            Properties.Settings.Default.temperatureCurveEnabled = checkBoxUseTemperatureCurve.Checked;
            Properties.Settings.Default.Save();

            UpdateCurveUiState();
            ApplyControlState();
        }

        private void curveEditor_ValueChanged(object sender, EventArgs e)
        {
            if (isLoadingSettings || isSyncingCurveEditorValues)
                return;

            SaveCurveSettings();

            if (checkBoxUseTemperatureCurve.Checked)
                ApplyControlState();
        }

        private void checkBoxTurnOn_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingSettings)
                return;

            Properties.Settings.Default.fanControlEnabled = checkBoxTurnOn.Checked;
            Properties.Settings.Default.Save();

            ApplyControlState();
        }

        private void trackBarFanSpeed_MouseCaptureChanged(object sender, EventArgs e)
        {
            trackBarFanSpeed.Value = ClampFanPercentForSafety(trackBarFanSpeed.Value);
            Properties.Settings.Default.fanSpeed = trackBarFanSpeed.Value;
            Properties.Settings.Default.Save();

            ApplyControlState();
        }

        private void trackBarFanSpeed_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Left && e.KeyCode != Keys.Right)
                return;

            trackBarFanSpeed_MouseCaptureChanged(sender, e);
        }

        private void buttonRefreshRPM_Click(object sender, EventArgs e)
        {
            RefreshFanSpeedLabel();
        }

        private void buttonRefreshCPUTemp_Click(object sender, EventArgs e)
        {
            var cpuTemperature = RefreshCpuTemperatureLabel();
            if (IsAutomaticCurveActive())
                ApplyAutomaticCurve(cpuTemperature);
        }

        private void RunMonitoringCycle()
        {
            ulong? cpuTemperature = null;

            if (Properties.Settings.Default.autoRefreshStats || IsAutomaticCurveActive())
                cpuTemperature = RefreshCpuTemperatureLabel();

            if (IsAutomaticCurveActive() && cpuTemperature.HasValue)
                ApplyAutomaticCurve(cpuTemperature.Value);

            if (Properties.Settings.Default.autoRefreshStats)
                RefreshFanSpeedLabel();
        }

        private void RefreshFanSpeedLabel()
        {
            if (!hardwareAccessAvailable)
            {
                labelRPM.Text = "launch via run.bat";
                return;
            }

            labelRPM.Text = string.Join(" ", asusControl.GetFanSpeeds());
        }

        private ulong RefreshCpuTemperatureLabel()
        {
            if (!hardwareAccessAvailable)
            {
                labelCPUTemp.Text = "launch via run.bat";
                return 0;
            }

            var cpuTemperature = asusControl.Thermal_Read_Cpu_Temperature();
            labelCPUTemp.Text = string.Format("{0} C", cpuTemperature);
            return cpuTemperature;
        }

        private void ApplyControlState()
        {
            UpdateCurveUiState();
            RefreshPollingTimer();

            if (!hardwareAccessAvailable)
            {
                labelValue.Text = "launcher required";
                return;
            }

            if (!checkBoxTurnOn.Checked)
            {
                ApplyFanSpeed(0, "turned off");
                return;
            }

            if (checkBoxUseTemperatureCurve.Checked)
            {
                ApplyAutomaticCurve(RefreshCpuTemperatureLabel());
                return;
            }

            var manualFanSpeed = ClampFanPercentForSafety(trackBarFanSpeed.Value);
            trackBarFanSpeed.Value = manualFanSpeed;
            ApplyFanSpeed(manualFanSpeed, string.Format("{0}%", manualFanSpeed));
        }

        private void ApplyAutomaticCurve(ulong cpuTemperature)
        {
            var curve = ReadCurveFromEditor();
            var targetFanSpeed = ClampFanPercentForSafety(curve.GetFanSpeed(
                (int)Math.Min(cpuTemperature, (ulong)int.MaxValue),
                FanCurveGraphControl.MinimumTemperature,
                FanCurveGraphControl.MaximumTemperature,
                GetMinimumFanSpeed(),
                FanCurveGraphControl.MaximumFanSpeed));
            ApplyFanSpeed(targetFanSpeed, string.Format("{0}% (curve)", targetFanSpeed));
        }

        private void ApplyFanSpeed(int value, string labelText)
        {
            labelValue.Text = labelText;

            if (fanSpeed == value)
                return;

            fanSpeed = value;
            asusControl.SetFanSpeeds(value);
        }

        private bool IsAutomaticCurveActive()
        {
            return checkBoxTurnOn.Checked && checkBoxUseTemperatureCurve.Checked;
        }

        private void UpdateCurveUiState()
        {
            groupBoxTemperatureCurve.Enabled = checkBoxUseTemperatureCurve.Checked;
            trackBarFanSpeed.Enabled = !checkBoxUseTemperatureCurve.Checked;
        }

        private void SaveCurveSettings()
        {
            var curve = ReadCurveFromEditor();
            UpdateCurveEditorValues(curve);
            Properties.Settings.Default.temperatureCurve = curve.Serialize();
            Properties.Settings.Default.Save();
        }

        private FanCurve ReadCurveFromEditor()
        {
            return fanCurveGraph.Curve;
        }

        private void UpdateCurveEditorValues(FanCurve curve)
        {
            if (curve == null || curve.Points.Count != CurvePointCount)
                curve = FanCurve.Default;

            isSyncingCurveEditorValues = true;
            try
            {
                fanCurveGraph.MinimumFanSpeed = GetMinimumFanSpeed();
                fanCurveGraph.Curve = curve;
                UpdateCurveSummaryLabel(fanCurveGraph.Curve);
            }
            finally
            {
                isSyncingCurveEditorValues = false;
            }
        }

        private void UpdateCurveEditorMinimumFanSpeed()
        {
            isSyncingCurveEditorValues = true;
            try
            {
                fanCurveGraph.MinimumFanSpeed = GetMinimumFanSpeed();
                UpdateCurveSummaryLabel(fanCurveGraph.Curve);
            }
            finally
            {
                isSyncingCurveEditorValues = false;
            }
        }

        private void UpdateCurveSummaryLabel(FanCurve curve)
        {
            var activeCurve = curve ?? FanCurve.Default;
            var summary = string.Join("  ", activeCurve.Points.Select((point, index) =>
                string.Format("P{0} {1}C/{2}%", index + 1, point.Temperature, point.FanSpeed)));
            labelCurveHint.Text = string.Format("Drag the five points. {0}", summary);
        }

        private int GetMinimumFanSpeed()
        {
            return Properties.Settings.Default.forbidUnsafeSettings ? 40 : 0;
        }

        private int ClampFanPercentForSafety(int value)
        {
            var clampedValue = Math.Max(0, Math.Min(100, value));
            if (!Properties.Settings.Default.forbidUnsafeSettings)
                return clampedValue;

            return Math.Max(40, Math.Min(99, clampedValue));
        }
    }
}
