using password_manager.Entities;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;

namespace password_manager.Repositrories
{
    public class AccountRepository
    {
        SQLiteHelper dbHelper = new SQLiteHelper();

        public Account GetAccountById(long id)
        {
            Account account = null;
            using (SQLiteConnection connection = dbHelper.CreateConnection())
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM account JOIN service ON account.service_id = service.id WHERE account.id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string serviceName = (string)reader["service_name"];
                                string login = (string)reader["login"];
                                byte[] passHash = (byte[])reader["password_hash"];
                                account = new Account(id, serviceName, login, passHash);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No matching records found.");
                        }
                    }
                }

                connection.Close();
            }

            return account;
        }

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

            List<Account> account = new List<Account>();

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
                        account.Add(new Account(id, serviceName, login, passHash));
                    }
                }
                connection.Close();
            }

            return account;
        }

        public void UpdateService(string login, string password, long serviceId, long accountId)
        {
            using (SQLiteConnection connection = dbHelper.CreateConnection())
            {
                connection.Open();

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

        public List<Account> SearchThrough(string searchTerm)
        {
            List<Account> services = new List<Account>();

            using (SQLiteConnection connection = dbHelper.CreateConnection())
            {
                connection.Open();

                string query = "SELECT account.*, service.service_name " +
                           "FROM service " +
                           "INNER JOIN account ON service.id = account.service_id " +
                           "WHERE service.service_name LIKE '%' || @searchTerm || '%' OR account.login LIKE '%' || @searchTerm || '%'";

                using (SQLiteCommand command = new SQLiteCommand(query, connection)) 
                { 
                    command.Parameters.AddWithValue("@searchTerm", searchTerm);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                long id = Convert.ToInt64(reader["id"]);
                                string serviceName = (string)reader["service_name"];
                                string login = (string)reader["login"];
                                byte[] passHash = (byte[])reader["password_hash"];
                                Account account = new Account(id, serviceName, login, passHash);
                                services.Add(account);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No matching records found.");
                        }
                    }
                }

                connection.Close();
            }

            return services;
        }

        public void DeleteAccount(long id)
        {
            using (SQLiteConnection connection = dbHelper.CreateConnection())
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "DELETE FROM account WHERE id = @id";
                    command.Parameters.AddWithValue("@id", id);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}
