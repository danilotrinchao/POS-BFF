using POS_BFF.Core.Domain.Requests;
using POS_BFF.Core.Domain.ValueObjects;

namespace POS_BFF.Core.Domain.Gateways.Company
{
    public interface ICompanyEmployeerGateway
    {
        Task<EmployeerDTO> GetEmployeerById(Guid id, Guid TenantId);
        Task<IEnumerable<EmployeerDTO>> GetAllEmployeers( Guid TenantId);
        Task<Guid> CreateEmployeer(EmployeerDTO employeer, Guid TenantId);
        Task<bool> UpdateEmployeerAsync(EmployeerDTO employeer, Guid TenantId);
        Task<bool> DeleteEmployeer(Guid id, Guid TenantId);
        Task<EmployeerDTO> GetEmployeerByEmailAsync(string email, Guid TenantId);
        Task<EmployeerDTO> GetEmployeerByCPFAsync(string cpf, Guid TenantId);
        Task<List<UserRole>> GetEmployeerRolesById(Guid id, Guid TenantId);
    }
}
