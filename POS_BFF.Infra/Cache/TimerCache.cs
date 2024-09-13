using POS_BFF.Application.Contracts;
using POS_BFF.Core.Domain.Entities;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace POS_BFF.Infra.Cache
{
    public class TimerCache : ITimerCache
    {

        private readonly IDatabase _redisDatabase;
        private const string IdsSetKey = "service_ids";
        private const string ServiceHashPrefix = "service:";

        public TimerCache(IDatabase redisDatabase)
        {
            _redisDatabase = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
        }

        // Adiciona um novo serviço ao Redis
        public async Task AddServiceAsync(Guid serviceId, ConsumerService service)
        {
            var serviceKey = $"{ServiceHashPrefix}{serviceId}";
            var serializedService = JsonConvert.SerializeObject(service);

            await _redisDatabase.StringSetAsync(serviceKey, serializedService);
            await _redisDatabase.SetAddAsync(IdsSetKey, serviceId.ToString());
        }

        // Remove um serviço do Redis
        public async Task RemoveServiceAsync(Guid serviceId)
        {
            var serviceKey = $"{ServiceHashPrefix}{serviceId}";

            await _redisDatabase.KeyDeleteAsync(serviceKey);
            await _redisDatabase.SetRemoveAsync(IdsSetKey, serviceId.ToString());
        }

        // Recupera todos os serviços armazenados
        public async Task<Dictionary<Guid, ConsumerService>> GetTimersAsync()
        {
            var timers = new Dictionary<Guid, ConsumerService>();
            var serviceIds = await _redisDatabase.SetMembersAsync(IdsSetKey);

            foreach (var id in serviceIds)
            {
                if (Guid.TryParse(id, out var serviceId))
                {
                    var serviceKey = $"{ServiceHashPrefix}{serviceId}";
                    var serializedService = await _redisDatabase.StringGetAsync(serviceKey);

                    if (serializedService.HasValue)
                    {
                        var service = JsonConvert.DeserializeObject<ConsumerService>(serializedService);
                        if (service != null)
                        {
                            timers[serviceId] = service;
                        }
                    }
                }
            }

            return timers;
        }

        private async Task ProcessKeysAsync(IEnumerable<RedisKey> keys, Dictionary<Guid, ConsumerService> timers)
        {
            var tasks = keys.Select(async key =>
            {
                try
                {
                    var timerInfo = await _redisDatabase.StringGetAsync(key);
                    if (timerInfo.HasValue)
                    {
                        if (Guid.TryParse(key.ToString(), out var timerId))
                        {
                            var consumerService = JsonConvert.DeserializeObject<ConsumerService>(timerInfo);
                            if (consumerService != null)
                            {
                                // Adiciona ao dicionário de forma segura
                                lock (timers)
                                {
                                    timers[timerId] = consumerService;
                                }
                            }
                        }
                    }
                }
                catch (JsonException ex)
                {
                    // Log e tratamento de exceção JSON
                    Console.WriteLine($"Erro ao desserializar a chave {key}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Log e tratamento de outras exceções
                    Console.WriteLine($"Erro ao processar a chave {key}: {ex.Message}");
                }
            });

            await Task.WhenAll(tasks);
        }

        public async Task UpdateTimerAsync(Guid timerId, TimerInfo timerInfo)
        {
            var serializedTimer = JsonConvert.SerializeObject(timerInfo);
            await _redisDatabase.StringSetAsync(timerId.ToString(), serializedTimer);
        }
    }

}
