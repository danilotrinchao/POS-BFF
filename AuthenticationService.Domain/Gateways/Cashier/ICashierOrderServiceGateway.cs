using AuthenticationService.Core.Domain.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Core.Domain.Gateways.Cashier
{
    public interface ICashierOrderServiceGateway
    {
        Task<OrderDto> GetOrderByIdAsync(Guid id);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<Guid> CreateOrderAsync(SaleDTO saleDto);
        Task<bool> UpdateOrderAsync(Guid id, SaleDTO saleDto);
        Task<bool> DeleteOrderAsync(Guid id);
        Task<bool> OpenCashier(decimal InitialBalance, int EmployeerId);
        Task<bool> CloseCashier(Guid CashierId);
    }

}
