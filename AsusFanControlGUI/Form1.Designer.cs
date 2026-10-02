namespace AsusFanControlGUI
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.trackBarFanSpeed = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.labelValue = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonRefreshRPM = new System.Windows.Forms.Button();
            this.labelRPM = new System.Windows.Forms.Label();
            this.checkBoxTurnOn = new System.Windows.Forms.CheckBox();
            this.labelCPUTemp = new System.Windows.Forms.Label();
            this.buttonRefreshCPUTemp = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.labelGPUTemp = new System.Windows.Forms.Label();
            this.buttonRefreshGPUTemp = new System.Windows.Forms.Button();
            this.labelGpuCaption = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemTurnOffControlOnExit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemForbidUnsafeSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemMinimizeToTrayOnClose = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAutoRefreshStats = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemStartWithWindows = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemFansOffOnSleep = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCheckForUpdates = new System.Windows.Forms.ToolStripMenuItem();
            this.radioManual = new System.Windows.Forms.RadioButton();
            this.radioFanCurve = new System.Windows.Forms.RadioButton();
            this.panelManual = new System.Windows.Forms.Panel();
            this.panelFanCurve = new System.Windows.Forms.Panel();
            this.labelSetpoints = new System.Windows.Forms.Label();
            this.comboBoxSetpointCount = new System.Windows.Forms.ComboBox();
            this.checkBoxUseGpu = new System.Windows.Forms.CheckBox();
            this.labelEditCurve = new System.Windows.Forms.Label();
            this.comboBoxEditCurve = new System.Windows.Forms.ComboBox();
            this.labelHeaderTemp = new System.Windows.Forms.Label();
            this.labelHeaderFan = new System.Windows.Forms.Label();
            this.labelPoint1 = new System.Windows.Forms.Label();
            this.labelPoint2 = new System.Windows.Forms.Label();
            this.labelPoint3 = new System.Windows.Forms.Label();
            this.numericTemp1 = new System.Windows.Forms.NumericUpDown();
            this.numericTemp2 = new System.Windows.Forms.NumericUpDown();
            this.numericTemp3 = new System.Windows.Forms.NumericUpDown();
            this.numericFan1 = new System.Windows.Forms.NumericUpDown();
            this.numericFan2 = new System.Windows.Forms.NumericUpDown();
            this.numericFan3 = new System.Windows.Forms.NumericUpDown();
            this.labelCurveHint = new System.Windows.Forms.Label();
            this.labelCurveStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFanSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTemp1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTemp2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTemp3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericFan1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericFan2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericFan3)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.panelManual.SuspendLayout();
            this.panelFanCurve.SuspendLayout();
            this.SuspendLayout();
            // 
            // trackBarFanSpeed
            // 
            this.trackBarFanSpeed.Location = new System.Drawing.Point(3, 8);
            this.trackBarFanSpeed.Maximum = 100;
            this.trackBarFanSpeed.Name = "trackBarFanSpeed";
            this.trackBarFanSpeed.Size = new System.Drawing.Size(300, 45);
            this.trackBarFanSpeed.TabIndex = 0;
            this.trackBarFanSpeed.Value = 100;
            this.trackBarFanSpeed.KeyUp += new System.Windows.Forms.KeyEventHandler(this.trackBarFanSpeed_KeyUp);
            this.trackBarFanSpeed.MouseCaptureChanged += new System.EventHandler(this.trackBarFanSpeed_MouseCaptureChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Current value:";
            // 
            // labelValue
            // 
            this.labelValue.AutoSize = true;
            this.labelValue.Location = new System.Drawing.Point(82, 56);
            this.labelValue.Name = "labelValue";
            this.labelValue.Size = new System.Drawing.Size(10, 13);
            this.labelValue.TabIndex = 2;
            this.labelValue.Text = "-";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 280);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Current RPM:";
            // 
            // buttonRefreshRPM
            // 
            this.buttonRefreshRPM.Location = new System.Drawing.Point(12, 275);
            this.buttonRefreshRPM.Name = "buttonRefreshRPM";
            this.buttonRefreshRPM.Size = new System.Drawing.Size(22, 23);
            this.buttonRefreshRPM.TabIndex = 4;
            this.buttonRefreshRPM.Text = "↻";
            this.buttonRefreshRPM.UseVisualStyleBackColor = true;
            this.buttonRefreshRPM.Click += new System.EventHandler(this.buttonRefreshRPM_Click);
            // 
            // labelRPM
            // 
            this.labelRPM.AutoSize = true;
            this.labelRPM.Location = new System.Drawing.Point(117, 280);
            this.labelRPM.Name = "labelRPM";
            this.labelRPM.Size = new System.Drawing.Size(10, 13);
            this.labelRPM.TabIndex = 5;
            this.labelRPM.Text = "-";
            // 
            // checkBoxTurnOn
            // 
            this.checkBoxTurnOn.AutoSize = true;
            this.checkBoxTurnOn.Location = new System.Drawing.Point(12, 32);
            this.checkBoxTurnOn.Name = "checkBoxTurnOn";
            this.checkBoxTurnOn.Size = new System.Drawing.Size(116, 17);
            this.checkBoxTurnOn.TabIndex = 6;
            this.checkBoxTurnOn.Text = "Turn on fan control";
            this.checkBoxTurnOn.UseVisualStyleBackColor = true;
            this.checkBoxTurnOn.CheckedChanged += new System.EventHandler(this.checkBoxTurnOn_CheckedChanged);
            // 
            // labelCPUTemp
            // 
            this.labelCPUTemp.AutoSize = true;
            this.labelCPUTemp.Location = new System.Drawing.Point(145, 308);
            this.labelCPUTemp.Name = "labelCPUTemp";
            this.labelCPUTemp.Size = new System.Drawing.Size(10, 13);
            this.labelCPUTemp.TabIndex = 9;
            this.labelCPUTemp.Text = "-";
            // 
            // buttonRefreshCPUTemp
            // 
            this.buttonRefreshCPUTemp.Location = new System.Drawing.Point(12, 303);
            this.buttonRefreshCPUTemp.Name = "buttonRefreshCPUTemp";
            this.buttonRefreshCPUTemp.Size = new System.Drawing.Size(22, 23);
            this.buttonRefreshCPUTemp.TabIndex = 8;
            this.buttonRefreshCPUTemp.Text = "↻";
            this.buttonRefreshCPUTemp.UseVisualStyleBackColor = true;
            this.buttonRefreshCPUTemp.Click += new System.EventHandler(this.buttonRefreshCPUTemp_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(40, 308);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Current CPU temp:";
            // 
            // labelGPUTemp
            // 
            this.labelGPUTemp.AutoSize = true;
            this.labelGPUTemp.Location = new System.Drawing.Point(145, 336);
            this.labelGPUTemp.Name = "labelGPUTemp";
            this.labelGPUTemp.Size = new System.Drawing.Size(10, 13);
            this.labelGPUTemp.TabIndex = 12;
            this.labelGPUTemp.Text = "-";
            // 
            // buttonRefreshGPUTemp
            // 
            this.buttonRefreshGPUTemp.Location = new System.Drawing.Point(12, 331);
            this.buttonRefreshGPUTemp.Name = "buttonRefreshGPUTemp";
            this.buttonRefreshGPUTemp.Size = new System.Drawing.Size(22, 23);
            this.buttonRefreshGPUTemp.TabIndex = 11;
            this.buttonRefreshGPUTemp.Text = "↻";
            this.buttonRefreshGPUTemp.UseVisualStyleBackColor = true;
            this.buttonRefreshGPUTemp.Click += new System.EventHandler(this.buttonRefreshGPUTemp_Click);
            // 
            // labelGpuCaption
            // 
            this.labelGpuCaption.AutoSize = true;
            this.labelGpuCaption.Location = new System.Drawing.Point(40, 336);
            this.labelGpuCaption.Name = "labelGpuCaption";
            this.labelGpuCaption.Size = new System.Drawing.Size(96, 13);
            this.labelGpuCaption.TabIndex = 10;
            this.labelGpuCaption.Text = "Current GPU temp:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItemCheckForUpdates});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(392, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemTurnOffControlOnExit,
            this.toolStripMenuItemForbidUnsafeSettings,
            this.toolStripMenuItemMinimizeToTrayOnClose,
            this.toolStripMenuItemAutoRefreshStats,
            this.toolStripMenuItemStartWithWindows,
            this.toolStripMenuItemFansOffOnSleep});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(72, 20);
            this.toolStripMenuItem1.Text = "Advanced";
            // 
            // toolStripMenuItemTurnOffControlOnExit
            // 
            this.toolStripMenuItemTurnOffControlOnExit.CheckOnClick = true;
            this.toolStripMenuItemTurnOffControlOnExit.Name = "toolStripMenuItemTurnOffControlOnExit";
            this.toolStripMenuItemTurnOffControlOnExit.Size = new System.Drawing.Size(207, 22);
            this.toolStripMenuItemTurnOffControlOnExit.Text = "Turn off control on exit";
            this.toolStripMenuItemTurnOffControlOnExit.CheckedChanged += new System.EventHandler(this.toolStripMenuItemTurnOffControlOnExit_CheckedChanged);
            // 
            // toolStripMenuItemForbidUnsafeSettings
            // 
            this.toolStripMenuItemForbidUnsafeSettings.CheckOnClick = true;
            this.toolStripMenuItemForbidUnsafeSettings.Name = "toolStripMenuItemForbidUnsafeSettings";
            this.toolStripMenuItemForbidUnsafeSettings.Size = new System.Drawing.Size(207, 22);
            this.toolStripMenuItemForbidUnsafeSettings.Text = "Forbid unsafe settings";
            this.toolStripMenuItemForbidUnsafeSettings.CheckedChanged += new System.EventHandler(this.toolStripMenuItemForbidUnsafeSettings_CheckedChanged);
            // 
            // toolStripMenuItemMinimizeToTrayOnClose
            // 
            this.toolStripMenuItemMinimizeToTrayOnClose.CheckOnClick = true;
            this.toolStripMenuItemMinimizeToTrayOnClose.Name = "toolStripMenuItemMinimizeToTrayOnClose";
            this.toolStripMenuItemMinimizeToTrayOnClose.Size = new System.Drawing.Size(207, 22);
            this.toolStripMenuItemMinimizeToTrayOnClose.Text = "Minimize to tray on close";
            this.toolStripMenuItemMinimizeToTrayOnClose.Click += new System.EventHandler(this.toolStripMenuItemMinimizeToTrayOnClose_Click);
            // 
            // toolStripMenuItemAutoRefreshStats
            // 
            this.toolStripMenuItemAutoRefreshStats.CheckOnClick = true;
            this.toolStripMenuItemAutoRefreshStats.Name = "toolStripMenuItemAutoRefreshStats";
            this.toolStripMenuItemAutoRefreshStats.Size = new System.Drawing.Size(207, 22);
            this.toolStripMenuItemAutoRefreshStats.Text = "Auto refresh stats";
            this.toolStripMenuItemAutoRefreshStats.Click += new System.EventHandler(this.toolStripMenuItemAutoRefreshStats_Click);
            // 
            // toolStripMenuItemStartWithWindows
            // 
            this.toolStripMenuItemStartWithWindows.CheckOnClick = true;
            this.toolStripMenuItemStartWithWindows.Name = "toolStripMenuItemStartWithWindows";
            this.toolStripMenuItemStartWithWindows.Size = new System.Drawing.Size(207, 22);
            this.toolStripMenuItemStartWithWindows.Text = "Start with Windows";
            this.toolStripMenuItemStartWithWindows.CheckedChanged += new System.EventHandler(this.toolStripMenuItemStartWithWindows_CheckedChanged);
            // 
            // toolStripMenuItemFansOffOnSleep
            // 
            this.toolStripMenuItemFansOffOnSleep.CheckOnClick = true;
            this.toolStripMenuItemFansOffOnSleep.Name = "toolStripMenuItemFansOffOnSleep";
            this.toolStripMenuItemFansOffOnSleep.Size = new System.Drawing.Size(207, 22);
            this.toolStripMenuItemFansOffOnSleep.Text = "Turn fans off on sleep";
            this.toolStripMenuItemFansOffOnSleep.CheckedChanged += new System.EventHandler(this.toolStripMenuItemFansOffOnSleep_CheckedChanged);
            // 
            // toolStripMenuItemCheckForUpdates
            // 
            this.toolStripMenuItemCheckForUpdates.Name = "toolStripMenuItemCheckForUpdates";
            this.toolStripMenuItemCheckForUpdates.Size = new System.Drawing.Size(115, 20);
            this.toolStripMenuItemCheckForUpdates.Text = "Check for updates";
            this.toolStripMenuItemCheckForUpdates.Click += new System.EventHandler(this.toolStripMenuItemCheckForUpdates_Click);
            // 
            // radioManual
            // 
            this.radioManual.AutoSize = true;
            this.radioManual.Location = new System.Drawing.Point(12, 54);
            this.radioManual.Name = "radioManual";
            this.radioManual.Size = new System.Drawing.Size(60, 17);
            this.radioManual.TabIndex = 11;
            this.radioManual.TabStop = true;
            this.radioManual.Text = "Manual";
            this.radioManual.UseVisualStyleBackColor = true;
            this.radioManual.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // radioFanCurve
            // 
            this.radioFanCurve.AutoSize = true;
            this.radioFanCurve.Location = new System.Drawing.Point(90, 54);
            this.radioFanCurve.Name = "radioFanCurve";
            this.radioFanCurve.Size = new System.Drawing.Size(43, 17);
            this.radioFanCurve.TabIndex = 12;
            this.radioFanCurve.TabStop = true;
            this.radioFanCurve.Text = "Auto";
            this.radioFanCurve.UseVisualStyleBackColor = true;
            this.radioFanCurve.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // panelManual
            // 
            this.panelManual.Controls.Add(this.trackBarFanSpeed);
            this.panelManual.Controls.Add(this.label1);
            this.panelManual.Controls.Add(this.labelValue);
            this.panelManual.Location = new System.Drawing.Point(12, 76);
            this.panelManual.Name = "panelManual";
            this.panelManual.Size = new System.Drawing.Size(368, 190);
            this.panelManual.TabIndex = 13;
            // 
            // panelFanCurve
            // 
            this.panelFanCurve.Controls.Add(this.labelSetpoints);
            this.panelFanCurve.Controls.Add(this.comboBoxSetpointCount);
            this.panelFanCurve.Controls.Add(this.checkBoxUseGpu);
            this.panelFanCurve.Controls.Add(this.labelEditCurve);
            this.panelFanCurve.Controls.Add(this.comboBoxEditCurve);
            this.panelFanCurve.Controls.Add(this.labelHeaderTemp);
            this.panelFanCurve.Controls.Add(this.labelHeaderFan);
            this.panelFanCurve.Controls.Add(this.labelPoint1);
            this.panelFanCurve.Controls.Add(this.labelPoint2);
            this.panelFanCurve.Controls.Add(this.labelPoint3);
            this.panelFanCurve.Controls.Add(this.numericTemp1);
            this.panelFanCurve.Controls.Add(this.numericFan1);
            this.panelFanCurve.Controls.Add(this.numericTemp2);
            this.panelFanCurve.Controls.Add(this.numericFan2);
            this.panelFanCurve.Controls.Add(this.numericTemp3);
            this.panelFanCurve.Controls.Add(this.numericFan3);
            this.panelFanCurve.Controls.Add(this.labelCurveHint);
            this.panelFanCurve.Controls.Add(this.labelCurveStatus);
            this.panelFanCurve.Location = new System.Drawing.Point(12, 76);
            this.panelFanCurve.Name = "panelFanCurve";
            this.panelFanCurve.Size = new System.Drawing.Size(368, 190);
            this.panelFanCurve.TabIndex = 14;
            this.panelFanCurve.Visible = false;
            // 
            // labelSetpoints
            // 
            this.labelSetpoints.AutoSize = true;
            this.labelSetpoints.Location = new System.Drawing.Point(8, 8);
            this.labelSetpoints.Name = "labelSetpoints";
            this.labelSetpoints.Size = new System.Drawing.Size(56, 13);
            this.labelSetpoints.TabIndex = 0;
            this.labelSetpoints.Text = "Setpoints";
            // 
            // comboBoxSetpointCount
            // 
            this.comboBoxSetpointCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSetpointCount.Items.AddRange(new object[] {
            "2",
            "3"});
            this.comboBoxSetpointCount.Location = new System.Drawing.Point(70, 4);
            this.comboBoxSetpointCount.Name = "comboBoxSetpointCount";
            this.comboBoxSetpointCount.Size = new System.Drawing.Size(44, 21);
            this.comboBoxSetpointCount.TabIndex = 1;
            this.comboBoxSetpointCount.SelectedIndexChanged += new System.EventHandler(this.setpointCount_Changed);
            // 
            // checkBoxUseGpu
            // 
            this.checkBoxUseGpu.AutoSize = true;
            this.checkBoxUseGpu.Location = new System.Drawing.Point(140, 6);
            this.checkBoxUseGpu.Name = "checkBoxUseGpu";
            this.checkBoxUseGpu.Size = new System.Drawing.Size(104, 17);
            this.checkBoxUseGpu.TabIndex = 2;
            this.checkBoxUseGpu.Text = "Use GPU curve";
            this.checkBoxUseGpu.UseVisualStyleBackColor = true;
            this.checkBoxUseGpu.CheckedChanged += new System.EventHandler(this.useGpu_CheckedChanged);
            // 
            // labelEditCurve
            // 
            this.labelEditCurve.AutoSize = true;
            this.labelEditCurve.Location = new System.Drawing.Point(8, 32);
            this.labelEditCurve.Name = "labelEditCurve";
            this.labelEditCurve.Size = new System.Drawing.Size(25, 13);
            this.labelEditCurve.TabIndex = 3;
            this.labelEditCurve.Text = "Edit";
            // 
            // comboBoxEditCurve
            // 
            this.comboBoxEditCurve.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxEditCurve.Items.AddRange(new object[] {
            "CPU",
            "GPU"});
            this.comboBoxEditCurve.Location = new System.Drawing.Point(70, 28);
            this.comboBoxEditCurve.Name = "comboBoxEditCurve";
            this.comboBoxEditCurve.Size = new System.Drawing.Size(80, 21);
            this.comboBoxEditCurve.TabIndex = 4;
            this.comboBoxEditCurve.SelectedIndexChanged += new System.EventHandler(this.editCurve_Changed);
            // 
            // labelHeaderTemp
            // 
            this.labelHeaderTemp.AutoSize = true;
            this.labelHeaderTemp.Location = new System.Drawing.Point(52, 54);
            this.labelHeaderTemp.Name = "labelHeaderTemp";
            this.labelHeaderTemp.Size = new System.Drawing.Size(21, 13);
            this.labelHeaderTemp.TabIndex = 5;
            this.labelHeaderTemp.Text = "°C";
            // 
            // labelHeaderFan
            // 
            this.labelHeaderFan.AutoSize = true;
            this.labelHeaderFan.Location = new System.Drawing.Point(128, 54);
            this.labelHeaderFan.Name = "labelHeaderFan";
            this.labelHeaderFan.Size = new System.Drawing.Size(38, 13);
            this.labelHeaderFan.TabIndex = 6;
            this.labelHeaderFan.Text = "Fan %";
            // 
            // labelPoint1
            // 
            this.labelPoint1.AutoSize = true;
            this.labelPoint1.Location = new System.Drawing.Point(8, 76);
            this.labelPoint1.Name = "labelPoint1";
            this.labelPoint1.Size = new System.Drawing.Size(13, 13);
            this.labelPoint1.TabIndex = 7;
            this.labelPoint1.Text = "1";
            // 
            // labelPoint2
            // 
            this.labelPoint2.AutoSize = true;
            this.labelPoint2.Location = new System.Drawing.Point(8, 102);
            this.labelPoint2.Name = "labelPoint2";
            this.labelPoint2.Size = new System.Drawing.Size(13, 13);
            this.labelPoint2.TabIndex = 8;
            this.labelPoint2.Text = "2";
            // 
            // labelPoint3
            // 
            this.labelPoint3.AutoSize = true;
            this.labelPoint3.Location = new System.Drawing.Point(8, 128);
            this.labelPoint3.Name = "labelPoint3";
            this.labelPoint3.Size = new System.Drawing.Size(13, 13);
            this.labelPoint3.TabIndex = 9;
            this.labelPoint3.Text = "3";
            // 
            // numericTemp1
            // 
            this.numericTemp1.Location = new System.Drawing.Point(36, 72);
            this.numericTemp1.Maximum = new decimal(new int[] { 110, 0, 0, 0 });
            this.numericTemp1.Name = "numericTemp1";
            this.numericTemp1.Size = new System.Drawing.Size(58, 20);
            this.numericTemp1.TabIndex = 10;
            this.numericTemp1.ValueChanged += new System.EventHandler(this.curveEditor_Changed);
            // 
            // numericFan1
            // 
            this.numericFan1.Location = new System.Drawing.Point(116, 72);
            this.numericFan1.Name = "numericFan1";
            this.numericFan1.Size = new System.Drawing.Size(58, 20);
            this.numericFan1.TabIndex = 11;
            this.numericFan1.ValueChanged += new System.EventHandler(this.curveEditor_Changed);
            // 
            // numericTemp2
            // 
            this.numericTemp2.Location = new System.Drawing.Point(36, 98);
            this.numericTemp2.Maximum = new decimal(new int[] { 110, 0, 0, 0 });
            this.numericTemp2.Name = "numericTemp2";
            this.numericTemp2.Size = new System.Drawing.Size(58, 20);
            this.numericTemp2.TabIndex = 12;
            this.numericTemp2.ValueChanged += new System.EventHandler(this.curveEditor_Changed);
            // 
            // numericFan2
            // 
            this.numericFan2.Location = new System.Drawing.Point(116, 98);
            this.numericFan2.Name = "numericFan2";
            this.numericFan2.Size = new System.Drawing.Size(58, 20);
            this.numericFan2.TabIndex = 13;
            this.numericFan2.ValueChanged += new System.EventHandler(this.curveEditor_Changed);
            // 
            // numericTemp3
            // 
            this.numericTemp3.Location = new System.Drawing.Point(36, 124);
            this.numericTemp3.Maximum = new decimal(new int[] { 110, 0, 0, 0 });
            this.numericTemp3.Name = "numericTemp3";
            this.numericTemp3.Size = new System.Drawing.Size(58, 20);
            this.numericTemp3.TabIndex = 14;
            this.numericTemp3.ValueChanged += new System.EventHandler(this.curveEditor_Changed);
            // 
            // numericFan3
            // 
            this.numericFan3.Location = new System.Drawing.Point(116, 124);
            this.numericFan3.Name = "numericFan3";
            this.numericFan3.Size = new System.Drawing.Size(58, 20);
            this.numericFan3.TabIndex = 15;
            this.numericFan3.ValueChanged += new System.EventHandler(this.curveEditor_Changed);
            // 
            // labelCurveHint
            // 
            this.labelCurveHint.AutoSize = true;
            this.labelCurveHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelCurveHint.Location = new System.Drawing.Point(8, 150);
            this.labelCurveHint.Name = "labelCurveHint";
            this.labelCurveHint.Size = new System.Drawing.Size(214, 13);
            this.labelCurveHint.TabIndex = 16;
            this.labelCurveHint.Text = "Fans stay off below the first setpoint.";
            // 
            // labelCurveStatus
            // 
            this.labelCurveStatus.AutoSize = true;
            this.labelCurveStatus.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelCurveStatus.Location = new System.Drawing.Point(8, 168);
            this.labelCurveStatus.MaximumSize = new System.Drawing.Size(350, 0);
            this.labelCurveStatus.Name = "labelCurveStatus";
            this.labelCurveStatus.Size = new System.Drawing.Size(250, 13);
            this.labelCurveStatus.TabIndex = 17;
            this.labelCurveStatus.Text = "Current: CPU -- °C, GPU -- °C → Fan --%";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 372);
            this.Controls.Add(this.labelGPUTemp);
            this.Controls.Add(this.buttonRefreshGPUTemp);
            this.Controls.Add(this.labelGpuCaption);
            this.Controls.Add(this.labelCPUTemp);
            this.Controls.Add(this.buttonRefreshCPUTemp);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelRPM);
            this.Controls.Add(this.buttonRefreshRPM);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panelFanCurve);
            this.Controls.Add(this.panelManual);
            this.Controls.Add(this.radioFanCurve);
            this.Controls.Add(this.radioManual);
            this.Controls.Add(this.checkBoxTurnOn);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.Text = "Asus Fan Control";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFanSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTemp1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTemp2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTemp3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericFan1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericFan2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericFan3)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelManual.ResumeLayout(false);
            this.panelManual.PerformLayout();
            this.panelFanCurve.ResumeLayout(false);
            this.panelFanCurve.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

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
        private System.Windows.Forms.Label labelGPUTemp;
        private System.Windows.Forms.Button buttonRefreshGPUTemp;
        private System.Windows.Forms.Label labelGpuCaption;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemTurnOffControlOnExit;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemForbidUnsafeSettings;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCheckForUpdates;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMinimizeToTrayOnClose;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAutoRefreshStats;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemStartWithWindows;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFansOffOnSleep;
        private System.Windows.Forms.RadioButton radioManual;
        private System.Windows.Forms.RadioButton radioFanCurve;
        private System.Windows.Forms.Panel panelManual;
        private System.Windows.Forms.Panel panelFanCurve;
        private System.Windows.Forms.Label labelSetpoints;
        private System.Windows.Forms.ComboBox comboBoxSetpointCount;
        private System.Windows.Forms.CheckBox checkBoxUseGpu;
        private System.Windows.Forms.Label labelEditCurve;
        private System.Windows.Forms.ComboBox comboBoxEditCurve;
        private System.Windows.Forms.Label labelHeaderTemp;
        private System.Windows.Forms.Label labelHeaderFan;
        private System.Windows.Forms.Label labelPoint1;
        private System.Windows.Forms.Label labelPoint2;
        private System.Windows.Forms.Label labelPoint3;
        private System.Windows.Forms.NumericUpDown numericTemp1;
        private System.Windows.Forms.NumericUpDown numericTemp2;
        private System.Windows.Forms.NumericUpDown numericTemp3;
        private System.Windows.Forms.NumericUpDown numericFan1;
        private System.Windows.Forms.NumericUpDown numericFan2;
        private System.Windows.Forms.NumericUpDown numericFan3;
        private System.Windows.Forms.Label labelCurveHint;
        private System.Windows.Forms.Label labelCurveStatus;
    }
}
