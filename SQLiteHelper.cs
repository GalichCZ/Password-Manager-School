using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace password_manager
{
    public class SQLiteHelper
    {
        public SQLiteConnection CreateConnection()
        {
            string dbFilePath = "YourDatabaseFile.db"; // The path to your SQLite database file

            if (!System.IO.File.Exists(dbFilePath))
            {
                SQLiteConnection.CreateFile(dbFilePath); // Create the database file if it doesn't exist
            }

            string connectionString = $"Data Source={dbFilePath};Version=3;";
            return new SQLiteConnection(connectionString);
        }
    }
}
