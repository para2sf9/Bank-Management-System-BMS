using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BankManagementSystem
{
    public class PayrollPanel : Panel
    {
        private readonly DatabaseService _db;
        private readonly string _role;
        private DataGridView _grid;
        private ComboBox cmbMonth;
        private TextBox txtYear;

        public PayrollPanel(DatabaseService db, string role)
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
                Text = "Payroll",
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

            top.Controls.Add(new Label { Text = "Month:", AutoSize = true, Location = new Point(0, 8) });
            cmbMonth = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(50, 5), Width = 60 };
            for (int i = 1; i <= 12; i++) cmbMonth.Items.Add(i.ToString());
            cmbMonth.SelectedIndex = 0;
            top.Controls.Add(cmbMonth);

            top.Controls.Add(new Label { Text = "Year:", AutoSize = true, Location = new Point(120, 8) });
            txtYear = new TextBox { Text = "2024", Location = new Point(160, 5), Width = 80 };
            top.Controls.Add(txtYear);

            var btnLoad = new Button { Text = "Load", Location = new Point(260, 5), Width = 80 };
            btnLoad.Click += async (s, e) => await LoadDataAsync();
            top.Controls.Add(btnLoad);

            if (_role == "admin" || _role == "hr")
            {
                var btnGen = new Button { Text = "Generate Payroll", Location = new Point(740, 5), Width = 130 };
                btnGen.Click += (s, e) => ShowGenerateForm();
                top.Controls.Add(btnGen);
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
            var records = await _db.GetPayrollAsync(int.Parse(cmbMonth.SelectedItem.ToString()), int.Parse(txtYear.Text));
            var empMap = _db.GetEmployeeNamesMap();
            _grid.DataSource = records.Select(r => new
            {
                r.EmployeeId,
                Name = empMap.ContainsKey(r.EmployeeId) ? empMap[r.EmployeeId] : "",
                r.Month,
                r.Year,
                r.BaseSalary,
                r.Bonus,
                r.Deductions,
                r.NetSalary
            }).ToList();
        }

        private void ShowGenerateForm()
        {
            using var f = new GeneratePayrollForm(_db);
            if (f.ShowDialog() == DialogResult.OK)
                LoadDataAsync();
        }
    }
}
