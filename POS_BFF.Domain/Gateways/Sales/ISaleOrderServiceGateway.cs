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
        Task<Guid> CreateSaleAsync(SaleDTO saleDto);
        Task<SaleDTO> GetSaleByIdAsync(Guid id);
        Task<IEnumerable<SaleDTO>> GetAllSalesAsync();
        Task<bool> CompleteSaleAsync(Guid id, SaleDTO saleDTO);
        Task<bool> CancelSaleAsync(Guid id);
        Task<Dictionary<EPaymentType, decimal>> GetDailyTotals();
    }
}
