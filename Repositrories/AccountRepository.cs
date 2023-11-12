using password_manager.Entities;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace password_manager.Repositrories
{
    public class AccountRepository
    {
        SQLiteHelper dbHelper = new SQLiteHelper();


        public void AddAccount(string login, long serviceId, string password)
        {

            using (SQLiteConnection connection = dbHelper.CreateConnection())
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "INSERT INTO account (login, password_hash, service_id) VALUES (@login, @password_hash, @service_id)";
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@password_hash", password);
                    command.Parameters.AddWithValue("@service_id", serviceId);

                    command.ExecuteNonQuery();
                }
                    
                connection.Close();
            }
        }

        public List<Account> GetAllServices()
        {

            List<Account> services = new List<Account>();

            using (SQLiteConnection connection = dbHelper.CreateConnection())
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM account JOIN service ON account.service_id = service.id", connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        long id = Convert.ToInt64(reader["id"]);
                        string serviceName = (string)reader["service_name"];
                        string login = (string)reader["login"];
                        byte[] passHash = (byte[])reader["password_hash"];
                        services.Add(new Account(id, serviceName, login, passHash));
                    }
                }
                connection.Close();
            }

            return services;
        }

        public void UpdateService(string login, string password, long serviceId, long accountId)
        {
            using (SQLiteConnection connection = dbHelper.CreateConnection())
            {
                connection.Open();

                //rewrite it using adding id of service

                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "UPDATE account SET service_id = @service_id, login = @login, password_hash = @password_hash WHERE id = @id";
                    command.Parameters.AddWithValue("@id", accountId);
                    command.Parameters.AddWithValue("@service_id", serviceId);
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@password_hash", password);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}
