namespace POS_BFF.Core.Domain.Interfaces
{
    public interface IStockBackgroundService
    {
        Task CheckAndNotifyStockAsync();
    }
}
