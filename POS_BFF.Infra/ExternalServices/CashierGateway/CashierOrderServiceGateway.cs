using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using POS_BFF.Core.Domain.Enums;
using POS_BFF.Core.Domain.Gateways.Authentication;
using POS_BFF.Core.Domain.Gateways.Cashier;
using POS_BFF.Core.Domain.Requests;
using System.Net.Http.Json;
using System.Text;

namespace POS_BFF.Infra.ExternalServices.CashierGateway
{
    public class CashierOrderServiceGateway : ICashierOrderServiceGateway
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticationTenantGateway _authenticationTenantGateway;

        public CashierOrderServiceGateway(IHttpClientFactory httpClientFactory,
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
            var baseAddress = _configuration["CashierApi:baseAddress"];
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(baseAddress);
            return httpClient;
        }

        public async Task<Guid> OpenCashier(decimal InitialBalance, Guid EmployeerId, Guid TentantId)
        {
            var httpClient = await CreateHttpClientAsync();
            var cs = await _authenticationTenantGateway.GetConnectionStringByTenantIdAsync(TentantId);
            httpClient.DefaultRequestHeaders.Add("X-Connection-String", cs.ConnectionString);
            httpClient.DefaultRequestHeaders.Add("X-Schema", cs.Schema);
            var cashier = new CashierOpenedDto
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


        public async Task<bool> CloseCashier(Guid employeerId, Dictionary<EPaymentType, decimal> totals, Guid TentantId)
        {
            var httpClient = await CreateHttpClientAsync();
            var cs = await _authenticationTenantGateway.GetConnectionStringByTenantIdAsync(TentantId);
            httpClient.DefaultRequestHeaders.Add("X-Connection-String", cs.ConnectionString);
            httpClient.DefaultRequestHeaders.Add("X-Schema", cs.Schema);
            var url = $"/close?employeerId={employeerId}";

            // Serializar o objeto totals como JSON
            var jsonContent = JsonConvert.SerializeObject(totals);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Send the PUT request with the JSON content
            var response = await httpClient.PutAsync(url, content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> GetOpenedCashier(Guid TentantId)
        {
            var httpClient = await CreateHttpClientAsync();
            var cs = await _authenticationTenantGateway.GetConnectionStringByTenantIdAsync(TentantId);
            httpClient.DefaultRequestHeaders.Add("X-Connection-String", cs.ConnectionString);
            httpClient.DefaultRequestHeaders.Add("X-Schema", cs.Schema);
            var response = await httpClient.GetAsync("/openedCashier");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            throw new HttpRequestException(response.ReasonPhrase);
        }

        public async Task<bool> GetOpenedCashierByEmployeerId(Guid employeerId, Guid TentantId)
        {
            var httpClient = await CreateHttpClientAsync();
            var cs = await _authenticationTenantGateway.GetConnectionStringByTenantIdAsync(TentantId);
            httpClient.DefaultRequestHeaders.Add("X-Connection-String", cs.ConnectionString);
            httpClient.DefaultRequestHeaders.Add("X-Schema", cs.Schema);
            var response = await httpClient.GetAsync($"/{employeerId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            throw new HttpRequestException(response.ReasonPhrase);
        }


    }

}
