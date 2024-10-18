namespace POS_BFF.Core.Domain.Gateways.Sales
{
    public interface ISalesServiceUsageGateway
    {
        Task StartServiceAsync(Guid orderItemId, Guid TenantId);
        Task PauseServiceAsync(Guid orderItemId, Guid TenantId);
        Task StopServiceAsync(Guid orderItemId, Guid TenantId);
        Task<dynamic> GetServiceUsageByOrderItemIdAsync(Guid orderItemId, Guid TenantId);
    }
}
