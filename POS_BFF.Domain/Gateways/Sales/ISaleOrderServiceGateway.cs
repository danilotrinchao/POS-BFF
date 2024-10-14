using POS_BFF.Core.Domain.Enums;
using POS_BFF.Core.Domain.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Core.Domain.Gateways.Sales
{
    public interface ISaleOrderServiceGateway
    {
        Task<Guid> CreateSaleAsync(SaleDTO saleDto, Guid TenantId);
        Task<SaleDTO> GetSaleByIdAsync(Guid id, Guid TenantId);
        Task<IEnumerable<SaleDTO>> GetAllSalesAsync(Guid TenantId);
        Task<bool> CompleteSaleAsync(Guid id, SaleDTO saleDTO, Guid TenantId);
        Task<bool> CancelSaleAsync(Guid id, Guid TenantId);
        Task<Dictionary<EPaymentType, decimal>> GetDailyTotals(Guid TenantId);
    }
}
