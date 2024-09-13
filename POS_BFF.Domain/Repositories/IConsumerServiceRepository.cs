using POS_BFF.Core.Domain.Entities;

namespace POS_BFF.Core.Domain.Repositories
{
    public interface IConsumerServiceRepository
    {
        Task<Guid> CreateConsumerService(ConsumerService consumer);
        Task<IEnumerable<ConsumerService>> GetConsumerServiceByUserId(int userId);
        Task<IEnumerable<ConsumerService>> GetAllConsumerService();
        Task<bool> UpdateConsumerService(ConsumerService consumer);
        Task<bool> DeleteConsumerService(Guid consumerId);
        Task<ConsumerService> GetConsumerServiceById(Guid id);
        Task<IEnumerable<ConsumerService>> GetActiveConsumerServicesAsync();
        Task UpdateElapsedTimeAsync(Guid consumerId);
    }
}
