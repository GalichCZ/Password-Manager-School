using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace password_manager.Entities
{
    public class Account
    {
        public long Id { get; set; }
        public string ServiceName { get; set; }
        public string Login { get; set; }
        private byte[] PasswordHash { get; set; }

        public string PasswordHashString
        {
            get => Encoding.UTF8.GetString(PasswordHash);
            set => PasswordHash = Encoding.UTF8.GetBytes(value);
        }

        public byte[] GetPasswordHash()
        {
            return PasswordHash;
        }

        public Account(long id, string serviceName, string login, string passwordHash)
        {
            Id = id;
            ServiceName = serviceName;
            Login = login;
            PasswordHashString = passwordHash;
        }

        public Account(long id, string serviceName, string login, byte[] passwordHash)
        {
            Id = id;
            ServiceName = serviceName;
            Login = login;
            PasswordHash = passwordHash;
        }

        public Account(string serviceName, string login, string passwordHash)
        {
            ServiceName = serviceName;
            Login = login;
            PasswordHashString = passwordHash;
        }

        public override string ToString()
        {
            return Id.ToString() + " " + ServiceName.ToString() + " " + Login.ToString() + " " + PasswordHashString;
        }
    }
}
