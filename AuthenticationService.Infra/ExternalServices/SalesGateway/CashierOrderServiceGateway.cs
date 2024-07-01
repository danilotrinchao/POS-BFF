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

        public async Task<Guid> OpenCashier(decimal InitialBalance, int EmployeerId)
        {
            var httpClient = await CreateHttpClientAsync();
            var cashier = new CashierDto
            {
                EmployeerId = EmployeerId,
                InitialBalance = InitialBalance
            };

            var response = await httpClient.PostAsJsonAsync("/open", cashier);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                return Guid.Parse(responseString);
            }
            else
            {
                throw new Exception($"Failed to open cashier. Status code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
            }
        }


        public async Task<bool> CloseCashier(Guid CashierId)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.DeleteAsync($"api/close?cashierId={CashierId}");
            return response.IsSuccessStatusCode;
        }
    }

}
