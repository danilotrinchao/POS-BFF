using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Core.Domain.ValueObjects
{
    public class UserRole
    {
        public int UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
