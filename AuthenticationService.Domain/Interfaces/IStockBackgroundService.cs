namespace AuthenticationService.Core.Domain.Interfaces
{
    public interface IStockBackgroundService
    {
        Task CheckAndNotifyStockAsync();
    }
}
