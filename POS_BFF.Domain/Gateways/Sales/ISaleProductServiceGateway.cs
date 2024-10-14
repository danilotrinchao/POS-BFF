using POS_BFF.Core.Domain.Enums;
using POS_BFF.Core.Domain.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Core.Domain.Gateways.Sales
{
    public interface ISaleProductServiceGateway
    {
        Task<Guid> AddProductAsync(PhysiqueProductDTO productDto, Guid TenantId);
        Task<Guid> AddServicetAsync(VirtualProductDTO productDto, Guid TenantId);
        Task<bool> UpdateProductAsync(Guid id, PhysiqueProductDTO productDto, Guid TenantId);
        Task<bool> UpdateServiceAsync(Guid id, VirtualProductDTO serviceDto, Guid TenantId);
        Task<bool> DeleteProductAsync(Guid id, Guid TenantId);
        Task<bool> DeleteServiceAsync(Guid id, Guid TenantId);
        Task<IEnumerable<VirtualProductDTO>> GetAllServicesAsync( Guid TenantId);
        Task<IEnumerable<PhysiqueProductDTO>> GetAllProductsAsync( Guid TenantId);
        Task<VirtualProductDTO> GetVirtualProductAsync(Guid serviceid, Guid TenantId);
        Task<PhysiqueProductDTO> GetPhysiqueProductAsync(Guid productid, Guid TenantId);
        Task<VirtualProductDTO> GetServiceById(Guid id, Guid TenantId);
        Task<PhysiqueProductDTO> GetProductById(Guid id, Guid TenantId);
        Task CheckAndNotifyStockAsync( Guid TenantId);
        Task<List<string>> GetNotifyStockAsync(Guid TenantId);
        Task<PhysiqueProductDTO> GetProductByBarCode(string barcode, Guid TenantId);
    }
}
