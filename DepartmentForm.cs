using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BankManagementSystem
{
    public class DepartmentForm : Form
    {
        private readonly DatabaseService _db;
        private readonly Department? _dept;
        private TextBox txtId, txtName, txtManager, txtLocation;

        public DepartmentForm(DatabaseService db, Department? dept)
        {
            _db = db;
            _dept = dept;
            Text = dept == null ? "Add Department" : "Edit Department";
            Size = new Size(480, 320);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;
            Initialize();
        }

        private void Initialize()
        {
            int y = 20, gap = 45, xLabel = 30, xBox = 150;
            var AddRow = (string label, Control ctrl) => {
                Controls.Add(new Label { Text = label, Location = new Point(xLabel, y), AutoSize = true });
                ctrl.Location = new Point(xBox, y - 3);
                ctrl.Width = 260;
                Controls.Add(ctrl);
                y += gap;
            };

            txtId = new TextBox();
            txtName = new TextBox();
            txtManager = new TextBox();
            txtLocation = new TextBox();

            AddRow("Department ID:", txtId);
            AddRow("Name:", txtName);
            AddRow("Manager:", txtManager);
            AddRow("Location:", txtLocation);

            var btnSave = new Button
            {
                Text = "Save",
                Location = new Point(xBox, y + 5),
                Width = 100,
                Height = 35,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += async (s, e) =>
            {
                btnSave.Enabled = false;
                try { await SaveAsync(); }
                catch (Exception ex) { MessageBox.Show($"Unable to save department: {ex.Message}", "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                finally { if (!IsDisposed) btnSave.Enabled = true; }
            };
            Controls.Add(btnSave);

            if (_dept != null)
            {
                txtId.Text = _dept.DepartmentId;
                txtId.ReadOnly = true;
                txtName.Text = _dept.Name;
                txtManager.Text = _dept.Manager;
                txtLocation.Text = _dept.Location;
            }
        }

        private async Task SaveAsync()
        {
            var d = new Department
            {
                DepartmentId = txtId.Text.Trim(),
                Name = txtName.Text.Trim(),
                Manager = txtManager.Text.Trim(),
                Location = txtLocation.Text.Trim()
            };
            if (string.IsNullOrEmpty(d.DepartmentId) || string.IsNullOrEmpty(d.Name))
            {
                MessageBox.Show("Department ID and Name are required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_dept != null)
                await _db.UpdateDepartmentAsync(d.DepartmentId, d);
            else
                await _db.InsertDepartmentAsync(d);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
