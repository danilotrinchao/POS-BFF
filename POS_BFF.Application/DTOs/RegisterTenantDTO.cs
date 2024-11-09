using POS_BFF.Core.Domain.Enums;

namespace POS_BFF.Application.DTOs
{
    public class RegisterTenantDTO
    {
        public Guid TenantId { get; set; }
        public string AdminEmail { get; set; }
        public string Password { get; set; }
        public EDatabaseType DatabaseType { get; set; }

    }
}
