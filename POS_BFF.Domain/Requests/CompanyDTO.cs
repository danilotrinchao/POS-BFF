namespace POS_BFF.Core.Domain.Requests
{
    public class CompanyDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string CNPJ { get; set; }
        public Guid TenantId { get; set; } // Associação ao Tenant
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public IList<StoreDTO> Stores { get; set; }
    }
}
