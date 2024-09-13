using POS_BFF.Domain.Enums;
using POS_BFF.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Permission> Permissions { get; set; }
        public EUserType Group { get; set; }
        public bool Inative { get; set; }
    }
}
