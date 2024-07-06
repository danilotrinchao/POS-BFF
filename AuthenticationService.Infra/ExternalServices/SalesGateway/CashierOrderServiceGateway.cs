using AuthenticationService.Core.Domain.Enums;
using AuthenticationService.Core.Domain.Gateways.Cashier;
using AuthenticationService.Core.Domain.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

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

            var responseString = await response.Content.ReadAsStringAsync();
            responseString = responseString.Replace("\\", "").Trim('"');
            if (!responseString.IsNullOrEmpty())
            {
                var result = Guid.Parse(responseString);
                return result;
            }
            else
            {
                throw new Exception($"Failed to open cashier. Status code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
            }
        }


        public async Task<bool> CloseCashier(int employeerId, Dictionary<EPaymentType, decimal> totals)
        {
            var httpClient = await CreateHttpClientAsync();
            var url = $"/close?employeerId={employeerId}";

            // Serializar o objeto totals como JSON
            var jsonContent = JsonConvert.SerializeObject(totals);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            // Send the PUT request with the JSON content
            var response = await httpClient.PutAsync(url, content);

            return response.IsSuccessStatusCode;
        }


    }

}
