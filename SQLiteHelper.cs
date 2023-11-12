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
            string dbFilePath = "database.db";

            if (!System.IO.File.Exists(dbFilePath))
            {
                SQLiteConnection.CreateFile(dbFilePath);
            }

            string connectionString = $"Data Source={dbFilePath};Version=3;";
            return new SQLiteConnection(connectionString);
        }

        public void CreateTables()
        {
            using (SQLiteConnection connection = CreateConnection())
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"
                                        CREATE TABLE IF NOT EXISTS service (
                                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        service_name TEXT NOT NULL
                                        );
                                        CREATE TABLE IF NOT EXISTS account (
                                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        login TEXT NOT NULL,
                                        password_hash BLOB NOT NULL,
                                        service_id INTEGER NOT NULL,
                                        FOREIGN KEY (service_id) REFERENCES service(id)
                                        );
                                        CREATE VIRTUAL TABLE mytable USING FTS5(content, service_name, login);";

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public void UpdateVirtualTableData()
        {
            using (SQLiteConnection connection = CreateConnection())
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"
                                        INSERT INTO mytable (content, service_name, login)
                                        SELECT account.id, service.service_name, account.login
                                        FROM account
                                        JOIN service ON account.service_id = service.id;";

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}
