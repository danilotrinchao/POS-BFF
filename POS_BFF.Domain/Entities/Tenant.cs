using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Core.Domain.Entities
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string ConnectionString { get; set; }
        public string Schema { get; set; }
        public string CompanyName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
