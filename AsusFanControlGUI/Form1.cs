using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AsusFanControl;

namespace AsusFanControlGUI
{
    public partial class Form1 : Form
    {
        AsusControl asusControl = new AsusControl();
        int fanSpeed = 0;

        public Form1()
        {
            InitializeComponent();

            trackBarFanSpeed.Value = Properties.Settings.Default.fanSpeed;
        }

        private void setFanSpeed()
        {
            var value = trackBarFanSpeed.Value;
            Properties.Settings.Default.fanSpeed = value;
            Properties.Settings.Default.Save();

            if (!checkBoxTurnOn.Checked)
                value = 0;
            else if(value < 40)
                value = 0;
            else if (value > 99)
                value = 99;

            if (!checkBoxTurnOn.Checked)
                labelValue.Text = "turned off";
            else if (value == 0)
                labelValue.Text = "turned off (set value between 40 and 100)";
            else
                labelValue.Text = value.ToString();

            if (fanSpeed == value)
                return;

            fanSpeed = value;

            asusControl.SetFansSpeed(value);
        }

        private void trackBar1_MouseCaptureChanged(object sender, EventArgs e)
        {
            setFanSpeed();
        }

        private void trackBar1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Left && e.KeyCode != Keys.Right)
                return;

            setFanSpeed();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            labelRPM.Text = string.Join(" ", asusControl.GetFanSpeeds());
        }

        private void checkBoxTurnOn_CheckedChanged(object sender, EventArgs e)
        {
            setFanSpeed();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            labelCPUTemp.Text = $"{asusControl.Thermal_Read_Cpu_Temperature()}";
        }
    }
}
