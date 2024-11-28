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
            try
            {
                Response.ContentType = "text/event-stream";
                Response.Headers.Add("Cache-Control", "no-cache");
                Response.Headers.Add("Connection", "keep-alive");
                Response.Headers.Add("X-Accel-Buffering", "no"); // Desabilita buffering

                while (!HttpContext.RequestAborted.IsCancellationRequested)
                {
                    try
                    {
                        var productNotifications = await _saleProductServiceGateway.GetNotifyStockAsync(TenantId);
                        var servicesNotifications = await _controlTimeService.GetNotifyServiceAsync();

                        foreach (var notification in productNotifications.Concat(servicesNotifications))
                        {
                            var message = $"data: {notification}\n\n"; // Formato SSE
                            await Response.WriteAsync(message);
                            await Response.Body.FlushAsync();
                        }

                        await Task.Delay(10000); // Aguarda antes de enviar a próxima rodada
                    }
                    catch (Exception innerEx)
                    {
                        Console.WriteLine($"Erro ao buscar notificações: {innerEx.Message}");
                        // Opcional: envie uma mensagem SSE com o erro
                        await Response.WriteAsync($"data: Erro ao processar notificações: {innerEx.Message}\n\n");
                        await Response.Body.FlushAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no SSE: {ex.Message}");
                // Opcional: envie resposta de erro para o cliente
                HttpContext.Abort();
            }
        }



    }
}
