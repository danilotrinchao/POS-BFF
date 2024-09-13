using POS_BFF.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Application.Contracts
{
    public interface ITimerCache
    {
        Task<Dictionary<Guid, ConsumerService>> GetTimersAsync();
        Task UpdateTimerAsync(Guid timerId, TimerInfo timerInfo);
    }

}
