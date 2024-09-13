using POS_BFF.Core.Domain.Gateways.Sales;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Infra.ExternalServices.SalesGateway
{
    public class SalesServiceUsageGateway : ISalesServiceUsageGateway
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SalesServiceUsageGateway(IHttpClientFactory httpClientFactory,
                                        IConfiguration configuration,
                                        IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<HttpClient> CreateHttpClientAsync()
        {
            var baseAddress = _configuration["SalesApi:baseAddress"];
            var httpClient = _httpClientFactory.CreateClient("SalesUsageService");
            httpClient.BaseAddress = new Uri(baseAddress);
            return httpClient;
        }

        public async Task StartServiceAsync(Guid orderItemId)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.PostAsync($"api/serviceusage/start/{orderItemId}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task PauseServiceAsync(Guid orderItemId)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.PostAsync($"api/serviceusage/pause/{orderItemId}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task StopServiceAsync(Guid orderItemId)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.PostAsync($"api/serviceusage/stop/{orderItemId}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task<dynamic> GetServiceUsageByOrderItemIdAsync(Guid orderItemId)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.GetAsync($"api/serviceusage/{orderItemId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<dynamic>();
        }
    }
}
