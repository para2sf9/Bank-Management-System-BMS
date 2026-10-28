using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BankManagementSystem
{
    public class AttendancePanel : Panel
    {
        private readonly DatabaseService _db;
        private readonly string _role;
        private DataGridView _grid;
        private TextBox _txtDate;
        private ComboBox _cmbEmployee;

        public AttendancePanel(DatabaseService db, string role)
        {
            _db = db;
            _role = role;
            BackColor = UiTheme.Page;
            Padding = new Padding(20);
            Initialize();
            UiTheme.StyleButtons(this);
            LoadDataAsync();
        }

        private void Initialize()
        {
            var lblTitle = new Label
            {
                Text = "Attendance",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            Controls.Add(lblTitle);

            var top = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(900, 40),
                BackColor = BackColor
            };

            top.Controls.Add(new Label { Text = "Date (YYYY-MM-DD):", AutoSize = true, Location = new Point(0, 8) });
            _txtDate = new TextBox { Text = DateTime.Now.ToString("yyyy-MM-dd"), Location = new Point(140, 5), Width = 120 };
            var btnLoad = new Button { Text = "Load", Location = new Point(270, 5), Width = 80 };
            btnLoad.Click += async (s, e) => await LoadDataAsync();
            top.Controls.Add(_txtDate);
            top.Controls.Add(btnLoad);

            if (_role == "admin" || _role == "hr")
            {
                var btnMark = new Button { Text = "Mark Attendance", Location = new Point(740, 5), Width = 130 };
                btnMark.Click += (s, e) => ShowMarkForm();
                top.Controls.Add(btnMark);
            }

            Controls.Add(top);

            _grid = new DataGridView
            {
                Location = new Point(20, 110),
                Size = new Size(900, 450),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false
            };
            UiTheme.StyleGrid(_grid);
            Controls.Add(_grid);
        }

        private async Task LoadDataAsync()
        {
            var records = await _db.GetAttendanceAsync(_txtDate.Text.Trim());
            var empMap = _db.GetEmployeeNamesMap();
            _grid.DataSource = records.Select(r => new
            {
                r.EmployeeId,
                Name = empMap.ContainsKey(r.EmployeeId) ? empMap[r.EmployeeId] : "",
                r.Date,
                r.Status,
                r.Notes
            }).ToList();
        }

        private void ShowMarkForm()
        {
            using var form = new MarkAttendanceForm(_db);
            if (form.ShowDialog() == DialogResult.OK)
                LoadDataAsync();
        }
    }
}
