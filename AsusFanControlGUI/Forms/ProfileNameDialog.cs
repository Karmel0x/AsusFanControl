using System.Windows.Forms;
using System.Drawing;

namespace AsusFanControlGUI.Forms
{
    /// <summary>A minimal modal dialog that asks the user to type a name string.</summary>
    public class ProfileNameDialog : Form
    {
        private TextBox _textBox;
        private Button _btnOk;
        private Button _btnCancel;
        private Label _promptLabel;

        public string InputValue => _textBox.Text.Trim();

        private ProfileNameDialog(string title, string prompt)
        {
            Text = title;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(340, 120);
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = DarkTheme.Background;
            ForeColor = DarkTheme.TextPrimary;

            _promptLabel = new Label
            {
                Text = prompt,
                Location = new Point(12, 14),
                Size = new Size(316, 18),
                ForeColor = DarkTheme.TextSecondary
            };

            _textBox = new TextBox
            {
                Location = new Point(12, 36),
                Size = new Size(316, 22),
                BackColor = DarkTheme.ButtonBackground,
                ForeColor = DarkTheme.TextPrimary,
                BorderStyle = BorderStyle.FixedSingle
            };

            _btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(156, 72),
                Size = new Size(80, 28)
            };
            DarkTheme.StyleButton(_btnOk);

            _btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(248, 72),
                Size = new Size(80, 28)
            };
            DarkTheme.StyleButton(_btnCancel);

            AcceptButton = _btnOk;
            CancelButton = _btnCancel;

            Controls.Add(_promptLabel);
            Controls.Add(_textBox);
            Controls.Add(_btnOk);
            Controls.Add(_btnCancel);
        }

        /// <summary>
        /// Shows the dialog. Returns the trimmed name entered by the user, or null if cancelled.
        /// </summary>
        public static string Show(string title, string prompt, IWin32Window owner = null)
        {
            using (var dlg = new ProfileNameDialog(title, prompt))
            {
                var result = owner != null
                    ? dlg.ShowDialog(owner)
                    : dlg.ShowDialog();

                if (result == DialogResult.OK && dlg.InputValue.Length > 0)
                    return dlg.InputValue;
                return null;
            }
        }
    }
}
