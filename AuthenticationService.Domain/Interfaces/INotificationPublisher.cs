namespace AuthenticationService.Core.Domain.Interfaces
{
    public interface INotificationPublisher
    {
        Task PublishAsync(string message);
        Task<string> GetNextNotificationAsync(CancellationToken cancellationToken);
        
    }
}
