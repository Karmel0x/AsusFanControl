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
        
        private DataGridView dataGridViewCurve;
        private Button buttonOK;
        private Button buttonCancel;
        private Button buttonAddPoint;
        private Button buttonRemovePoint;
        
        public FanCurveSettingsForm(FanCurve fanCurve)
        {
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
            this.dataGridViewCurve = new DataGridView();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.buttonAddPoint = new Button();
            this.buttonRemovePoint = new Button();
            
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCurve)).BeginInit();
            this.SuspendLayout();
            
            // dataGridViewCurve
            this.dataGridViewCurve.AllowUserToAddRows = false;
            this.dataGridViewCurve.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCurve.Location = new Point(12, 12);
            this.dataGridViewCurve.Name = "dataGridViewCurve";
            this.dataGridViewCurve.Size = new Size(360, 200);
            this.dataGridViewCurve.TabIndex = 0;
            
            var tempColumn = new DataGridViewTextBoxColumn();
            tempColumn.HeaderText = "Temperature (°C)";
            tempColumn.Name = "Temperature";
            tempColumn.Width = 150;
            this.dataGridViewCurve.Columns.Add(tempColumn);
            
            var speedColumn = new DataGridViewTextBoxColumn();
            speedColumn.HeaderText = "Fan Speed (%)";
            speedColumn.Name = "FanSpeed";
            speedColumn.Width = 150;
            this.dataGridViewCurve.Columns.Add(speedColumn);
            
            // buttonAddPoint
            this.buttonAddPoint.Location = new Point(12, 218);
            this.buttonAddPoint.Name = "buttonAddPoint";
            this.buttonAddPoint.Size = new Size(75, 23);
            this.buttonAddPoint.TabIndex = 1;
            this.buttonAddPoint.Text = "Add Point";
            this.buttonAddPoint.UseVisualStyleBackColor = true;
            this.buttonAddPoint.Click += new EventHandler(this.buttonAddPoint_Click);
            
            // buttonRemovePoint
            this.buttonRemovePoint.Location = new Point(93, 218);
            this.buttonRemovePoint.Name = "buttonRemovePoint";
            this.buttonRemovePoint.Size = new Size(90, 23);
            this.buttonRemovePoint.TabIndex = 2;
            this.buttonRemovePoint.Text = "Remove Point";
            this.buttonRemovePoint.UseVisualStyleBackColor = true;
            this.buttonRemovePoint.Click += new EventHandler(this.buttonRemovePoint_Click);
            
            // buttonOK
            this.buttonOK.DialogResult = DialogResult.OK;
            this.buttonOK.Location = new Point(216, 218);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new EventHandler(this.buttonOK_Click);
            
            // buttonCancel
            this.buttonCancel.DialogResult = DialogResult.Cancel;
            this.buttonCancel.Location = new Point(297, 218);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            
            // FanCurveSettingsForm
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(384, 253);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonRemovePoint);
            this.Controls.Add(this.buttonAddPoint);
            this.Controls.Add(this.dataGridViewCurve);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FanCurveSettingsForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Fan Curve Settings";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCurve)).EndInit();
            this.ResumeLayout(false);
        }
        
        private void LoadCurveData()
        {
            dataGridViewCurve.Rows.Clear();
            foreach (var point in FanCurve.Points.OrderBy(p => p.Temperature))
            {
                dataGridViewCurve.Rows.Add(point.Temperature, point.FanSpeed);
            }
        }
        
        private void buttonAddPoint_Click(object sender, EventArgs e)
        {
            var newTemp = 40;
            var newSpeed = 50;
            
            if (FanCurve.Points.Count > 0)
            {
                var maxTemp = FanCurve.Points.Max(p => p.Temperature);
                newTemp = maxTemp + 10;
            }
            
            FanCurve.Points.Add(new FanCurvePoint(newTemp, newSpeed));
            LoadCurveData();
        }
        
        private void buttonRemovePoint_Click(object sender, EventArgs e)
        {
            if (dataGridViewCurve.SelectedRows.Count > 0 && FanCurve.Points.Count > 1)
            {
                var selectedIndex = dataGridViewCurve.SelectedRows[0].Index;
                if (selectedIndex < FanCurve.Points.Count)
                {
                    FanCurve.Points.RemoveAt(selectedIndex);
                    LoadCurveData();
                }
            }
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
                        if (temp >= 0 && temp <= 100 && speed >= 0 && speed <= 100)
                        {
                            FanCurve.Points.Add(new FanCurvePoint(temp, speed));
                        }
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
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}