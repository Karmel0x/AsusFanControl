using AsusFanControlGUI.Controls;

namespace AsusFanControlGUI
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(Form1));

            // ── Declare all controls ──────────────────────────────────────────────
            this.menuStrip1                             = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1                     = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemTurnOffControlOnExit  = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemForbidUnsafeSettings  = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemMinimizeToTrayOnClose = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAutoRefreshStats      = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCheckForUpdates       = new System.Windows.Forms.ToolStripMenuItem();

            this.mainTabControl    = new System.Windows.Forms.TabControl();
            this.tabPageManual     = new System.Windows.Forms.TabPage();
            this.tabPageFanCurves  = new System.Windows.Forms.TabPage();

            // Manual tab controls
            this.trackBarFanSpeed      = new System.Windows.Forms.TrackBar();
            this.label1                = new System.Windows.Forms.Label();
            this.labelValue            = new System.Windows.Forms.Label();
            this.label2                = new System.Windows.Forms.Label();
            this.buttonRefreshRPM      = new System.Windows.Forms.Button();
            this.labelRPM              = new System.Windows.Forms.Label();
            this.checkBoxTurnOn        = new System.Windows.Forms.CheckBox();
            this.labelCPUTemp          = new System.Windows.Forms.Label();
            this.buttonRefreshCPUTemp  = new System.Windows.Forms.Button();
            this.label4                = new System.Windows.Forms.Label();

            // Fan Curves tab controls
            this.panelCurveToolbar   = new System.Windows.Forms.FlowLayoutPanel();
            this.labelProfile        = new System.Windows.Forms.Label();
            this.profileComboBox     = new System.Windows.Forms.ComboBox();
            this.btnSaveProfile      = new System.Windows.Forms.Button();
            this.btnNewProfile       = new System.Windows.Forms.Button();
            this.btnDeleteProfile    = new System.Windows.Forms.Button();
            this.checkBoxClampToGrid = new System.Windows.Forms.CheckBox();

            this.fanCurvePanelCpu    = new FanCurvePanel();
            this.fanCurvePanelGpu    = new FanCurvePanel();

            this.panelCurveBottom    = new System.Windows.Forms.Panel();
            this.btnStartCurve       = new System.Windows.Forms.Button();
            this.btnStopCurve        = new System.Windows.Forms.Button();
            this.lblCurveStatus      = new System.Windows.Forms.Label();
            this.lblLiveCpuTemp      = new System.Windows.Forms.Label();
            this.lblLiveRpm          = new System.Windows.Forms.Label();

            // ── Suspend layouts ───────────────────────────────────────────────────
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFanSpeed)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.mainTabControl.SuspendLayout();
            this.tabPageManual.SuspendLayout();
            this.tabPageFanCurves.SuspendLayout();
            this.panelCurveToolbar.SuspendLayout();
            this.panelCurveBottom.SuspendLayout();
            this.SuspendLayout();

            // ════════════════════════════════════════════════════════════════════
            //  menuStrip1
            // ════════════════════════════════════════════════════════════════════
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.toolStripMenuItem1,
                this.toolStripMenuItemCheckForUpdates
            });
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(760, 24);
            this.menuStrip1.TabIndex = 0;

            // toolStripMenuItem1 (Advanced)
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.toolStripMenuItemTurnOffControlOnExit,
                this.toolStripMenuItemForbidUnsafeSettings,
                this.toolStripMenuItemMinimizeToTrayOnClose,
                this.toolStripMenuItemAutoRefreshStats
            });
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Text = "Advanced";

            this.toolStripMenuItemTurnOffControlOnExit.CheckOnClick = true;
            this.toolStripMenuItemTurnOffControlOnExit.Name = "toolStripMenuItemTurnOffControlOnExit";
            this.toolStripMenuItemTurnOffControlOnExit.Text = "Turn off control on exit";
            this.toolStripMenuItemTurnOffControlOnExit.CheckedChanged +=
                new System.EventHandler(this.toolStripMenuItemTurnOffControlOnExit_CheckedChanged);

            this.toolStripMenuItemForbidUnsafeSettings.CheckOnClick = true;
            this.toolStripMenuItemForbidUnsafeSettings.Name = "toolStripMenuItemForbidUnsafeSettings";
            this.toolStripMenuItemForbidUnsafeSettings.Text = "Forbid unsafe settings";
            this.toolStripMenuItemForbidUnsafeSettings.CheckedChanged +=
                new System.EventHandler(this.toolStripMenuItemForbidUnsafeSettings_CheckedChanged);

            this.toolStripMenuItemMinimizeToTrayOnClose.CheckOnClick = true;
            this.toolStripMenuItemMinimizeToTrayOnClose.Name = "toolStripMenuItemMinimizeToTrayOnClose";
            this.toolStripMenuItemMinimizeToTrayOnClose.Text = "Minimize to tray on close";
            this.toolStripMenuItemMinimizeToTrayOnClose.Click +=
                new System.EventHandler(this.toolStripMenuItemMinimizeToTrayOnClose_Click);

            this.toolStripMenuItemAutoRefreshStats.CheckOnClick = true;
            this.toolStripMenuItemAutoRefreshStats.Name = "toolStripMenuItemAutoRefreshStats";
            this.toolStripMenuItemAutoRefreshStats.Text = "Auto refresh stats";
            this.toolStripMenuItemAutoRefreshStats.Click +=
                new System.EventHandler(this.toolStripMenuItemAutoRefreshStats_Click);

            this.toolStripMenuItemCheckForUpdates.Name = "toolStripMenuItemCheckForUpdates";
            this.toolStripMenuItemCheckForUpdates.Text = "Check for updates";
            this.toolStripMenuItemCheckForUpdates.Click +=
                new System.EventHandler(this.toolStripMenuItemCheckForUpdates_Click);

            // ════════════════════════════════════════════════════════════════════
            //  mainTabControl
            // ════════════════════════════════════════════════════════════════════
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.TabPages.AddRange(new System.Windows.Forms.TabPage[]
            {
                this.tabPageManual,
                this.tabPageFanCurves
            });
            this.mainTabControl.SelectedIndexChanged +=
                new System.EventHandler(this.mainTabControl_SelectedIndexChanged);

            // ════════════════════════════════════════════════════════════════════
            //  tabPageManual  — existing controls repositioned inside tab
            // ════════════════════════════════════════════════════════════════════
            this.tabPageManual.Name = "tabPageManual";
            this.tabPageManual.Text = "Manual Control";
            this.tabPageManual.UseVisualStyleBackColor = false;
            this.tabPageManual.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.checkBoxTurnOn,
                this.trackBarFanSpeed,
                this.label1,
                this.labelValue,
                this.label2,
                this.buttonRefreshRPM,
                this.labelRPM,
                this.buttonRefreshCPUTemp,
                this.label4,
                this.labelCPUTemp
            });

            // Manual tab controls
            this.checkBoxTurnOn.AutoSize = true;
            this.checkBoxTurnOn.Location = new System.Drawing.Point(12, 14);
            this.checkBoxTurnOn.Name = "checkBoxTurnOn";
            this.checkBoxTurnOn.Text = "Turn on fan control";
            this.checkBoxTurnOn.CheckedChanged +=
                new System.EventHandler(this.checkBoxTurnOn_CheckedChanged);

            this.trackBarFanSpeed.Location = new System.Drawing.Point(12, 38);
            this.trackBarFanSpeed.Maximum = 100;
            this.trackBarFanSpeed.Name = "trackBarFanSpeed";
            this.trackBarFanSpeed.Size = new System.Drawing.Size(300, 45);
            this.trackBarFanSpeed.Value = 100;
            this.trackBarFanSpeed.KeyUp +=
                new System.Windows.Forms.KeyEventHandler(this.trackBarFanSpeed_KeyUp);
            this.trackBarFanSpeed.MouseCaptureChanged +=
                new System.EventHandler(this.trackBarFanSpeed_MouseCaptureChanged);

            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 88);
            this.label1.Text = "Current value:";

            this.labelValue.AutoSize = true;
            this.labelValue.Location = new System.Drawing.Point(95, 88);
            this.labelValue.Text = "-";

            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 116);
            this.label2.Text = "Current RPM:";

            this.buttonRefreshRPM.Location = new System.Drawing.Point(12, 111);
            this.buttonRefreshRPM.Name = "buttonRefreshRPM";
            this.buttonRefreshRPM.Size = new System.Drawing.Size(22, 23);
            this.buttonRefreshRPM.Text = "↻";
            this.buttonRefreshRPM.UseVisualStyleBackColor = true;
            this.buttonRefreshRPM.Click +=
                new System.EventHandler(this.buttonRefreshRPM_Click);

            this.labelRPM.AutoSize = true;
            this.labelRPM.Location = new System.Drawing.Point(120, 116);
            this.labelRPM.Text = "-";

            this.buttonRefreshCPUTemp.Location = new System.Drawing.Point(12, 140);
            this.buttonRefreshCPUTemp.Name = "buttonRefreshCPUTemp";
            this.buttonRefreshCPUTemp.Size = new System.Drawing.Size(22, 23);
            this.buttonRefreshCPUTemp.Text = "↻";
            this.buttonRefreshCPUTemp.UseVisualStyleBackColor = true;
            this.buttonRefreshCPUTemp.Click +=
                new System.EventHandler(this.buttonRefreshCPUTemp_Click);

            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(40, 145);
            this.label4.Text = "Current CPU temp:";

            this.labelCPUTemp.AutoSize = true;
            this.labelCPUTemp.Location = new System.Drawing.Point(148, 145);
            this.labelCPUTemp.Text = "-";

            // ════════════════════════════════════════════════════════════════════
            //  tabPageFanCurves
            // ════════════════════════════════════════════════════════════════════
            this.tabPageFanCurves.Name = "tabPageFanCurves";
            this.tabPageFanCurves.Text = "Fan Curves";
            this.tabPageFanCurves.UseVisualStyleBackColor = false;
            this.tabPageFanCurves.Padding = new System.Windows.Forms.Padding(4);
            this.tabPageFanCurves.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.panelCurveToolbar,
                this.fanCurvePanelCpu,
                this.fanCurvePanelGpu,
                this.panelCurveBottom
            });

            // ── Toolbar (FlowLayoutPanel) ─────────────────────────────────────────
            this.panelCurveToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelCurveToolbar.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.panelCurveToolbar.WrapContents = false;
            this.panelCurveToolbar.Height = 38;
            this.panelCurveToolbar.Padding = new System.Windows.Forms.Padding(4, 4, 4, 0);
            this.panelCurveToolbar.Name = "panelCurveToolbar";
            this.panelCurveToolbar.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.labelProfile,
                this.profileComboBox,
                this.btnSaveProfile,
                this.btnNewProfile,
                this.btnDeleteProfile,
                this.checkBoxClampToGrid
            });

            this.labelProfile.Text = "Profile:";
            this.labelProfile.AutoSize = true;
            this.labelProfile.Margin = new System.Windows.Forms.Padding(0, 7, 4, 0);

            this.profileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.profileComboBox.DisplayMember = "Name";
            this.profileComboBox.Width = 160;
            this.profileComboBox.Margin = new System.Windows.Forms.Padding(0, 4, 6, 0);
            this.profileComboBox.Name = "profileComboBox";
            this.profileComboBox.SelectedIndexChanged +=
                new System.EventHandler(this.profileComboBox_SelectedIndexChanged);

            this.btnSaveProfile.Text = "💾 Save";
            this.btnSaveProfile.Width = 68;
            this.btnSaveProfile.Height = 26;
            this.btnSaveProfile.Margin = new System.Windows.Forms.Padding(0, 4, 4, 0);
            this.btnSaveProfile.Name = "btnSaveProfile";
            this.btnSaveProfile.Enabled = false;
            this.btnSaveProfile.Click += new System.EventHandler(this.btnSaveProfile_Click);

            this.btnNewProfile.Text = "＋ New";
            this.btnNewProfile.Width = 64;
            this.btnNewProfile.Height = 26;
            this.btnNewProfile.Margin = new System.Windows.Forms.Padding(0, 4, 4, 0);
            this.btnNewProfile.Name = "btnNewProfile";
            this.btnNewProfile.Click += new System.EventHandler(this.btnNewProfile_Click);

            this.btnDeleteProfile.Text = "🗑 Delete";
            this.btnDeleteProfile.Width = 74;
            this.btnDeleteProfile.Height = 26;
            this.btnDeleteProfile.Margin = new System.Windows.Forms.Padding(0, 4, 8, 0);
            this.btnDeleteProfile.Name = "btnDeleteProfile";
            this.btnDeleteProfile.Enabled = false;
            this.btnDeleteProfile.Click += new System.EventHandler(this.btnDeleteProfile_Click);

            this.checkBoxClampToGrid.Text = "Clamp to Grid (5°C)";
            this.checkBoxClampToGrid.AutoSize = true;
            this.checkBoxClampToGrid.Margin = new System.Windows.Forms.Padding(0, 7, 0, 0);
            this.checkBoxClampToGrid.Name = "checkBoxClampToGrid";
            this.checkBoxClampToGrid.CheckedChanged +=
                new System.EventHandler(this.checkBoxClampToGrid_CheckedChanged);

            // ── CPU Fan Curve Panel ───────────────────────────────────────────────
            this.fanCurvePanelCpu.Dock = System.Windows.Forms.DockStyle.Top;
            this.fanCurvePanelCpu.Height = 210;
            this.fanCurvePanelCpu.Name = "fanCurvePanelCpu";
            this.fanCurvePanelCpu.PanelTitle = "CPU Fan Profile, RPM/°C";
            this.fanCurvePanelCpu.CurveColor = System.Drawing.Color.FromArgb(79, 195, 247);
            this.fanCurvePanelCpu.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);

            // ── GPU Fan Curve Panel ───────────────────────────────────────────────
            this.fanCurvePanelGpu.Dock = System.Windows.Forms.DockStyle.Top;
            this.fanCurvePanelGpu.Height = 210;
            this.fanCurvePanelGpu.Name = "fanCurvePanelGpu";
            this.fanCurvePanelGpu.PanelTitle = "GPU Fan Profile, RPM/°C";
            this.fanCurvePanelGpu.CurveColor = System.Drawing.Color.FromArgb(239, 83, 80);
            this.fanCurvePanelGpu.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);

            // ── Bottom control bar ────────────────────────────────────────────────
            this.panelCurveBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelCurveBottom.Height = 38;
            this.panelCurveBottom.Name = "panelCurveBottom";
            this.panelCurveBottom.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.btnStartCurve,
                this.btnStopCurve,
                this.lblCurveStatus,
                this.lblLiveCpuTemp,
                this.lblLiveRpm
            });

            this.btnStartCurve.Text = "▶ Start Curve Control";
            this.btnStartCurve.Location = new System.Drawing.Point(6, 6);
            this.btnStartCurve.Size = new System.Drawing.Size(148, 26);
            this.btnStartCurve.Name = "btnStartCurve";
            this.btnStartCurve.Click += new System.EventHandler(this.btnStartCurve_Click);

            this.btnStopCurve.Text = "■ Stop";
            this.btnStopCurve.Location = new System.Drawing.Point(160, 6);
            this.btnStopCurve.Size = new System.Drawing.Size(72, 26);
            this.btnStopCurve.Name = "btnStopCurve";
            this.btnStopCurve.Enabled = false;
            this.btnStopCurve.Click += new System.EventHandler(this.btnStopCurve_Click);

            this.lblCurveStatus.AutoSize = false;
            this.lblCurveStatus.Location = new System.Drawing.Point(240, 10);
            this.lblCurveStatus.Size = new System.Drawing.Size(260, 18);
            this.lblCurveStatus.Name = "lblCurveStatus";
            this.lblCurveStatus.Text = "Status: Idle";
            this.lblCurveStatus.ForeColor = System.Drawing.Color.FromArgb(140, 140, 180);

            this.lblLiveCpuTemp.AutoSize = true;
            this.lblLiveCpuTemp.Location = new System.Drawing.Point(508, 10);
            this.lblLiveCpuTemp.Name = "lblLiveCpuTemp";
            this.lblLiveCpuTemp.Text = "CPU: --°C";
            this.lblLiveCpuTemp.ForeColor = System.Drawing.Color.FromArgb(79, 195, 247);

            this.lblLiveRpm.AutoSize = true;
            this.lblLiveRpm.Location = new System.Drawing.Point(594, 10);
            this.lblLiveRpm.Name = "lblLiveRpm";
            this.lblLiveRpm.Text = "Fan: --%";
            this.lblLiveRpm.ForeColor = System.Drawing.Color.FromArgb(239, 83, 80);

            // ════════════════════════════════════════════════════════════════════
            //  Form1
            // ════════════════════════════════════════════════════════════════════
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 540);
            this.MinimumSize = new System.Drawing.Size(760, 540);
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Asus Fan Control";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);

            // ── Resume layouts ────────────────────────────────────────────────────
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFanSpeed)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelCurveToolbar.ResumeLayout(false);
            this.panelCurveToolbar.PerformLayout();
            this.panelCurveBottom.ResumeLayout(false);
            this.panelCurveBottom.PerformLayout();
            this.tabPageManual.ResumeLayout(false);
            this.tabPageManual.PerformLayout();
            this.tabPageFanCurves.ResumeLayout(false);
            this.mainTabControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        // ── Field declarations ────────────────────────────────────────────────────
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemTurnOffControlOnExit;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemForbidUnsafeSettings;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMinimizeToTrayOnClose;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAutoRefreshStats;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCheckForUpdates;

        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage tabPageManual;
        private System.Windows.Forms.TabPage tabPageFanCurves;

        // Manual tab
        private System.Windows.Forms.TrackBar trackBarFanSpeed;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelValue;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonRefreshRPM;
        private System.Windows.Forms.Label labelRPM;
        private System.Windows.Forms.CheckBox checkBoxTurnOn;
        private System.Windows.Forms.Label labelCPUTemp;
        private System.Windows.Forms.Button buttonRefreshCPUTemp;
        private System.Windows.Forms.Label label4;

        // Fan Curves tab
        private System.Windows.Forms.FlowLayoutPanel panelCurveToolbar;
        private System.Windows.Forms.Label labelProfile;
        private System.Windows.Forms.ComboBox profileComboBox;
        private System.Windows.Forms.Button btnSaveProfile;
        private System.Windows.Forms.Button btnNewProfile;
        private System.Windows.Forms.Button btnDeleteProfile;
        private System.Windows.Forms.CheckBox checkBoxClampToGrid;
        private FanCurvePanel fanCurvePanelCpu;
        private FanCurvePanel fanCurvePanelGpu;
        private System.Windows.Forms.Panel panelCurveBottom;
        private System.Windows.Forms.Button btnStartCurve;
        private System.Windows.Forms.Button btnStopCurve;
        private System.Windows.Forms.Label lblCurveStatus;
        private System.Windows.Forms.Label lblLiveCpuTemp;
        private System.Windows.Forms.Label lblLiveRpm;
    }
}
