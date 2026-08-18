using AsusFanControl;
using System;
using System.Windows.Forms;

namespace AsusFanControlGUI
{
    public partial class Form1 : Form
    {
        AsusControl asusControl = new AsusControl();
        FanCurve fanCurve = new FanCurve();
        int fanSpeed = 0;
        Timer timer;
        NotifyIcon trayIcon;
        bool fanCurveMode = false;
        int lastReadTemp = 0;

        public Form1()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            toolStripMenuItemTurnOffControlOnExit.Checked = Properties.Settings.Default.turnOffControlOnExit;
            toolStripMenuItemForbidUnsafeSettings.Checked = Properties.Settings.Default.forbidUnsafeSettings;
            toolStripMenuItemMinimizeToTrayOnClose.Checked = Properties.Settings.Default.minimizeToTrayOnClose;
            toolStripMenuItemAutoRefreshStats.Checked = Properties.Settings.Default.autoRefreshStats;
            trackBarFanSpeed.Value = Properties.Settings.Default.fanSpeed;
            
            fanCurve = FanCurve.DeserializeFromString(Properties.Settings.Default.fanCurveData);
            fanCurveMode = Properties.Settings.Default.fanCurveEnabled;
            fanCurve.IsEnabled = fanCurveMode;
            checkBoxFanCurve.Checked = fanCurveMode;
            
            // Populate the refresh interval menu label
            UpdateRefreshIntervalMenuLabel();
            
            if (fanCurveMode)
            {
                trackBarFanSpeed.Enabled = false;
            }
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
                    var showItem = new ToolStripMenuItem("Show");
                    showItem.Click += (s1, e1) =>
                    {
                        trayIcon.Visible = false;
                        Show();
                    };

                    var exitItem = new ToolStripMenuItem("Exit");
                    exitItem.Click += (s1, e1) =>
                    {
                        Close();
                        trayIcon.Visible = false;
                        Application.Exit();
                    };

                    var trayMenu = new ContextMenuStrip();
                    trayMenu.Items.Add(showItem);
                    trayMenu.Items.Add(exitItem);

                    trayIcon = new NotifyIcon()
                    {
                        Icon = Icon,
                        ContextMenuStrip = trayMenu,
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
            timer.Interval = Properties.Settings.Default.refreshIntervalMs;
            timer.Tick += new EventHandler(TimerEventProcessor);
            timer.Start();
        }

