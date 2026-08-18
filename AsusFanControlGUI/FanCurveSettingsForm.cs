using AsusFanControl;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AsusFanControlGUI
{
    public partial class FanCurveSettingsForm : Form
    {
        public FanCurve FanCurve { get; private set; }
        
        private FanCurveGraphControl graphControl;
        private DataGridView dataGridViewCurve;
        private Button buttonOK;
        private Button buttonCancel;
        private Button buttonAddPoint;
        private Button buttonRemovePoint;
        private ComboBox comboBoxPresets;
        private Label labelPresets;
        private Label labelHelpTip;
        private bool isUpdatingGrid = false;
        private int currentTemp = -1;

        public FanCurveSettingsForm(FanCurve fanCurve, int currentTemperature = -1)
        {
            currentTemp = currentTemperature;
            FanCurve = new FanCurve();
            FanCurve.Points.Clear();
            foreach (var point in fanCurve.Points)
            {
                FanCurve.Points.Add(new FanCurvePoint(point.Temperature, point.FanSpeed));
            }
            FanCurve.IsEnabled = fanCurve.IsEnabled;
            
            InitializeComponent();
            LoadCurveData();
        }
        
        private void InitializeComponent()
        {
            this.graphControl = new FanCurveGraphControl();
            this.dataGridViewCurve = new DataGridView();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.buttonAddPoint = new Button();
            this.buttonRemovePoint = new Button();
            this.comboBoxPresets = new ComboBox();
            this.labelPresets = new Label();
            this.labelHelpTip = new Label();
            
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCurve)).BeginInit();
            this.SuspendLayout();
            
            // FanCurveSettingsForm
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(720, 485);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Fan Curve Editor & Graph";
            this.BackColor = Color.FromArgb(30, 30, 40);
            this.ForeColor = Color.White;

            // graphControl
            this.graphControl.Location = new Point(12, 12);
            this.graphControl.Name = "graphControl";
            this.graphControl.Size = new Size(465, 415);
            this.graphControl.TabIndex = 0;
            this.graphControl.FanCurve = this.FanCurve;
            this.graphControl.CurrentTemperature = currentTemp;
            this.graphControl.ForbidUnsafeSettings = Properties.Settings.Default.forbidUnsafeSettings;
            this.graphControl.CurveChanged += GraphControl_CurveChanged;

            // DataGridView
            this.dataGridViewCurve.AllowUserToAddRows = false;
            this.dataGridViewCurve.AllowUserToDeleteRows = false;
            this.dataGridViewCurve.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCurve.Location = new Point(488, 12);
            this.dataGridViewCurve.Name = "dataGridViewCurve";
            this.dataGridViewCurve.Size = new Size(220, 220);
            this.dataGridViewCurve.TabIndex = 1;
            this.dataGridViewCurve.BackgroundColor = Color.FromArgb(40, 40, 52);
            this.dataGridViewCurve.ForeColor = Color.Black;
            this.dataGridViewCurve.CellValueChanged += DataGridViewCurve_CellValueChanged;

            var tempColumn = new DataGridViewTextBoxColumn();
            tempColumn.HeaderText = "Temp (°C)";
            tempColumn.Name = "Temperature";
            tempColumn.Width = 95;
            this.dataGridViewCurve.Columns.Add(tempColumn);
            
            var speedColumn = new DataGridViewTextBoxColumn();
            speedColumn.HeaderText = "Speed (%)";
            speedColumn.Name = "FanSpeed";
            speedColumn.Width = 95;
            this.dataGridViewCurve.Columns.Add(speedColumn);

            // buttonAddPoint
            this.buttonAddPoint.Location = new Point(488, 238);
            this.buttonAddPoint.Name = "buttonAddPoint";
            this.buttonAddPoint.Size = new Size(105, 26);
            this.buttonAddPoint.TabIndex = 2;
            this.buttonAddPoint.Text = "+ Add Point";
            this.buttonAddPoint.UseVisualStyleBackColor = true;
            this.buttonAddPoint.Click += new EventHandler(this.buttonAddPoint_Click);
            
            // buttonRemovePoint
            this.buttonRemovePoint.Location = new Point(603, 238);
            this.buttonRemovePoint.Name = "buttonRemovePoint";
            this.buttonRemovePoint.Size = new Size(105, 26);
            this.buttonRemovePoint.TabIndex = 3;
            this.buttonRemovePoint.Text = "- Remove Point";
            this.buttonRemovePoint.UseVisualStyleBackColor = true;
            this.buttonRemovePoint.Click += new EventHandler(this.buttonRemovePoint_Click);

            // labelPresets
            this.labelPresets.AutoSize = true;
            this.labelPresets.Location = new Point(488, 280);
            this.labelPresets.Name = "labelPresets";
            this.labelPresets.Size = new Size(100, 13);
            this.labelPresets.Text = "Preset Profiles:";
            this.labelPresets.ForeColor = Color.FromArgb(200, 200, 220);

            // comboBoxPresets
            this.comboBoxPresets.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxPresets.FormattingEnabled = true;
            this.comboBoxPresets.Items.AddRange(new object[] {
                "-- Select Preset --",
                "Default Balanced",
                "Quiet Profile",
                "Aggressive Gaming",
                "Max Speed Profile"
            });
            this.comboBoxPresets.Location = new Point(488, 298);
            this.comboBoxPresets.Name = "comboBoxPresets";
            this.comboBoxPresets.Size = new Size(220, 21);
            this.comboBoxPresets.SelectedIndex = 0;
            this.comboBoxPresets.SelectedIndexChanged += ComboBoxPresets_SelectedIndexChanged;

            // labelHelpTip
            this.labelHelpTip.Location = new Point(12, 435);
            this.labelHelpTip.Name = "labelHelpTip";
            this.labelHelpTip.Size = new Size(465, 38);
            this.labelHelpTip.TabIndex = 6;
            this.labelHelpTip.Text = "💡 Drag points on graph to adjust curve • Double-click graph to add a point\r\n   Right-click point to delete • Safety limits (" + (Properties.Settings.Default.forbidUnsafeSettings ? "40%-99%" : "Unlocked") + ") active.";
            this.labelHelpTip.ForeColor = Color.FromArgb(170, 190, 220);
            this.labelHelpTip.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular);

            // buttonOK
            this.buttonOK.DialogResult = DialogResult.OK;
            this.buttonOK.Location = new Point(542, 435);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new Size(80, 30);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new EventHandler(this.buttonOK_Click);
            
            // buttonCancel
            this.buttonCancel.DialogResult = DialogResult.Cancel;
            this.buttonCancel.Location = new Point(628, 435);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new Size(80, 30);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            
            this.Controls.Add(this.labelHelpTip);
            this.Controls.Add(this.labelPresets);
            this.Controls.Add(this.comboBoxPresets);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonRemovePoint);
            this.Controls.Add(this.buttonAddPoint);
            this.Controls.Add(this.dataGridViewCurve);
            this.Controls.Add(this.graphControl);
            
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCurve)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void GraphControl_CurveChanged(object sender, EventArgs e)
        {
            LoadCurveData();
        }

        private void LoadCurveData()
        {
            isUpdatingGrid = true;
            dataGridViewCurve.Rows.Clear();
            
            FanCurve.Points.Sort((a, b) => a.Temperature.CompareTo(b.Temperature));
            foreach (var point in FanCurve.Points)
            {
                dataGridViewCurve.Rows.Add(point.Temperature, point.FanSpeed);
            }
            isUpdatingGrid = false;
        }

        private void DataGridViewCurve_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isUpdatingGrid || e.RowIndex < 0)
                return;

            try
            {
                var row = dataGridViewCurve.Rows[e.RowIndex];
                if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                {
                    if (int.TryParse(row.Cells[0].Value.ToString(), out int temp) &&
                        int.TryParse(row.Cells[1].Value.ToString(), out int speed))
                    {
                        temp = Math.Max(0, Math.Min(100, temp));
                        speed = Math.Max(0, Math.Min(100, speed));

                        if (Properties.Settings.Default.forbidUnsafeSettings && speed > 0)
                        {
                            speed = Math.Max(40, Math.Min(99, speed));
                        }

                        if (e.RowIndex < FanCurve.Points.Count)
                        {
                            FanCurve.Points[e.RowIndex].Temperature = temp;
                            FanCurve.Points[e.RowIndex].FanSpeed = speed;
                            FanCurve.Points.Sort((a, b) => a.Temperature.CompareTo(b.Temperature));
                            graphControl.Invalidate();
                        }
                    }
                }
            }
            catch { }
        }
        
        private void buttonAddPoint_Click(object sender, EventArgs e)
        {
            var newTemp = 50;
            var newSpeed = 60;
            
            if (FanCurve.Points.Count > 0)
            {
                var maxTemp = FanCurve.Points.Max(p => p.Temperature);
                newTemp = Math.Min(100, maxTemp + 10);
                newSpeed = FanCurve.Points.Last().FanSpeed;
            }

            if (Properties.Settings.Default.forbidUnsafeSettings && newSpeed > 0)
            {
                newSpeed = Math.Max(40, Math.Min(99, newSpeed));
            }
            
            FanCurve.Points.Add(new FanCurvePoint(newTemp, newSpeed));
            FanCurve.Points.Sort((a, b) => a.Temperature.CompareTo(b.Temperature));
            LoadCurveData();
            graphControl.Invalidate();
        }
        
        private void buttonRemovePoint_Click(object sender, EventArgs e)
        {
            if (graphControl.SelectedPointIndex >= 0)
            {
                graphControl.RemoveSelectedPoint();
                return;
            }

            if (dataGridViewCurve.SelectedRows.Count > 0 && FanCurve.Points.Count > 2)
            {
                var selectedIndex = dataGridViewCurve.SelectedRows[0].Index;
                if (selectedIndex < FanCurve.Points.Count)
                {
                    FanCurve.Points.RemoveAt(selectedIndex);
                    LoadCurveData();
                    graphControl.Invalidate();
                }
            }
            else if (FanCurve.Points.Count > 2)
            {
                FanCurve.Points.RemoveAt(FanCurve.Points.Count - 1);
                LoadCurveData();
                graphControl.Invalidate();
            }
        }

        private void ComboBoxPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxPresets.SelectedIndex <= 0) return;

            FanCurve.Points.Clear();
            bool unsafeLimit = Properties.Settings.Default.forbidUnsafeSettings;

            switch (comboBoxPresets.SelectedIndex)
            {
                case 1: // Default Balanced
                    FanCurve.Points.Add(new FanCurvePoint(30, unsafeLimit ? 40 : 30));
                    FanCurve.Points.Add(new FanCurvePoint(50, 50));
                    FanCurve.Points.Add(new FanCurvePoint(70, 80));
                    FanCurve.Points.Add(new FanCurvePoint(90, unsafeLimit ? 99 : 100));
                    break;
                case 2: // Quiet Profile
                    FanCurve.Points.Add(new FanCurvePoint(35, unsafeLimit ? 40 : 25));
                    FanCurve.Points.Add(new FanCurvePoint(60, 45));
                    FanCurve.Points.Add(new FanCurvePoint(75, 70));
                    FanCurve.Points.Add(new FanCurvePoint(90, 95));
                    break;
                case 3: // Aggressive Gaming
                    FanCurve.Points.Add(new FanCurvePoint(30, 50));
                    FanCurve.Points.Add(new FanCurvePoint(50, 70));
                    FanCurve.Points.Add(new FanCurvePoint(65, 88));
                    FanCurve.Points.Add(new FanCurvePoint(80, unsafeLimit ? 99 : 100));
                    break;
                case 4: // Max Speed Profile
                    FanCurve.Points.Add(new FanCurvePoint(30, 75));
                    FanCurve.Points.Add(new FanCurvePoint(50, 90));
                    FanCurve.Points.Add(new FanCurvePoint(65, unsafeLimit ? 99 : 100));
                    break;
            }

            LoadCurveData();
            graphControl.Invalidate();
        }
        
        private void buttonOK_Click(object sender, EventArgs e)
        {
            FanCurve.Points.Clear();
            
            foreach (DataGridViewRow row in dataGridViewCurve.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                {
                    if (int.TryParse(row.Cells[0].Value.ToString(), out int temp) &&
                        int.TryParse(row.Cells[1].Value.ToString(), out int speed))
                    {
                        temp = Math.Max(0, Math.Min(100, temp));
                        speed = Math.Max(0, Math.Min(100, speed));

                        if (Properties.Settings.Default.forbidUnsafeSettings && speed > 0)
                        {
                            speed = Math.Max(40, Math.Min(99, speed));
                        }

                        FanCurve.Points.Add(new FanCurvePoint(temp, speed));
                    }
                }
            }
            
            if (FanCurve.Points.Count < 2)
            {
                MessageBox.Show("Fan curve must have at least 2 points.", "Invalid Curve", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            FanCurve.Points.Sort((a, b) => a.Temperature.CompareTo(b.Temperature));
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}