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

        public Form1()
        {
            InitializeComponent();
        }

        private void setFanSpeed(int value)
        {
            if (value < 40)
                value = 0;
            if (value > 99)
                value = 99;

            labelValue.Text = value == 0 ? "turned off (set value between 40 and 100)" : value.ToString();
            asusControl.SetFansSpeed(value);
        }

        private void trackBar1_MouseCaptureChanged(object sender, EventArgs e)
        {
            setFanSpeed(trackBar1.Value);
        }

        private void trackBar1_KeyUp(object sender, KeyEventArgs e)
        {
            setFanSpeed(trackBar1.Value);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            labelRPM.Text = string.Join(" ", asusControl.GetFanSpeeds());
        }

    }
}
