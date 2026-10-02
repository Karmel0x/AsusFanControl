using AsusFanControl;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsusFanControlGUI
{
    public partial class Form1 : Form
    {
        AsusControl asusControl = new AsusControl();
        int fanSpeed = 0;
        Timer timer;
        NotifyIcon trayIcon;

        int? _pendingFanPercent;
        DateTime _pendingSinceUtc;
        const int DebounceSeconds = 5;

        readonly FanSetpoint[] _cpuPoints = new FanSetpoint[FanCurve.MaxSetpoints];
        readonly FanSetpoint[] _gpuPoints = new FanSetpoint[FanCurve.MaxSetpoints];
        int _pointCount = FanCurve.MaxSetpoints;
        bool _useGpu;
        int _editIndex;
        bool _loading = true;
        bool _updatingEditors;
        volatile bool _systemSuspended;

        public Form1()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            SystemEvents.PowerModeChanged += OnPowerModeChanged;

            toolStripMenuItemTurnOffControlOnExit.Checked = Properties.Settings.Default.turnOffControlOnExit;
            toolStripMenuItemForbidUnsafeSettings.Checked = Properties.Settings.Default.forbidUnsafeSettings;
            toolStripMenuItemMinimizeToTrayOnClose.Checked = Properties.Settings.Default.minimizeToTrayOnClose;
            toolStripMenuItemAutoRefreshStats.Checked = Properties.Settings.Default.autoRefreshStats;
            toolStripMenuItemFansOffOnSleep.Checked = Properties.Settings.Default.fansOffOnSleep;
            toolStripMenuItemStartWithWindows.Checked = Properties.Settings.Default.startWithWindows;
            trackBarFanSpeed.Value = Properties.Settings.Default.fanSpeed;

            LoadCurveSettings();
            comboBoxSetpointCount.SelectedIndex = _pointCount == FanCurve.MinSetpoints ? 0 : 1;
            comboBoxEditCurve.SelectedIndex = 0;
            checkBoxUseGpu.Checked = _useGpu;
            PushEditors();

            radioManual.Checked = !Properties.Settings.Default.useFanCurve;
            radioFanCurve.Checked = Properties.Settings.Default.useFanCurve;
            panelManual.Visible = radioManual.Checked;
            panelFanCurve.Visible = radioFanCurve.Checked;
            LayoutForMode();

            _loading = false;
            if (Properties.Settings.Default.startWithWindows)
                WindowsStartup.Enable(out _);
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.turnOffControlOnExit)
                asusControl.SetFanSpeeds(0);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            SystemEvents.PowerModeChanged -= OnPowerModeChanged;
            base.OnFormClosed(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timerRefreshStats();
            RefreshTemperatures(true);
        }

        private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend)
                HandleSleep();
            else if (e.Mode == PowerModes.Resume)
                HandleWake();
        }

        private void HandleSleep()
        {
            if (!Properties.Settings.Default.fansOffOnSleep)
                return;

            // Block the timer before releasing test mode so it cannot turn fans back on.
            _systemSuspended = true;
            try
            {
                asusControl.SetFanSpeedsNow(0);
            }
            catch (Exception)
            {
            }
        }

        private void HandleWake()
        {
            if (!_systemSuspended)
                return;

            _systemSuspended = false;
            if (!IsHandleCreated || IsDisposed)
                return;

            if (InvokeRequired)
                BeginInvoke(new Action(RestoreFansAfterWake));
            else
                RestoreFansAfterWake();
        }

        private void RestoreFansAfterWake()
        {
            _pendingFanPercent = null;
            fanSpeed = -1;
            setFanSpeed();
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
                        ContextMenu = new ContextMenu(new MenuItem[] {
                            new MenuItem("Show", (s1, e1) =>
                            {
                                trayIcon.Visible = false;
                                Show();
                            }),
                            new MenuItem("Exit", (s1, e1) =>
                            {
                                Close();
                                trayIcon.Visible = false;
                                Application.Exit();
                            }),
                        }),
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

        private const int TimerIntervalMs = 5000; // 5 seconds - reduces hardware polling and UI blocking

        private void timerRefreshStats()
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
            if (!Properties.Settings.Default.autoRefreshStats &&
                !(radioFanCurve.Checked && checkBoxTurnOn.Checked))
                return;

            timer = new Timer();
            timer.Interval = TimerIntervalMs;
            timer.Tick += TimerEventProcessor;
            timer.Start();
        }

        private void TimerEventProcessor(object sender, EventArgs e)
        {
            if (_systemSuspended)
                return;

            bool clampUnsafe = Properties.Settings.Default.forbidUnsafeSettings;
            bool inAuto = radioFanCurve.Checked;
            bool controlOn = checkBoxTurnOn.Checked;
            bool useGpu = _useGpu;
            int pointCount = _pointCount;
            FanSetpoint[] cpuPoints = CopyPoints(_cpuPoints);
            FanSetpoint[] gpuPoints = CopyPoints(_gpuPoints);

            Task.Run(() =>
            {
                int cpu = asusControl.GetCpuTemperatureCelsius();
                int gpu = asusControl.GetGpuTemperatureCelsius();
                string rpmText = string.Join(" ", asusControl.GetFanSpeeds());

                int cpuPercent = inAuto && cpu >= 0
                    ? FanCurve.GetFanPercent(cpu, cpuPoints, pointCount, clampUnsafe)
                    : -1;
                int gpuPercent = inAuto && useGpu && gpu >= 0
                    ? FanCurve.GetFanPercent(gpu, gpuPoints, pointCount, clampUnsafe)
                    : -1;
                int newPercent = inAuto ? FanCurve.Combine(cpuPercent, gpuPercent) : -1;

                if (!IsHandleCreated || IsDisposed)
                    return;

                try
                {
                    BeginInvoke(new Action(() =>
                    {
                        if (IsDisposed)
                            return;

                        labelRPM.Text = rpmText;
                        ShowTemperatures(cpu, gpu, newPercent);
                        if (_systemSuspended || !inAuto || !controlOn)
                            return;
                        ApplyDebouncedFanPercent(newPercent);
                    }));
                }
                catch (InvalidOperationException)
                {
                }
            });
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

        private void toolStripMenuItemStartWithWindows_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;

            bool enable = toolStripMenuItemStartWithWindows.Checked;
            if (enable)
            {
                string error;
                if (!WindowsStartup.Enable(out error))
                {
                    _loading = true;
                    toolStripMenuItemStartWithWindows.Checked = false;
                    _loading = false;
                    Properties.Settings.Default.startWithWindows = false;
                    Properties.Settings.Default.Save();
                    MessageBox.Show(this,
                        "Could not turn on start with Windows.\r\n\r\n" + error,
                        "Asus Fan Control",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                WindowsStartup.Disable();
            }

            Properties.Settings.Default.startWithWindows = enable;
            Properties.Settings.Default.Save();
        }

        private void toolStripMenuItemFansOffOnSleep_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            Properties.Settings.Default.fansOffOnSleep = toolStripMenuItemFansOffOnSleep.Checked;
            Properties.Settings.Default.Save();
        }

        private void toolStripMenuItemCheckForUpdates_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Karmel0x/AsusFanControl/releases");
        }

        private void setFanSpeed()
        {
            if (_systemSuspended)
                return;

            if (!checkBoxTurnOn.Checked)
            {
                if (fanSpeed != 0)
                {
                    fanSpeed = 0;
                    asusControl.SetFanSpeeds(0);
                }
                _pendingFanPercent = null;
                labelValue.Text = "turned off";
                return;
            }

            if (radioManual.Checked)
            {
                var value = trackBarFanSpeed.Value;
                Properties.Settings.Default.fanSpeed = value;
                Properties.Settings.Default.Save();
                if (value == 0)
                    labelValue.Text = "turned off";
                else
                    labelValue.Text = value.ToString();
                ApplyFanPercentNow(value);
            }
            else if (radioFanCurve.Checked)
            {
                int cpu = asusControl.GetCpuTemperatureCelsius();
                int gpu = asusControl.GetGpuTemperatureCelsius();
                int percent = EvaluateFanPercent(cpu, gpu);
                ShowTemperatures(cpu, gpu, percent);
                ApplyFanPercentNow(percent);
            }
        }

        private void ApplyFanPercentNow(int percent)
        {
            if (_systemSuspended || percent < 0)
                return;
            _pendingFanPercent = null;
            if (fanSpeed == percent)
                return;
            fanSpeed = percent;
            asusControl.SetFanSpeeds(percent);
        }

        private void ApplyDebouncedFanPercent(int percent)
        {
            if (_systemSuspended || percent < 0 || !checkBoxTurnOn.Checked)
                return;

            if (percent == fanSpeed)
            {
                _pendingFanPercent = null;
                return;
            }

            DateTime now = DateTime.UtcNow;
            if (percent != _pendingFanPercent)
            {
                _pendingFanPercent = percent;
                _pendingSinceUtc = now;
                return;
            }

            if ((now - _pendingSinceUtc).TotalSeconds < DebounceSeconds)
                return;

            fanSpeed = percent;
            asusControl.SetFanSpeeds(percent);
            _pendingFanPercent = null;
        }

        private void checkBoxTurnOn_CheckedChanged(object sender, EventArgs e)
        {
            setFanSpeed();
            timerRefreshStats();
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
            if (e.KeyCode != Keys.Left && e.KeyCode != Keys.Right) return;
            trackBarFanSpeed_MouseCaptureChanged(sender, e);
        }

        private void buttonRefreshRPM_Click(object sender, EventArgs e)
        {
            labelRPM.Text = string.Join(" ", asusControl.GetFanSpeeds());
        }

        private void buttonRefreshCPUTemp_Click(object sender, EventArgs e)
        {
            RefreshTemperatures(true);
        }

        private void buttonRefreshGPUTemp_Click(object sender, EventArgs e)
        {
            RefreshTemperatures(true);
        }

        private void RefreshTemperatures(bool applyNow)
        {
            int cpu = asusControl.GetCpuTemperatureCelsius();
            int gpu = asusControl.GetGpuTemperatureCelsius();
            int percent = radioFanCurve.Checked ? EvaluateFanPercent(cpu, gpu) : -1;
            ShowTemperatures(cpu, gpu, percent);
            if (applyNow && radioFanCurve.Checked && checkBoxTurnOn.Checked)
                ApplyFanPercentNow(percent);
        }

        private void radioMode_CheckedChanged(object sender, EventArgs e)
        {
            var radio = sender as RadioButton;
            if (radio != null && !radio.Checked)
                return;

            panelManual.Visible = radioManual.Checked;
            panelFanCurve.Visible = radioFanCurve.Checked;
            LayoutForMode();
            Properties.Settings.Default.useFanCurve = radioFanCurve.Checked;
            Properties.Settings.Default.Save();
            _pendingFanPercent = null;
            timerRefreshStats();
            setFanSpeed();
        }

        private void curveEditor_Changed(object sender, EventArgs e)
        {
            if (_loading || _updatingEditors)
                return;

            PullEditors();
            PushEditors();
            SaveCurveSettings();
            if (radioFanCurve.Checked)
                setFanSpeed();
        }

        private void editCurve_Changed(object sender, EventArgs e)
        {
            if (_loading || _updatingEditors)
                return;

            PullEditors();
            _editIndex = comboBoxEditCurve.SelectedIndex == 1 ? 1 : 0;
            PushEditors();
            SaveCurveSettings();
            if (radioFanCurve.Checked)
                setFanSpeed();
        }

        private void setpointCount_Changed(object sender, EventArgs e)
        {
            if (_loading || _updatingEditors)
                return;

            int newCount = comboBoxSetpointCount.SelectedIndex == 0
                ? FanCurve.MinSetpoints
                : FanCurve.MaxSetpoints;
            if (newCount == _pointCount)
                return;

            PullEditors();
            if (newCount == FanCurve.MinSetpoints)
            {
                _cpuPoints[1] = _cpuPoints[2];
                _gpuPoints[1] = _gpuPoints[2];
            }
            else
            {
                _cpuPoints[2] = _cpuPoints[1];
                _gpuPoints[2] = _gpuPoints[1];
                _cpuPoints[1] = FanCurve.Midpoint(_cpuPoints[0], _cpuPoints[2]);
                _gpuPoints[1] = FanCurve.Midpoint(_gpuPoints[0], _gpuPoints[2]);
            }

            _pointCount = newCount;
            FanCurve.Normalize(_cpuPoints, _pointCount);
            FanCurve.Normalize(_gpuPoints, _pointCount);
            PushEditors();
            SaveCurveSettings();
            if (radioFanCurve.Checked)
                setFanSpeed();
        }

        private void useGpu_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;

            _useGpu = checkBoxUseGpu.Checked;
            SaveCurveSettings();
            if (radioFanCurve.Checked)
                setFanSpeed();
        }

        private void LoadCurveSettings()
        {
            var settings = Properties.Settings.Default;
            _pointCount = FanCurve.ClampCount(settings.fanCurvePointCount);
            _useGpu = settings.useGpuCurve;
            _cpuPoints[0] = new FanSetpoint(settings.cpuCurveTemp1, settings.cpuCurveFan1);
            _cpuPoints[1] = new FanSetpoint(settings.cpuCurveTemp2, settings.cpuCurveFan2);
            _cpuPoints[2] = new FanSetpoint(settings.cpuCurveTemp3, settings.cpuCurveFan3);
            _gpuPoints[0] = new FanSetpoint(settings.gpuCurveTemp1, settings.gpuCurveFan1);
            _gpuPoints[1] = new FanSetpoint(settings.gpuCurveTemp2, settings.gpuCurveFan2);
            _gpuPoints[2] = new FanSetpoint(settings.gpuCurveTemp3, settings.gpuCurveFan3);
            FanCurve.Normalize(_cpuPoints, _pointCount);
            FanCurve.Normalize(_gpuPoints, _pointCount);
        }

        private void SaveCurveSettings()
        {
            var settings = Properties.Settings.Default;
            settings.fanCurvePointCount = _pointCount;
            settings.useGpuCurve = _useGpu;
            settings.cpuCurveTemp1 = _cpuPoints[0].TemperatureC;
            settings.cpuCurveFan1 = _cpuPoints[0].FanPercent;
            settings.cpuCurveTemp2 = _cpuPoints[1].TemperatureC;
            settings.cpuCurveFan2 = _cpuPoints[1].FanPercent;
            settings.cpuCurveTemp3 = _cpuPoints[2].TemperatureC;
            settings.cpuCurveFan3 = _cpuPoints[2].FanPercent;
            settings.gpuCurveTemp1 = _gpuPoints[0].TemperatureC;
            settings.gpuCurveFan1 = _gpuPoints[0].FanPercent;
            settings.gpuCurveTemp2 = _gpuPoints[1].TemperatureC;
            settings.gpuCurveFan2 = _gpuPoints[1].FanPercent;
            settings.gpuCurveTemp3 = _gpuPoints[2].TemperatureC;
            settings.gpuCurveFan3 = _gpuPoints[2].FanPercent;
            settings.Save();
        }

        private FanSetpoint[] ActivePoints()
        {
            return _editIndex == 1 ? _gpuPoints : _cpuPoints;
        }

        private void PullEditors()
        {
            var points = ActivePoints();
            points[0].TemperatureC = (int)numericTemp1.Value;
            points[0].FanPercent = (int)numericFan1.Value;
            points[1].TemperatureC = (int)numericTemp2.Value;
            points[1].FanPercent = (int)numericFan2.Value;
            if (_pointCount >= FanCurve.MaxSetpoints)
            {
                points[2].TemperatureC = (int)numericTemp3.Value;
                points[2].FanPercent = (int)numericFan3.Value;
            }
            FanCurve.Normalize(points, _pointCount);
        }

        private void PushEditors()
        {
            var points = ActivePoints();
            bool third = _pointCount >= FanCurve.MaxSetpoints;
            _updatingEditors = true;
            try
            {
                numericTemp1.Value = points[0].TemperatureC;
                numericFan1.Value = points[0].FanPercent;
                numericTemp2.Value = points[1].TemperatureC;
                numericFan2.Value = points[1].FanPercent;
                numericTemp3.Value = points[2].TemperatureC;
                numericFan3.Value = points[2].FanPercent;
                labelPoint3.Visible = third;
                numericTemp3.Visible = third;
                numericFan3.Visible = third;
            }
            finally
            {
                _updatingEditors = false;
            }
        }

        private int EvaluateFanPercent(int cpu, int gpu)
        {
            bool clampUnsafe = Properties.Settings.Default.forbidUnsafeSettings;
            int cpuPercent = cpu >= 0
                ? FanCurve.GetFanPercent(cpu, _cpuPoints, _pointCount, clampUnsafe)
                : -1;
            int gpuPercent = _useGpu && gpu >= 0
                ? FanCurve.GetFanPercent(gpu, _gpuPoints, _pointCount, clampUnsafe)
                : -1;
            return FanCurve.Combine(cpuPercent, gpuPercent);
        }

        private void ShowTemperatures(int cpu, int gpu, int percent)
        {
            labelCPUTemp.Text = cpu >= 0 ? cpu.ToString() : "N/A";
            labelGPUTemp.Text = gpu >= 0 ? gpu.ToString() : "N/A";
            if (radioFanCurve.Checked)
                labelCurveStatus.Text = FormatStatus(cpu, gpu, percent);
        }

        private static string FormatStatus(int cpu, int gpu, int percent)
        {
            string fan = percent >= 0 ? percent + "%" : "--%";
            return "Current: CPU " + FormatTemp(cpu) + ", GPU " + FormatTemp(gpu) + " → Fan " + fan;
        }

        private static string FormatTemp(int value)
        {
            return value >= 0 ? value + " °C" : "N/A";
        }

        private void LayoutForMode()
        {
            int panelHeight = radioFanCurve.Checked ? 190 : 85;
            panelManual.Height = panelHeight;
            panelFanCurve.Height = panelHeight;

            int statsTop = panelManual.Top + panelHeight + 8;
            PlaceStatRow(statsTop, buttonRefreshRPM, label2, labelRPM);
            PlaceStatRow(statsTop + 28, buttonRefreshCPUTemp, label4, labelCPUTemp);
            PlaceStatRow(statsTop + 56, buttonRefreshGPUTemp, labelGpuCaption, labelGPUTemp);
            ClientSize = new System.Drawing.Size(392, statsTop + 56 + 23 + 16);
        }

        private static void PlaceStatRow(int y, Button button, Label caption, Label value)
        {
            button.Top = y;
            caption.Top = y + 5;
            value.Top = y + 5;
        }

        private static FanSetpoint[] CopyPoints(FanSetpoint[] source)
        {
            var copy = new FanSetpoint[source.Length];
            Array.Copy(source, copy, source.Length);
            return copy;
        }
    }
}
