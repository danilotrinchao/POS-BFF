using POS_BFF.Core.Domain.Requets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Core.Domain.Gateways.Sales
{
    public interface ISaleClientServiceGateway
    {
        Task<Guid> CreateClientAsync(ClientRequest client, Guid TenantId);
        Task<ClientRequest> GetClientByIdAsync(Guid id, Guid TenantId);
        Task<IEnumerable<ClientRequest>> GetAllClientsAsync(Guid TenantId);
        Task<bool> UpdateClientAsync(ClientRequest client, Guid TenantId);
        Task<bool> DeleteClientAsync(Guid id, Guid TenantId);
    }
}
