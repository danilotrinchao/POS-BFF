using POS_BFF.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace POS_BFF.Application.Backgrounds
{
    public class TimerBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<TimerBackgroundService> _logger;

        public TimerBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<TimerBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var controlTimeService = scope.ServiceProvider.GetRequiredService<IControllTimeService>();
                        await controlTimeService.CheckAndNotifyServiceTimeAsync();
                    }

                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
              }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro no TimerBackgroundService");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }
        }
        }
}
