using password_manager.Entities;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace password_manager.Repositrories
{
    class ServiceRepository
    {
        SQLiteHelper dbHelper = new SQLiteHelper();
        
        public void AddService(string serviceName)
        {
            using (SQLiteConnection connection = dbHelper.CreateConnection())
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Connection = connection;
                    command.CommandText = $"INSERT INTO service (service_name) VALUES ('{serviceName}');";
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public bool CheckServiceExists(string serviceName)
        {
            bool exists = false;

            using (SQLiteConnection connection = dbHelper.CreateConnection())
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Connection = connection;
                    command.CommandText = $"SELECT COUNT(*) FROM service WHERE service_name = '{serviceName}';";

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            exists = reader.GetInt32(0) > 0;
                        }
                    }
                }

                connection.Close();
            }

            return exists;
        }

        public List<Service> GetServices()
        {
            List<Service> services = new List<Service>();

            using (SQLiteConnection connection = dbHelper.CreateConnection())
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT * FROM service";

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            services.Add(new Service(reader.GetInt64(0), reader.GetString(1)));
                        }
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
                    command.CommandText = $"UPDATE service SET service_name = '{service.ServiceName}' WHERE id = {service.Id};";
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}
