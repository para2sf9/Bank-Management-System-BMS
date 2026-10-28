using System;
using System.Windows.Forms;

namespace BankManagementSystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var db = new DatabaseService();
            using (var login = new LoginForm(db))
            {
                if (login.ShowDialog() == DialogResult.OK && login.CurrentUser != null)
                {
                    Application.Run(new MainForm(db, login.CurrentUser));
                }
            }
        }
    }
}
