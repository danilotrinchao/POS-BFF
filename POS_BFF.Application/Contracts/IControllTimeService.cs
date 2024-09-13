using POS_BFF.Core.Domain.Entities;
using POS_BFF.Domain.Entities;

namespace POS_BFF.Application.Contracts
{
    public interface IControllTimeService
    {
        Task<IEnumerable<ConsumerService>> GetServicesByUserId(int userId);
        Task<bool> UpdateConsumerService(Guid consumerServiceId, int totalTime);
        Task<Guid> CreateConsumerService(ConsumerService consumer);
        Task CheckAndNotifyServiceTimeAsync();
        Task<IEnumerable<ConsumerService>> GetActiveConsumerServicesAsync();
        Task<List<string>> GetNotifyServiceAsync();


    }
}
