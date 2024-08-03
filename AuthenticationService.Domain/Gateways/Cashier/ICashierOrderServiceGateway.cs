using AuthenticationService.Core.Domain.Enums;
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
        Task<Guid> OpenCashier(decimal InitialBalance, int EmployeerId);
        Task<bool> CloseCashier(int employeerId, Dictionary<EPaymentType, decimal> totals);
        Task<bool> GetOpenedCashier();
        Task<bool> GetOpenedCashierByEmployeerId(int employeerId);
    }

}
