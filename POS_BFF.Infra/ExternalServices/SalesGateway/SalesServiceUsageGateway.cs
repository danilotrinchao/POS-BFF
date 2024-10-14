using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using POS_BFF.Core.Domain.Gateways.Authentication;
using POS_BFF.Core.Domain.Gateways.Sales;
using System.Net.Http.Json;

namespace POS_BFF.Infra.ExternalServices.SalesGateway
{
    public class SalesServiceUsageGateway : ISalesServiceUsageGateway
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticationTenantGateway _authenticationTenantGateway;

        public SalesServiceUsageGateway(IHttpClientFactory httpClientFactory,
                                        IConfiguration configuration,
                                        IHttpContextAccessor httpContextAccessor,
                                        IAuthenticationTenantGateway authenticationTenantGateway)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _authenticationTenantGateway = authenticationTenantGateway;
        }

        private async Task<HttpClient> CreateHttpClientAsync()
        {
            var baseAddress = _configuration["SalesApi:baseAddress"];
            var httpClient = _httpClientFactory.CreateClient("SalesUsageService");
            httpClient.BaseAddress = new Uri(baseAddress);
            return httpClient;
        }

        public async Task StartServiceAsync(Guid orderItemId, Guid TenantId)
        {
            var httpClient = await CreateHttpClientAsync();
            var cs = await _authenticationTenantGateway.GetConnectionStringByTenantIdAsync(TenantId);
            httpClient.DefaultRequestHeaders.Add("X-Connection-String", cs.ConnectionString);
            httpClient.DefaultRequestHeaders.Add("X-Schema", cs.Schema);
            var response = await httpClient.PostAsync($"api/serviceusage/start/{orderItemId}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task PauseServiceAsync(Guid orderItemId, Guid TenantId)
        {
            var httpClient = await CreateHttpClientAsync();
            var cs = await _authenticationTenantGateway.GetConnectionStringByTenantIdAsync(TenantId);
            httpClient.DefaultRequestHeaders.Add("X-Connection-String", cs.ConnectionString);
            httpClient.DefaultRequestHeaders.Add("X-Schema", cs.Schema);
            var response = await httpClient.PostAsync($"api/serviceusage/pause/{orderItemId}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task StopServiceAsync(Guid orderItemId, Guid TenantId)
        {
            var httpClient = await CreateHttpClientAsync();
            var cs = await _authenticationTenantGateway.GetConnectionStringByTenantIdAsync(TenantId);
            httpClient.DefaultRequestHeaders.Add("X-Connection-String", cs.ConnectionString);
            httpClient.DefaultRequestHeaders.Add("X-Schema", cs.Schema);
            var response = await httpClient.PostAsync($"api/serviceusage/stop/{orderItemId}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task<dynamic> GetServiceUsageByOrderItemIdAsync(Guid orderItemId, Guid TenantId)
        {
            var httpClient = await CreateHttpClientAsync();
            var cs = await _authenticationTenantGateway.GetConnectionStringByTenantIdAsync(TenantId);
            httpClient.DefaultRequestHeaders.Add("X-Connection-String", cs.ConnectionString);
            httpClient.DefaultRequestHeaders.Add("X-Schema", cs.Schema);
            var response = await httpClient.GetAsync($"api/serviceusage/{orderItemId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<dynamic>();
        }
    }
}
