using Microsoft.Win32;
using AsusFanControl;
using System;
using System.Diagnostics;
using System.IO;
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
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false))
                {
                    toolStripMenuItemRunAtStartup.Checked = key?.GetValue("AsusFanControl") != null;
                }
            }
            catch
            {
                toolStripMenuItemRunAtStartup.Checked = Properties.Settings.Default.runAtStartup;
            }
            trackBarFanSpeed.Value = Properties.Settings.Default.fanSpeed;
            
            fanCurve = FanCurve.DeserializeFromString(Properties.Settings.Default.fanCurveData);
            fanCurveMode = Properties.Settings.Default.fanCurveEnabled;
            fanCurve.IsEnabled = fanCurveMode;
            checkBoxFanCurve.Checked = fanCurveMode;
            
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
            buttonRefreshRPM_Click(sender, e);
            buttonRefreshCPUTemp_Click(sender, e);
            
            if (fanCurveMode && fanCurve.IsEnabled && checkBoxTurnOn.Checked)
            {
                int rawTargetSpeed = fanCurve.GetFanSpeedForTemperature(lastReadTemp);
                int targetSpeed = ApplySafetyLimits(rawTargetSpeed);
                
                if (targetSpeed != fanSpeed)
                {
                    fanSpeed = targetSpeed;
                    asusControl.SetFanSpeeds(targetSpeed);
                    trackBarFanSpeed.Value = Math.Max(0, Math.Min(100, targetSpeed));
                    labelValue.Text = targetSpeed.ToString();
                }
            }
        }

        private int ApplySafetyLimits(int speed)
        {
            if (speed <= 0)
                return 0;

            if (Properties.Settings.Default.forbidUnsafeSettings)
            {
                return Math.Max(40, Math.Min(99, speed));
            }

            return Math.Max(0, Math.Min(100, speed));
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

            if (toolStripMenuItemForbidUnsafeSettings.Checked)
            {
                if (trackBarFanSpeed.Value < 40)
                    trackBarFanSpeed.Value = 40;
                else if (trackBarFanSpeed.Value > 99)
                    trackBarFanSpeed.Value = 99;

                if (checkBoxTurnOn.Checked)
                    setFanSpeed();
            }
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
            try
            {
                Process.Start(new ProcessStartInfo("https://github.com/Karmel0x/AsusFanControl/releases") { UseShellExecute = true });
            }
            catch { }
        }

        private void toolStripMenuItemRunAsSystem_Click(object sender, EventArgs e)
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string batPath = Path.Combine(baseDir, "run.bat");

                if (!File.Exists(batPath))
                {
                    MessageBox.Show("run.bat was not found in the application directory.", "File Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var psi = new ProcessStartInfo
                {
                    FileName = batPath,
                    UseShellExecute = true,
                    Verb = "runas"
                };

                Process.Start(psi);
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to launch as SYSTEM: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void setFanSpeed()
        {
            if (fanCurveMode && fanCurve.IsEnabled)
                return;

            var rawValue = trackBarFanSpeed.Value;
            var value = ApplySafetyLimits(rawValue);

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
                else
                {
                    buttonRefreshCPUTemp_Click(sender, e);
                    int rawTarget = fanCurve.GetFanSpeedForTemperature(lastReadTemp);
                    int target = ApplySafetyLimits(rawTarget);
                    fanSpeed = target;
                    asusControl.SetFanSpeeds(target);
                    trackBarFanSpeed.Value = Math.Max(0, Math.Min(100, target));
                    labelValue.Text = target.ToString();
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
                
                buttonRefreshCPUTemp_Click(sender, e);
                int rawTarget = fanCurve.GetFanSpeedForTemperature(lastReadTemp);
                int targetSpeed = ApplySafetyLimits(rawTarget);
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
            // Pass lastReadTemp so the current CPU temperature line is shown on the graph!
            var settingsForm = new FanCurveSettingsForm(fanCurve, lastReadTemp);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                fanCurve = settingsForm.FanCurve;
                
                Properties.Settings.Default.fanCurveData = fanCurve.SerializeToString();
                Properties.Settings.Default.Save();
                
                if (fanCurveMode && fanCurve.IsEnabled && checkBoxTurnOn.Checked)
                {
                    int rawTarget = fanCurve.GetFanSpeedForTemperature(lastReadTemp);
                    int targetSpeed = ApplySafetyLimits(rawTarget);
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
                    timerRefreshStats();
                }
            }
        }

        private void UpdateRefreshIntervalMenuLabel()
        {
            var seconds = Properties.Settings.Default.refreshIntervalMs / 1000.0;
            toolStripMenuItemRefreshInterval.Text = $"Refresh interval: {seconds:0.#}s";
        }

        private void SetStartupRegistry(bool enable)
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string batPath = Path.Combine(baseDir, "run.bat");
                string targetPath = File.Exists(batPath) ? batPath : Application.ExecutablePath;
                const string runKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
                const string appName = "AsusFanControl";

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(runKeyPath, true))
                {
                    if (key != null)
                    {
                        if (enable)
                        {
                            key.SetValue(appName, $"\"{targetPath}\"");
                        }
                        else
                        {
                            key.DeleteValue(appName, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update startup registry key: " + ex.Message,
                    "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripMenuItemRunAtStartup_CheckedChanged(object sender, EventArgs e)
        {
            bool enable = toolStripMenuItemRunAtStartup.Checked;
            Properties.Settings.Default.runAtStartup = enable;
            Properties.Settings.Default.Save();
            SetStartupRegistry(enable);
        }
    }
}
