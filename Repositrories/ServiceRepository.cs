using password_manager.Entities;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace password_manager.Repositrories
{
    public class ServiceRepository
    {
        SQLiteHelper dbHelper = new SQLiteHelper();

        public void CreateTables()
        {
            using (SQLiteConnection connection = dbHelper.CreateConnection())
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"CREATE TABLE IF NOT EXISTS service (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            service_name TEXT NOT NULL,
                                            login TEXT NOT NULL,
                                            password_hash BLOB NOT NULL
                                            );";

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public void AddService(Service service)
        {

            using (SQLiteConnection connection = dbHelper.CreateConnection())
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "INSERT INTO service(service_name, login, password_hash) VALUES(@service_name, @login, @password_hash)";
                    command.Parameters.AddWithValue("@service_name", service.ServiceName);
                    command.Parameters.AddWithValue("@login", service.Login);
                    command.Parameters.AddWithValue("@password_hash", service.GetPasswordHash());

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public List<Service> GetAllServices()
        {
            List<Service> services = new List<Service>();

            using (SQLiteConnection connection = dbHelper.CreateConnection())
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM service", connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        long id = Convert.ToInt64(reader["id"]);
                        string serviceName = (string)reader["service_name"];
                        string login = (string)reader["login"];
                        byte[] passHash = (byte[])reader["password_hash"];
                        services.Add(new Service(id, serviceName, login, passHash));
                    }
                }
                connection.Close();
            }

            return services;
        }

        public void UpdateService(Service service)
        {
            using (SQLiteConnection connection = dbHelper.CreateConnection())
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "UPDATE service SET service_name = @service_name, login = @login, password_hash = @password_hash WHERE id = @id";
                    command.Parameters.AddWithValue("@id", service.Id);
                    command.Parameters.AddWithValue("@service_name", service.ServiceName);
                    command.Parameters.AddWithValue("@login", service.Login);
                    command.Parameters.AddWithValue("@password_hash", service.GetPasswordHash());

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}
