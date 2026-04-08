using System.Drawing;
using System.Windows.Forms;

namespace AsusFanControlGUI
{
    public static class DarkTheme
    {
        public static Color Background       = Color.FromArgb(15, 15, 26);
        public static Color PanelBackground  = Color.FromArgb(22, 22, 46);
        public static Color GridLine         = Color.FromArgb(42, 42, 62);
        public static Color AccentBlue       = Color.FromArgb(79, 195, 247);
        public static Color AccentRed        = Color.FromArgb(239, 83, 80);
        public static Color TextPrimary      = Color.White;
        public static Color TextSecondary    = Color.FromArgb(140, 140, 180);
        public static Color ButtonBackground = Color.FromArgb(34, 34, 68);
        public static Color ButtonHover      = Color.FromArgb(51, 51, 102);
        public static Color WarningOrange    = Color.Orange;
        public static Color ErrorRed         = Color.FromArgb(239, 83, 80);
        public static Color SuccessGreen     = Color.FromArgb(102, 187, 106);

        /// <summary>Recursively applies dark colors to a control and all its children.</summary>
        public static void Apply(Control control)
        {
            control.BackColor = Background;
            control.ForeColor = TextPrimary;

            foreach (Control child in control.Controls)
            {
                if (child is Button btn)
                {
                    StyleButton(btn);
                }
                else if (child is ComboBox cb)
                {
                    StyleComboBox(cb);
                }
                else if (child is Label lbl)
                {
                    lbl.BackColor = Color.Transparent;
                    lbl.ForeColor = TextPrimary;
                }
                else if (child is CheckBox chk)
                {
                    chk.BackColor = Color.Transparent;
                    chk.ForeColor = TextPrimary;
                }
                else if (child is Panel || child is GroupBox || child is FlowLayoutPanel || child is TableLayoutPanel)
                {
                    child.BackColor = Background;
                    child.ForeColor = TextPrimary;
                    Apply(child);
                }
                else if (child is TabControl tc)
                {
                    tc.BackColor = Background;
                    tc.ForeColor = TextPrimary;
                    foreach (TabPage tp in tc.TabPages)
                    {
                        tp.BackColor = Background;
                        tp.ForeColor = TextPrimary;
                        Apply(tp);
                    }
                }
                else
                {
                    Apply(child);
                }
            }
        }

        public static void StyleButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 100);
            btn.FlatAppearance.BorderSize = 1;
            btn.BackColor = ButtonBackground;
            btn.ForeColor = TextPrimary;
            btn.Cursor = Cursors.Hand;

            btn.MouseEnter += (s, e) => btn.BackColor = ButtonHover;
            btn.MouseLeave += (s, e) => btn.BackColor = ButtonBackground;
        }

        public static void StyleComboBox(ComboBox cb)
        {
            cb.BackColor = ButtonBackground;
            cb.ForeColor = TextPrimary;
            cb.FlatStyle = FlatStyle.Flat;
        }
    }
}
