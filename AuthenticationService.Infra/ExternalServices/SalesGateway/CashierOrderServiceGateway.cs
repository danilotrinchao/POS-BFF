using AuthenticationService.Core.Domain.Gateways.Cashier;
using AuthenticationService.Core.Domain.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AuthenticationService.Infra.ExternalServices.SalesGateway
{
    public class CashierOrderServiceGateway : ICashierOrderServiceGateway
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CashierOrderServiceGateway(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<HttpClient> CreateHttpClientAsync()
        {
            var baseAddress = _configuration["CashierApi:baseAddress"];
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(baseAddress);
            return httpClient;
        }

        public async Task<OrderDto> GetOrderByIdAsync(Guid id)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.GetAsync($"api/orders/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<OrderDto>();
            }
            throw new HttpRequestException(response.ReasonPhrase);
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.GetAsync("api/orders");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<OrderDto>>();
            }
            throw new HttpRequestException(response.ReasonPhrase);
        }

        public async Task<Guid> CreateOrderAsync(SaleDTO saleDto)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.PostAsJsonAsync("api/orders", saleDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Guid>();
        }

        public async Task<bool> UpdateOrderAsync(Guid id, SaleDTO saleDto)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.PutAsJsonAsync($"api/orders/{id}", saleDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.DeleteAsync($"api/orders/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> OpenCashier(decimal InitialBalance, int EmployeerId)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.DeleteAsync($"api/open?InitialBalance={InitialBalance}&EmployeerId={EmployeerId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CloseCashier(Guid CashierId)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.DeleteAsync($"api/close?cashierId={CashierId}");
            return response.IsSuccessStatusCode;
        }
    }

}
