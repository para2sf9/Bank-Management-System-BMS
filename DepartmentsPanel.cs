using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BankManagementSystem
{
    public class DepartmentsPanel : Panel
    {
        private readonly DatabaseService _db;
        private readonly string _role;
        private DataGridView _grid;

        public DepartmentsPanel(DatabaseService db, string role)
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
                Text = "Departments",
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

            var btnRefresh = new Button { Text = "Refresh", Location = new Point(0, 5), Width = 80 };
            btnRefresh.Click += async (s, e) => await LoadDataAsync();
            top.Controls.Add(btnRefresh);

            if (_role == "admin" || _role == "hr")
            {
                var btnAdd = new Button { Text = "Add Department", Location = new Point(750, 5), Width = 120 };
                btnAdd.Click += (s, e) => ShowForm(null);
                top.Controls.Add(btnAdd);
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
            var list = await _db.GetDepartmentsAsync();
            _grid.DataSource = list.Select(d => new { d.DepartmentId, d.Name, d.Manager, d.Location }).ToList();
        }

        private void ShowForm(Department? d)
        {
            using var f = new DepartmentForm(_db, d);
            if (f.ShowDialog() == DialogResult.OK)
                LoadDataAsync();
        }

        private void EditSelected()
        {
            if (_grid.SelectedRows.Count == 0) return;
            var id = _grid.SelectedRows[0].Cells[0].Value?.ToString();
            var d = _db.GetDepartmentById(id);
            if (d != null) ShowForm(d);
        }

        private async Task DeleteSelectedAsync()
        {
            if (_grid.SelectedRows.Count == 0) return;
            var id = _grid.SelectedRows[0].Cells[0].Value?.ToString();
            if (MessageBox.Show("Delete this department?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                await _db.DeleteDepartmentAsync(id);
                await LoadDataAsync();
            }
        }
    }
}
