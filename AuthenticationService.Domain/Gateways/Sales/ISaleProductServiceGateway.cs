using AuthenticationService.Core.Domain.Enums;
using AuthenticationService.Core.Domain.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Core.Domain.Gateways.Sales
{
    public interface ISaleProductServiceGateway
    {
        Task<Guid> AddProductAsync(PhysiqueProductDTO productDto);
        Task<Guid> AddServicetAsync(VirtualProductDTO productDto);
        Task<bool> UpdateProductAsync(Guid id, PhysiqueProductDTO productDto);
        Task<bool> UpdateServiceAsync(Guid id, VirtualProductDTO serviceDto);
        Task<bool> DeleteProductAsync(Guid id);
        Task<bool> DeleteServiceAsync(Guid id);
        Task<IEnumerable<VirtualProductDTO>> GetAllServicesAsync();
        Task<IEnumerable<PhysiqueProductDTO>> GetAllProductsAsync();
        Task<VirtualProductDTO> GetVirtualProductAsync(Guid serviceid);
        Task<PhysiqueProductDTO> GetPhysiqueProductAsync(Guid productid);
        Task<VirtualProductDTO> GetServiceById(Guid id);
        Task<PhysiqueProductDTO> GetProductById(Guid id);
    }
}
