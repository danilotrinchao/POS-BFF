using Microsoft.AspNetCore.Http;
using POS_BFF.Core.Domain.Interfaces;
using System.Collections.Concurrent;
using System.Text;

namespace POS_BFF.Infra.Notifications
{
    public class NotificationPublisher : INotificationPublisher
    {

        private readonly BlockingCollection<string> _notifications = new BlockingCollection<string>();
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationPublisher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task PublishAsync(string message)
        {
            var response = _httpContextAccessor.HttpContext?.Response;
            if (response != null)
            {
                response.ContentType = "text/event-stream";

                if (!response.Headers.ContainsKey("Cache-Control"))
                {
                    response.Headers.Add("Cache-Control", "no-cache");
                }

                if (!response.Headers.ContainsKey("Connection"))
                {
                    response.Headers.Add("Connection", "keep-alive");
                }

                var formattedMessage = $"data: {message}\n\n";
                var buffer = Encoding.UTF8.GetBytes(formattedMessage);

                await response.Body.WriteAsync(buffer, 0, buffer.Length);
                await response.Body.FlushAsync();
            }
        }

        public Task<string> GetNextNotificationAsync(CancellationToken cancellationToken)
        {
            // Este método bloqueará a execução até que uma nova notificação esteja disponível ou a operação seja cancelada
            return Task.FromResult(_notifications.Take(cancellationToken));
        }
    }
}
