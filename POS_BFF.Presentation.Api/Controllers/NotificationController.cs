using POS_BFF.Application.Contracts;
using POS_BFF.Application.Services;
using POS_BFF.Core.Domain.Gateways.Sales;
using POS_BFF.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace POS_BFF.Presentation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationPublisher _notificationPublisher;
        private readonly ISaleProductServiceGateway _saleProductServiceGateway;
        private readonly IControllTimeService _controlTimeService;

        public NotificationController(INotificationPublisher notificationPublisher, 
            ISaleProductServiceGateway saleProductServiceGateway,
            IControllTimeService controllTimeService)
        {
            _notificationPublisher = notificationPublisher;
            _saleProductServiceGateway = saleProductServiceGateway;
            _controlTimeService = controllTimeService;
        }

        [HttpGet("stream")]
        public async Task StreamNotifications(Guid TenantId)
        {
            Response.ContentType = "text/event-stream";
            //Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            Response.Headers.Add("X-Accel-Buffering", "no"); // NGINX e proxies
            Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            Response.Headers.Add("Pragma", "no-cache");
            Response.Headers.Add("Expires", "0");

            // Loop contínuo para enviar notificações em tempo real
            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                try
                {
                    var productNotifications = await _saleProductServiceGateway.GetNotifyStockAsync(TenantId);
                    var servicesNotifications = await _controlTimeService.GetNotifyServiceAsync();

                    foreach (var notification in productNotifications.Concat(servicesNotifications))
                    {
                        var message = $"data: {notification}\n\n";
                        await Response.WriteAsync(message);
                        await Response.Body.FlushAsync();
                    }

                    await Task.Delay(10000); // Ajuste conforme necessário
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro no SSE: {ex.Message}");
                    break; // Encerra o loop em caso de erro
                }
            }

        }


    }
}
