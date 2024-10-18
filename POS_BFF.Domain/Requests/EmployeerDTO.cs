using POS_BFF.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace POS_BFF.Core.Domain.Requests
{
    public class EmployeerDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Email { get; set; }
        public string CPF { get; set; }
        public string Phone { get; set; }
        public EUserType UserType { get; set; }
        public string PasswordHash { get; set; }
        public List<Guid> RoleIds { get; set; }
        public Guid StoreId { get; set; }
        public StoreDTO Store { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool Inative { get; set; }
    }
}
