using POS_BFF.Core.Domain.Enums;
using POS_BFF.Core.Domain.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Core.Domain.Gateways.Cashier
{
    public interface ICashierOrderServiceGateway
    {
        Task<Guid> OpenCashier(decimal InitialBalance, int EmployeerId, Guid TentantId);
        Task<bool> CloseCashier(int employeerId, Dictionary<EPaymentType, decimal> totals, Guid TentantId);
        Task<bool> GetOpenedCashier( Guid TentantId);
        Task<bool> GetOpenedCashierByEmployeerId(int employeerId, Guid TentantId);
    }

}
