using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BankManagementSystem
{
    public class EmployeesPanel : Panel
    {
        private readonly DatabaseService _db;
        private readonly string _role;
        private DataGridView _grid;
        private TextBox _txtSearch;

        public EmployeesPanel(DatabaseService db, string role)
        {
            _db = db;
            _role = role;
            BackColor = UiTheme.Page;
            Padding = new Padding(20);
            InitializeComponents();
            UiTheme.StyleButtons(this);
            LoadDataAsync();
        }

        private void InitializeComponents()
        {
            var lblTitle = new Label
            {
                Text = "Employees",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            Controls.Add(lblTitle);

            var topPanel = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(900, 40),
                BackColor = BackColor
            };

            var lblSearch = new Label { Text = "Search:", AutoSize = true, Location = new Point(0, 8) };
            _txtSearch = new TextBox { Location = new Point(60, 5), Width = 200 };
            var btnSearch = new Button { Text = "Search", Location = new Point(270, 5), Width = 80 };
            var btnRefresh = new Button { Text = "Refresh", Location = new Point(360, 5), Width = 80 };

            btnSearch.Click += async (s, e) => await LoadDataAsync();
            btnRefresh.Click += async (s, e) => await LoadDataAsync();

            topPanel.Controls.Add(lblSearch);
            topPanel.Controls.Add(_txtSearch);
            topPanel.Controls.Add(btnSearch);
            topPanel.Controls.Add(btnRefresh);

            if (_role == "admin" || _role == "hr")
            {
                var btnAdd = new Button { Text = "Add Employee", Location = new Point(750, 5), Width = 120 };
                btnAdd.Click += (s, e) => ShowEmployeeForm(null);
                topPanel.Controls.Add(btnAdd);
            }

            Controls.Add(topPanel);

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

            if (_role == "admin" || _role == "hr")
            {
                var btnEdit = new Button { Text = "Edit", Location = new Point(20, 570), Width = 80 };
                var btnDelete = new Button { Text = "Delete", Location = new Point(110, 570), Width = 80 };
                btnEdit.Click += (s, e) => EditSelected();
                btnDelete.Click += async (s, e) => await DeleteSelectedAsync();
                Controls.Add(btnEdit);
                Controls.Add(btnDelete);
            }
        }

        private async Task LoadDataAsync()
        {
            var employees = await _db.GetEmployeesAsync(_txtSearch.Text.Trim());
            _grid.DataSource = employees.Select(e => new
            {
                e.EmployeeId,
                e.FirstName,
                e.LastName,
                e.Email,
                e.DepartmentId,
                e.Position,
                e.Salary,
                e.Status
            }).ToList();
        }

        private void ShowEmployeeForm(Employee? employee)
        {
            using var form = new EmployeeForm(_db, employee);
            if (form.ShowDialog() == DialogResult.OK)
                LoadDataAsync();
        }

        private void EditSelected()
        {
            if (_grid.SelectedRows.Count == 0) return;
            var empId = _grid.SelectedRows[0].Cells[0].Value?.ToString();
            if (string.IsNullOrEmpty(empId)) return;
            var emp = _db.GetEmployeeById(empId);
            if (emp != null) ShowEmployeeForm(emp);
        }

        private async Task DeleteSelectedAsync()
        {
            if (_grid.SelectedRows.Count == 0) return;
            var empId = _grid.SelectedRows[0].Cells[0].Value?.ToString();
            if (string.IsNullOrEmpty(empId)) return;
            if (MessageBox.Show("Delete this employee?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                await _db.SoftDeleteEmployeeAsync(empId);
                await LoadDataAsync();
            }
        }
    }
}
