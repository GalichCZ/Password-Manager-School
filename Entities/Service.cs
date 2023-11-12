using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace password_manager.Entities
{
    class Service
    {
        public long Id { get; set; }
        public string ServiceName { get; set; }

        public Service(long id, string serviceName)
        {
            Id = id;
            ServiceName = serviceName;
        }
    }
}
