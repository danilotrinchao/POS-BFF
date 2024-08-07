using AuthenticationService.Core.Domain.Entities;
using AuthenticationService.Domain.Entities;

namespace AuthenticationService.Application.Contracts
{
    public interface IConsumerService
    {
        Task<IEnumerable<ConsumerService>> GetServicesByUserId(int userId);
        Task<bool> UpdateConsumerService(Guid consumerServiceId, int totalTime);
        Task<Guid> CreateConsumerService(ConsumerService consumer);


    }
}
