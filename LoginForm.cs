using System;
using System.Drawing;
using System.Windows.Forms;

namespace BankManagementSystem
{
    public class LoginForm : Form
    {
        private readonly DatabaseService _db;
        public UserInfo? CurrentUser { get; private set; }

        public LoginForm(DatabaseService db)
        {
            _db = db;
            Text = "Bank Management System - Login";
            Size = new Size(480, 390);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(245, 247, 251);
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 105,
                BackColor = Color.FromArgb(31, 78, 121)
            };

            var title = new Label
            {
                Text = "Bank Management System",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(28, 22)
            };
            header.Controls.Add(title);

            var subtitle = new Label
            {
                Text = "Secure employee and payroll management",
                ForeColor = Color.FromArgb(220, 235, 248),
                AutoSize = true,
                Location = new Point(30, 62)
            };
            header.Controls.Add(subtitle);

            var lblUser = new Label { Text = "Username", Location = new Point(55, 135), AutoSize = true };
            var txtUser = new TextBox { Location = new Point(55, 155), Width = 350, Height = 30 };

            var lblPass = new Label { Text = "Password", Location = new Point(55, 198), AutoSize = true };
            var txtPass = new TextBox { Location = new Point(55, 218), Width = 350, Height = 30, PasswordChar = '*' };

            var btnLogin = new Button
            {
                Text = "Login",
                Location = new Point(55, 270),
                Width = 350,
                Height = 35,
                BackColor = Color.FromArgb(31, 78, 121),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogin.FlatAppearance.BorderSize = 0;

            var lblStatus = new Label
            {
                Text = _db.Connected ? "Connected to MongoDB" : $"MongoDB unavailable: {_db.ConnectionError}",
                ForeColor = _db.Connected ? Color.FromArgb(32, 120, 75) : Color.Firebrick,
                AutoSize = true,
                Location = new Point(55, 325),
                MaximumSize = new Size(350, 0)
            };
            btnLogin.Enabled = _db.Connected;

            btnLogin.Click += (s, e) =>
            {
                var user = _db.Authenticate(txtUser.Text.Trim(), txtPass.Text.Trim());
                if (user != null)
                {
                    CurrentUser = user;
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show(_db.Connected ? "Invalid username or password." : "MongoDB is not available. Start the local MongoDB service and try again.", "Sign-in failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            txtPass.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnLogin.PerformClick(); };

            Controls.Add(header);
            Controls.Add(lblUser);
            Controls.Add(txtUser);
            Controls.Add(lblPass);
            Controls.Add(txtPass);
            Controls.Add(btnLogin);
            Controls.Add(lblStatus);
        }
    }
}
