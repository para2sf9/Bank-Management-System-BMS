using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BankManagementSystem
{
    public class MarkAttendanceForm : Form
    {
        private readonly DatabaseService _db;
        private ComboBox cmbEmployee;
        private TextBox txtDate;
        private ComboBox cmbStatus;
        private TextBox txtNotes;
        private Button _btnSave;

        public MarkAttendanceForm(DatabaseService db)
        {
            _db = db;
            Text = "Mark Attendance";
            Size = new Size(480, 380);
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
            int y = 20, gap = 50, xLabel = 30, xBox = 160;
            var AddRow = (string label, Control ctrl) => {
                Controls.Add(new Label { Text = label, Location = new Point(xLabel, y), AutoSize = true });
                ctrl.Location = new Point(xBox, y - 3);
                ctrl.Width = 260;
                Controls.Add(ctrl);
                y += gap;
            };

            cmbEmployee = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            txtDate = new TextBox { Text = DateTime.Now.ToString("yyyy-MM-dd") };
            cmbStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatus.Items.AddRange(new[] { "Present", "Absent", "On Leave", "Half Day" });
            cmbStatus.SelectedIndex = 0;
            txtNotes = new TextBox();

            AddRow("Employee:", cmbEmployee);
            AddRow("Date:", txtDate);
            AddRow("Status:", cmbStatus);
            AddRow("Notes:", txtNotes);

            _btnSave = new Button
            {
                Text = "Save",
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
                catch (Exception ex) { MessageBox.Show($"Unable to save attendance: {ex.Message}", "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
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
                else MessageBox.Show("Add an employee before recording attendance.", "No employees", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            var record = new AttendanceRecord
            {
                EmployeeId = empId,
                Date = txtDate.Text.Trim(),
                Status = cmbStatus.SelectedItem?.ToString() ?? "Present",
                Notes = txtNotes.Text.Trim()
            };
            await _db.MarkAttendanceAsync(record);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
