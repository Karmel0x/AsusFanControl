using System;
using System.Drawing;
using System.Windows.Forms;

namespace AsusFanControlGUI
{
    /// <summary>
    /// Simple dialog for choosing the auto-refresh interval.
    /// The user drags a slider between 1 and 30 seconds.
    /// </summary>
    public class RefreshIntervalDialog : Form
    {
        public int IntervalMs { get; private set; }

        private TrackBar trackBarInterval;
        private Label labelPreview;
        private Button buttonOK;
        private Button buttonCancel;

        public RefreshIntervalDialog(int currentIntervalMs)
        {
            IntervalMs = currentIntervalMs;
            BuildUI(currentIntervalMs);
        }

        private void BuildUI(int currentIntervalMs)
        {
            this.Text = "Set Refresh Interval";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(300, 130);

            var labelTitle = new Label
            {
                Text = "Refresh interval (seconds):",
                AutoSize = true,
                Location = new Point(12, 12)
            };

            trackBarInterval = new TrackBar
            {
                Minimum = 1,
                Maximum = 30,
                TickFrequency = 1,
                LargeChange = 5,
                SmallChange = 1,
                Value = Math.Max(1, Math.Min(30, currentIntervalMs / 1000)),
                Location = new Point(12, 30),
                Size = new Size(276, 45)
            };
            trackBarInterval.ValueChanged += (s, e) =>
            {
                labelPreview.Text = $"{trackBarInterval.Value} second{(trackBarInterval.Value == 1 ? "" : "s")}";
            };

            labelPreview = new Label
            {
                Text = $"{trackBarInterval.Value} second{(trackBarInterval.Value == 1 ? "" : "s")}",
                AutoSize = true,
                Font = new Font(Font.FontFamily, 9f, FontStyle.Bold),
                Location = new Point(12, 78)
            };

            buttonOK = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(132, 95),
                Size = new Size(75, 23)
            };
            buttonOK.Click += (s, e) =>
            {
                IntervalMs = trackBarInterval.Value * 1000;
                this.Close();
            };

            buttonCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(213, 95),
                Size = new Size(75, 23)
            };

            this.Controls.AddRange(new Control[]
            {
                labelTitle, trackBarInterval, labelPreview, buttonOK, buttonCancel
            });

            this.AcceptButton = buttonOK;
            this.CancelButton = buttonCancel;
        }
    }
}
