using AuthenticationService.Application.Contracts;
using AuthenticationService.Core.Domain.Entities;
using AuthenticationService.Core.Domain.Interfaces;
using AuthenticationService.Core.Domain.Repositories;

namespace AuthenticationService.Application.Services
{
    public class ControlTimeService: IControllTimeService
    {
        private readonly IConsumerServiceRepository _consumerServiceRepository;
        private readonly INotificationPublisher _notificationPublisher;
        private readonly ITimerCache _timerCache;
        private readonly HashSet<string> _notifiedServices = new HashSet<string>();
        public ControlTimeService(IConsumerServiceRepository consumerServiceRepository,
                                  INotificationPublisher notificationPublisher,
                                  ITimerCache timerCache)
        {
            _consumerServiceRepository = consumerServiceRepository;
            _notificationPublisher = notificationPublisher;
            _timerCache = timerCache;
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

            bool isUpdated = await _consumerServiceRepository.UpdateConsumerService(consumer);
            if (isUpdated)
            {
                // Atualizar o cache Redis
                var timerInfo = new TimerInfo
                {
                    TimerId = consumerServiceId,
                    StartTime = consumer.StartTime.Minute, // Certifique-se de que StartTime é corretamente atribuído
                    TotalTime = totalTime,
                    IsRunning = consumer.is_Active
                };

                await _timerCache.UpdateTimerAsync(consumerServiceId, timerInfo);
            }

            return isUpdated;
        }

        public async Task<Guid> CreateConsumerService(ConsumerService consumer)
        {
            return await _consumerServiceRepository.CreateConsumerService(consumer);
        }

        public async Task CheckAndNotifyServiceTimeAsync()
        {
            var activeTimers = await _timerCache.GetTimersAsync();
            foreach (var timerInfo in activeTimers.Values)
            {
                if (timerInfo.is_Active && timerInfo.totalTime <= 3) // 3 minutos ou menos
                {
                    var message = $"Serviço '{timerInfo.serviceName}' para o usuário {timerInfo.userId} está prestes a expirar.";
                    await _notificationPublisher.PublishAsync(message);
                }
            }
        }
        public async Task<List<string>> GetNotifyServiceAsync()
        {
            var notifications = new List<string>();

            var activeTimers = await _timerCache.GetTimersAsync();

            foreach (var product in activeTimers.Values)
            {
                if (_notifiedServices.Contains(product.serviceName))
                    continue; // Ignora produtos já notificados
                if (product.is_Active && product.totalTime <= 2) // 3 minutos ou menos
                {
                    var message = $"Serviço '{product.serviceName}' para o usuário {product.userId} está prestes a expirar.";
                    notifications.Add(message); // Adiciona a mensagem à lista de notificações
                    _notifiedServices.Add(product.serviceName); // Marca o produto como notificado
                }
            }

            return notifications; // Retorna a lista de notificações
        }
        public async Task<IEnumerable<ConsumerService>> GetActiveConsumerServicesAsync()
        {
            // Adicione aqui qualquer lógica adicional necessária antes de chamar o repositório
            return await _consumerServiceRepository.GetActiveConsumerServicesAsync();
        }
    }
}