using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BankManagementSystem
{
    public class EmployeeForm : Form
    {
        private readonly DatabaseService _db;
        private readonly Employee? _employee;
        private TextBox txtId, txtFirst, txtLast, txtEmail, txtPhone, txtDept, txtPos, txtSalary, txtDate;
        private ComboBox cmbStatus;

        public EmployeeForm(DatabaseService db, Employee? employee)
        {
            _db = db;
            _employee = employee;
            Text = employee == null ? "Add Employee" : "Edit Employee";
            Size = new Size(520, 520);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;
            Initialize();
        }

        private void Initialize()
        {
            int y = 20, gap = 40, xLabel = 30, xBox = 160;
            var AddRow = (string label, Control ctrl) => {
                Controls.Add(new Label { Text = label, Location = new Point(xLabel, y), AutoSize = true });
                ctrl.Location = new Point(xBox, y - 3);
                ctrl.Width = 280;
                Controls.Add(ctrl);
                y += gap;
            };

            txtId = new TextBox();
            txtFirst = new TextBox();
            txtLast = new TextBox();
            txtEmail = new TextBox();
            txtPhone = new TextBox();
            txtDept = new TextBox();
            txtPos = new TextBox();
            txtSalary = new TextBox();
            txtDate = new TextBox();
            cmbStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatus.Items.AddRange(new[] { "Active", "Inactive", "On Leave" });
            cmbStatus.SelectedIndex = 0;

            AddRow("Employee ID:", txtId);
            AddRow("First Name:", txtFirst);
            AddRow("Last Name:", txtLast);
            AddRow("Email:", txtEmail);
            AddRow("Phone:", txtPhone);
            AddRow("Department ID:", txtDept);
            AddRow("Position:", txtPos);
            AddRow("Salary:", txtSalary);
            AddRow("Joining Date:", txtDate);
            AddRow("Status:", cmbStatus);

            var btnSave = new Button
            {
                Text = "Save",
                Location = new Point(xBox, y + 10),
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
                catch (Exception ex) { MessageBox.Show($"Unable to save employee: {ex.Message}", "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                finally { if (!IsDisposed) btnSave.Enabled = true; }
            };
            Controls.Add(btnSave);

            if (_employee != null)
            {
                txtId.Text = _employee.EmployeeId;
                txtId.ReadOnly = true;
                txtFirst.Text = _employee.FirstName;
                txtLast.Text = _employee.LastName;
                txtEmail.Text = _employee.Email;
                txtPhone.Text = _employee.Phone;
                txtDept.Text = _employee.DepartmentId;
                txtPos.Text = _employee.Position;
                txtSalary.Text = _employee.Salary.ToString();
                txtDate.Text = _employee.JoiningDate;
                cmbStatus.SelectedItem = _employee.Status;
            }
        }

        private async Task SaveAsync()
        {
            var emp = new Employee
            {
                EmployeeId = txtId.Text.Trim(),
                FirstName = txtFirst.Text.Trim(),
                LastName = txtLast.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                DepartmentId = txtDept.Text.Trim(),
                Position = txtPos.Text.Trim(),
                Salary = double.TryParse(txtSalary.Text, out var s) ? s : 0,
                JoiningDate = txtDate.Text.Trim(),
                Status = cmbStatus.SelectedItem?.ToString() ?? "Active"
            };
            if (string.IsNullOrEmpty(emp.EmployeeId) || string.IsNullOrEmpty(emp.FirstName) || string.IsNullOrEmpty(emp.Email))
            {
                MessageBox.Show("Employee ID, First Name and Email are required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_employee != null)
                await _db.UpdateEmployeeAsync(emp.EmployeeId, emp);
            else
                await _db.InsertEmployeeAsync(emp);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
