using System;
using System.Drawing;
using System.Windows.Forms;

namespace BankManagementSystem
{
    public class MainForm : Form
    {
        private readonly DatabaseService _db;
        private readonly UserInfo _user;
        private Panel _contentPanel;

        public MainForm(DatabaseService db, UserInfo user)
        {
            _db = db;
            _user = user;
            Text = "Bank Management System";
            Size = new Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1000, 650);
            InitializeComponents();
            ShowDashboard();
        }

        private void InitializeComponents()
        {
            var sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = UiTheme.Navy
            };

            var lblTitle = new Label
            {
                Text = "BANK MANAGEMENT",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            sidebar.Controls.Add(lblTitle);

            string[] buttons = { "Dashboard", "Employees", "Departments", "Attendance", "Payroll", "Logout" };
            int y = 70;
            foreach (var btnText in buttons)
            {
                var btn = new Button
                {
                    Text = btnText,
                    Size = new Size(180, 40),
                    Location = new Point(20, y),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = UiTheme.Navy,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10),
                    Cursor = Cursors.Hand
                };
                UiTheme.StyleButton(btn, btnText == "Dashboard");
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Padding = new Padding(18, 0, 0, 0);
                var local = btnText;
                btn.Click += (s, e) => HandleSidebarClick(local);
                sidebar.Controls.Add(btn);
                y += 50;
            }

            var lblUser = new Label
            {
                Text = $"User: {_user.Username}\nRole: {_user.Role}",
                ForeColor = Color.FromArgb(190, 210, 230),
                AutoSize = true,
                Location = new Point(20, y + 20)
            };
            sidebar.Controls.Add(lblUser);

            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 246, 250)
            };

            Controls.Add(_contentPanel);
            Controls.Add(sidebar);
        }

        private void HandleSidebarClick(string option)
        {
            if (option == "Logout")
            {
                Application.Exit();
                return;
            }
            _contentPanel.Controls.Clear();
            switch (option)
            {
                case "Dashboard": ShowDashboard(); break;
                case "Employees": ShowEmployees(); break;
                case "Departments": ShowDepartments(); break;
                case "Attendance": ShowAttendance(); break;
                case "Payroll": ShowPayroll(); break;
            }
        }

        private void ShowDashboard()
        {
            var stats = _db.GetStats();
            var panel = new DashboardPanel(stats.employees, stats.departments, stats.presentToday);
            panel.Dock = DockStyle.Fill;
            _contentPanel.Controls.Add(panel);
        }

        private void ShowEmployees()
        {
            var panel = new EmployeesPanel(_db, _user.Role);
            panel.Dock = DockStyle.Fill;
            _contentPanel.Controls.Add(panel);
        }

        private void ShowDepartments()
        {
            var panel = new DepartmentsPanel(_db, _user.Role);
            panel.Dock = DockStyle.Fill;
            _contentPanel.Controls.Add(panel);
        }

        private void ShowAttendance()
        {
            var panel = new AttendancePanel(_db, _user.Role);
            panel.Dock = DockStyle.Fill;
            _contentPanel.Controls.Add(panel);
        }

        private void ShowPayroll()
        {
            var panel = new PayrollPanel(_db, _user.Role);
            panel.Dock = DockStyle.Fill;
            _contentPanel.Controls.Add(panel);
        }
    }
}
