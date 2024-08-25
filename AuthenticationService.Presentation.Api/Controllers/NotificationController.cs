using AuthenticationService.Application.Contracts;
using AuthenticationService.Application.Services;
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
        public async Task<IActionResult> StreamNotifications()
        {
            Response.ContentType = "text/event-stream";
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            // Loop contínuo para enviar notificações em tempo real
            while (true)
            {
                // Notificações de produtos
                var productNotifications = await _saleProductServiceGateway.GetNotifyStockAsync();
                var servicesNotifications = await _controlTimeService.GetNotifyServiceAsync();
                foreach (var notification in productNotifications)
                {
                    await _notificationPublisher.PublishAsync(notification);
                    await Task.Delay(500000); // Exemplo: Atraso de 5 segundos entre as notificações
                }
                foreach (var notification in servicesNotifications)
                {
                    await _notificationPublisher.PublishAsync(notification);
                    await Task.Delay(500000); // Exemplo: Atraso de 5 segundos entre as notificações
                }
                // Notificações de controle de tempo de serviço
                await _controlTimeService.CheckAndNotifyServiceTimeAsync();

                await Task.Delay(100000); // Ajuste o intervalo conforme necessário
            }

            // Este return nunca será alcançado, pois o loop acima é contínuo.
            // Mas o método precisa ter um retorno válido para compilar.
            return new EmptyResult();
        }

    }
}
