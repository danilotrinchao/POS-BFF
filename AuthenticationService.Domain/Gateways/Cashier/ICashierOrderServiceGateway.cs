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
        Task<bool> CloseCashier(Guid CashierId);
    }

}
