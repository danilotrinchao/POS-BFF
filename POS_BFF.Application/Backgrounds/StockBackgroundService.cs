using POS_BFF.Core.Domain.Gateways.Sales;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace POS_BFF.Application.Backgrounds
{
    public class StockBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<StockBackgroundService> _logger;

        public StockBackgroundService(IServiceScopeFactory scopeFactory, ILogger<StockBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var stockService = scope.ServiceProvider.GetRequiredService<ISaleProductServiceGateway>();
                        await stockService.CheckAndNotifyStockAsync();
                    }
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Intervalo de verificação
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao verificar e notificar o estoque.");
                }
            }
        }
    }
}