        private void TimerEventProcessor(object sender, EventArgs e)
        {
            // Fetch RPM and temperature into the UI labels first
            buttonRefreshRPM_Click(sender, e);
            buttonRefreshCPUTemp_Click(sender, e);
            
            if (fanCurveMode && fanCurve.IsEnabled)
            {
                // Use the UI-level temperature that was just read above.
                // This avoids a second system-level call and keeps the fan curve
                // operating on the same data the user sees on screen.
                var targetSpeed = fanCurve.GetFanSpeedForTemperature(lastReadTemp);
                
                if (targetSpeed != fanSpeed)
                {
                    fanSpeed = targetSpeed;
                    asusControl.SetFanSpeeds(targetSpeed);
                    trackBarFanSpeed.Value = targetSpeed;
                    labelValue.Text = targetSpeed.ToString();
                }
            }
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
            if (fanCurveMode && fanCurve.IsEnabled && !toolStripMenuItemAutoRefreshStats.Checked)
            {
                toolStripMenuItemAutoRefreshStats.Checked = true;
                MessageBox.Show("Auto refresh stats cannot be disabled while fan curve is active.", 
                    "Fan Curve Active", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
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
            if (fanCurveMode && fanCurve.IsEnabled)
                return;

            var value = trackBarFanSpeed.Value;
            Properties.Settings.Default.fanSpeed = value;
            Properties.Settings.Default.Save();

            if (!checkBoxTurnOn.Checked)
                value = 0;

            if (value == 0)
                labelValue.Text = "turned off";
            else
                labelValue.Text = value.ToString();

            if (fanSpeed == value)
                return;

            fanSpeed = value;

            asusControl.SetFanSpeeds(value);
        }

        private void checkBoxTurnOn_CheckedChanged(object sender, EventArgs e)
        {
            if (fanCurveMode && fanCurve.IsEnabled)
            {
                if (!checkBoxTurnOn.Checked)
                {
                    asusControl.SetFanSpeeds(0);
                    fanSpeed = 0;
                    labelValue.Text = "turned off";
                }
                return;
            }
            
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
            var rawTemp = asusControl.Thermal_Read_Cpu_Temperature();
            labelCPUTemp.Text = $"{rawTemp}";
            // Cache the last read temperature so the fan curve can use it
            // without making a redundant system-level call
            lastReadTemp = (int)rawTemp;
        }

        private void checkBoxFanCurve_CheckedChanged(object sender, EventArgs e)
        {
            fanCurveMode = checkBoxFanCurve.Checked;
            fanCurve.IsEnabled = fanCurveMode;
            
            Properties.Settings.Default.fanCurveEnabled = fanCurveMode;
            Properties.Settings.Default.Save();
            
            if (fanCurveMode)
            {
                checkBoxTurnOn.Checked = true;
                trackBarFanSpeed.Enabled = false;
                
                if (!toolStripMenuItemAutoRefreshStats.Checked)
                {
                    toolStripMenuItemAutoRefreshStats.Checked = true;
                    Properties.Settings.Default.autoRefreshStats = true;
                    Properties.Settings.Default.Save();
                    timerRefreshStats();
                }
                
                // Do an immediate read so lastReadTemp is fresh before the timer fires
                buttonRefreshCPUTemp_Click(sender, e);
                var targetSpeed = fanCurve.GetFanSpeedForTemperature(lastReadTemp);
                fanSpeed = targetSpeed;
                asusControl.SetFanSpeeds(targetSpeed);
                trackBarFanSpeed.Value = Math.Max(0, Math.Min(100, targetSpeed));
                labelValue.Text = targetSpeed.ToString();
            }
            else
            {
                trackBarFanSpeed.Enabled = true;
                setFanSpeed();
            }
        }

        private void buttonFanCurveSettings_Click(object sender, EventArgs e)
        {
            var settingsForm = new FanCurveSettingsForm(fanCurve);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                fanCurve = settingsForm.FanCurve;
                
                Properties.Settings.Default.fanCurveData = fanCurve.SerializeToString();
                Properties.Settings.Default.Save();
                
                if (fanCurveMode && fanCurve.IsEnabled && checkBoxTurnOn.Checked)
                {
                    // Use the already-cached UI temperature; no extra system call needed
                    var targetSpeed = fanCurve.GetFanSpeedForTemperature(lastReadTemp);
                    fanSpeed = targetSpeed;
                    asusControl.SetFanSpeeds(targetSpeed);
                    trackBarFanSpeed.Value = Math.Max(0, Math.Min(100, targetSpeed));
                    labelValue.Text = targetSpeed.ToString();
                }
            }
        }

        private void toolStripMenuItemRefreshInterval_Click(object sender, EventArgs e)
        {
            using (var dlg = new RefreshIntervalDialog(Properties.Settings.Default.refreshIntervalMs))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    Properties.Settings.Default.refreshIntervalMs = dlg.IntervalMs;
                    Properties.Settings.Default.Save();
                    UpdateRefreshIntervalMenuLabel();
                    timerRefreshStats(); // restart timer with new interval
                }
            }
        }

        private void UpdateRefreshIntervalMenuLabel()
        {
            var seconds = Properties.Settings.Default.refreshIntervalMs / 1000.0;
            toolStripMenuItemRefreshInterval.Text = $"Refresh interval: {seconds:0.#}s";
        }

    }
}
