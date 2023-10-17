using Npgsql;
using password_manager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace password_manager
{
    public class DBConnection
    {
        static readonly string dbConnect = "Host=localhost;Port=5432;Username=postgres;Password=admin;Database=password_manager";

        public static Service RetrieveData()
        {
            Service data = null;
            using(NpgsqlConnection connection = new NpgsqlConnection(dbConnect)) 
            {
                connection.Open();
                string sql = "SELECT * FROM public.service";

                using(NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                using(NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        long id = Convert.ToInt64(reader["id"]);
                        string serviceName = (string)reader["service_name"];
                        string login = (string)reader["login"];
                        byte[] passHash = (byte[])reader["password_hash"];

                        data = new Service(id, serviceName, login, passHash);
                        Console.WriteLine(reader.ToString());
                    }
                }

                connection.Close();
            }
            return data;
        }
    }
}
