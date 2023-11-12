using password_manager.Repositrories;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace password_manager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        SQLiteHelper dbHelper = new SQLiteHelper();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            dbHelper.CreateTables();
        }
    }
}
