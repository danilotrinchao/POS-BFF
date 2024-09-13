using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Core.Domain.Gateways.Sales
{
    public interface ISalesServiceUsageGateway
    {
        Task StartServiceAsync(Guid orderItemId);
        Task PauseServiceAsync(Guid orderItemId);
        Task StopServiceAsync(Guid orderItemId);
        Task<dynamic> GetServiceUsageByOrderItemIdAsync(Guid orderItemId);
    }
}
