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
        public async Task StreamNotifications()
        {
            Response.ContentType = "text/event-stream";
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            // Loop contínuo para enviar notificações em tempo real
            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                // Notificações de produtos
                var productNotifications = await _saleProductServiceGateway.GetNotifyStockAsync();
                var servicesNotifications = await _controlTimeService.GetNotifyServiceAsync();

                foreach (var notification in productNotifications.Concat(servicesNotifications))
                {
                    var message = $"data: {notification}\n\n"; // Formato para SSE
                    await Response.WriteAsync(message);
                    await Response.Body.FlushAsync(); // Assegura que a mensagem é enviada imediatamente
                    await Task.Delay(5000); // Ajuste o atraso conforme necessário
                }

                await Task.Delay(10000); // Ajuste o intervalo conforme necessário
            }
        }


    }
}
