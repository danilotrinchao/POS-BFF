using AuthenticationService.Application.Contracts;
using AuthenticationService.Core.Domain.Entities;
using AuthenticationService.Core.Domain.Repositories;

namespace AuthenticationService.Application.Services
{
    public class ControlTimeService: IConsumerService
    {
        public readonly IConsumerServiceRepository _consumerServiceRepository;
        public ControlTimeService(IConsumerServiceRepository consumerServiceRepository)
        {
                _consumerServiceRepository = consumerServiceRepository;
        }

        public Task<IEnumerable<ConsumerService>> GetServicesByUserId(int userId)
        {
          var result = _consumerServiceRepository.GetConsumerServiceByUserId(userId);
          if(result == null)
            throw new NotImplementedException();

          return result;
        }

        public async Task<bool> UpdateConsumerService(Guid consumerServiceId, int totalTime)
        {
            var consumer = await _consumerServiceRepository.GetConsumerServiceById(consumerServiceId);
            consumer.totalTime = totalTime;
            if(totalTime == 0)
                consumer.is_Active = false;
            return await _consumerServiceRepository.UpdateConsumerService(consumer);
        }

        public async Task<Guid> CreateConsumerService(ConsumerService consumer)
        {
            return await _consumerServiceRepository.CreateConsumerService(consumer);
        }
    }
}