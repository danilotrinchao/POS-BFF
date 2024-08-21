using AuthenticationService.Core.Domain.Gateways.Sales;
using AuthenticationService.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace AuthenticationService.Presentation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationPublisher _notificationPublisher;
        private readonly ISaleProductServiceGateway _saleProductServiceGateway;

        public NotificationController(INotificationPublisher notificationPublisher, ISaleProductServiceGateway saleProductServiceGateway)
        {
            _notificationPublisher = notificationPublisher;
            _saleProductServiceGateway = saleProductServiceGateway;
        }

        [HttpGet("stream")]
        public async Task<IActionResult> StreamNotifications()
        {
            Response.ContentType = "text/event-stream";
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            var notifications = await _saleProductServiceGateway.GetNotifyStockAsync();

            foreach (var notification in notifications)
            {
                await _notificationPublisher.PublishAsync(notification);
                await Task.Delay(5000); // Exemplo: Atraso de 5 segundos entre as notificações
            }

            return new EmptyResult();
        }
    }
}
