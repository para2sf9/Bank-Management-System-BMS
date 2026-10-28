using System;
using System.Drawing;
using System.Windows.Forms;

namespace BankManagementSystem
{
    public class DashboardPanel : Panel
    {
        public DashboardPanel(int employees, int departments, int present)
        {
            BackColor = UiTheme.Page;
            AutoScroll = true;
            Padding = new Padding(20);

            var lblTitle = new Label
            {
                Text = "Dashboard",
                ForeColor = UiTheme.Navy,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            Controls.Add(lblTitle);

            Controls.Add(new Label
            {
                Text = "Overview of your workforce and daily operations",
                ForeColor = Color.FromArgb(100, 116, 139),
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(22, 57)
            });

            var cards = new (string title, int value)[]
            {
                ("Total Employees", employees),
                ("Total Departments", departments),
                ("Present Today", present)
            };

            int x = 20;
            foreach (var card in cards)
            {
                var cardPanel = new Panel
                {
                    Size = new Size(235, 135),
                    Location = new Point(x, 90),
                    BackColor = Color.White
                };
                cardPanel.Paint += (s, e) => e.Graphics.FillRectangle(new SolidBrush(card.title == "Present Today" ? Color.FromArgb(39, 151, 110) : UiTheme.Blue), 0, 0, 5, cardPanel.Height);

                var lblValue = new Label
                {
                    Text = card.value.ToString(),
                    ForeColor = UiTheme.Navy,
                    Font = new Font("Segoe UI", 28, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(22, 20)
                };

                var lblText = new Label
                {
                    Text = card.title,
                    Font = new Font("Segoe UI", 11),
                    AutoSize = true,
                    Location = new Point(22, 82)
                };

                cardPanel.Controls.Add(lblValue);
                cardPanel.Controls.Add(lblText);
                Controls.Add(cardPanel);
                x += 255;
            }

            var lblInfo = new Label
            {
                Text = "Quick access\nUse the navigation menu to manage employees, departments, attendance, and payroll.",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true,
                Location = new Point(20, 260)
            };
            Controls.Add(lblInfo);
        }
    }
}
