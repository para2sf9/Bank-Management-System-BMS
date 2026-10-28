using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BankManagementSystem
{
    public class GeneratePayrollForm : Form
    {
        private readonly DatabaseService _db;
        private ComboBox cmbEmployee;
        private ComboBox cmbMonth;
        private TextBox txtYear;
        private TextBox txtBase;
        private TextBox txtBonus;
        private TextBox txtDeductions;
        private Button _btnSave;

        public GeneratePayrollForm(DatabaseService db)
        {
            _db = db;
            Text = "Generate Payroll";
            Size = new Size(480, 450);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;
            Initialize();
            Shown += async (s, e) => await LoadEmployeesAsync();
        }

        private void Initialize()
        {
            int y = 20, gap = 45, xLabel = 30, xBox = 160;
            var AddRow = (string label, Control ctrl) => {
                Controls.Add(new Label { Text = label, Location = new Point(xLabel, y), AutoSize = true });
                ctrl.Location = new Point(xBox, y - 3);
                ctrl.Width = 260;
                Controls.Add(ctrl);
                y += gap;
            };

            cmbEmployee = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMonth = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            for (int i = 1; i <= 12; i++) cmbMonth.Items.Add(i.ToString());
            cmbMonth.SelectedIndex = 0;

            txtYear = new TextBox { Text = "2024" };
            txtBase = new TextBox();
            txtBonus = new TextBox { Text = "0" };
            txtDeductions = new TextBox { Text = "0" };

            AddRow("Employee:", cmbEmployee);
            AddRow("Month:", cmbMonth);
            AddRow("Year:", txtYear);
            AddRow("Base Salary:", txtBase);
            AddRow("Bonus:", txtBonus);
            AddRow("Deductions:", txtDeductions);

            _btnSave = new Button
            {
                Text = "Generate",
                Location = new Point(xBox, y + 5),
                Width = 100,
                Height = 35,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnSave.FlatAppearance.BorderSize = 0;
            _btnSave.Enabled = false;
            _btnSave.Click += async (s, e) =>
            {
                _btnSave.Enabled = false;
                try { await SaveAsync(); }
                catch (Exception ex) { MessageBox.Show($"Unable to generate payroll: {ex.Message}", "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                finally { if (!IsDisposed) _btnSave.Enabled = true; }
            };
            Controls.Add(_btnSave);
        }

        private async Task LoadEmployeesAsync()
        {
            try
            {
                var emps = await _db.GetEmployeesAsync();
                cmbEmployee.Items.Clear();
                foreach (var employee in emps)
                    cmbEmployee.Items.Add($"{employee.EmployeeId} - {employee.FirstName} {employee.LastName}");
                if (cmbEmployee.Items.Count > 0) cmbEmployee.SelectedIndex = 0;
                else MessageBox.Show("Add an employee before generating payroll.", "No employees", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to load employees: {ex.Message}", "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _btnSave.Enabled = cmbEmployee.Items.Count > 0;
            }
        }

        private async Task SaveAsync()
        {
            if (cmbEmployee.SelectedItem == null)
            {
                MessageBox.Show("Select an employee.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var empId = cmbEmployee.SelectedItem.ToString().Split(" - ")[0];
            var record = new PayrollRecord
            {
                EmployeeId = empId,
                Month = int.Parse(cmbMonth.SelectedItem.ToString()),
                Year = int.Parse(txtYear.Text.Trim()),
                BaseSalary = double.TryParse(txtBase.Text, out var b) ? b : 0,
                Bonus = double.TryParse(txtBonus.Text, out var bo) ? bo : 0,
                Deductions = double.TryParse(txtDeductions.Text, out var d) ? d : 0
            };
            await _db.GeneratePayrollAsync(record);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
