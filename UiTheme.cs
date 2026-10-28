using System.Drawing;
using System.Windows.Forms;

namespace BankManagementSystem
{
    internal static class UiTheme
    {
        public static readonly Color Navy = Color.FromArgb(21, 61, 104);
        public static readonly Color Blue = Color.FromArgb(31, 105, 166);
        public static readonly Color LightBlue = Color.FromArgb(232, 242, 252);
        public static readonly Color Page = Color.FromArgb(244, 247, 251);
        public static readonly Color Text = Color.FromArgb(35, 50, 70);

        public static void StyleGrid(DataGridView grid)
        {
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Color.FromArgb(226, 232, 240);
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Navy,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                SelectionBackColor = Navy,
                Padding = new Padding(8, 0, 8, 0)
            };
            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(45, 55, 72),
                Font = new Font("Segoe UI", 9),
                SelectionBackColor = LightBlue,
                SelectionForeColor = Color.FromArgb(20, 60, 100),
                Padding = new Padding(8, 5, 8, 5)
            };
            grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 250, 253)
            };
            grid.RowTemplate.Height = 34;
            grid.ColumnHeadersHeight = 38;
            grid.RowHeadersVisible = false;
            grid.AllowUserToResizeRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        public static void StyleButton(Button button, bool primary = false)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = primary ? Blue : Color.White;
            button.ForeColor = primary ? Color.White : Navy;
            button.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.Padding = new Padding(8, 0, 8, 0);
        }

        public static void StyleButtons(Control root)
        {
            foreach (Control control in root.Controls)
            {
                if (control is Button button)
                    StyleButton(button);
                if (control.HasChildren)
                    StyleButtons(control);
            }
        }
    }
}
