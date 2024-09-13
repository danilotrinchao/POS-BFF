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
        Task<int> CreateClientAsync(ClientRequest client);
        Task<ClientRequest> GetClientByIdAsync(int id);
        Task<IEnumerable<ClientRequest>> GetAllClientsAsync();
        Task<bool> UpdateClientAsync(ClientRequest client);
        Task<bool> DeleteClientAsync(int id);
    }
}
