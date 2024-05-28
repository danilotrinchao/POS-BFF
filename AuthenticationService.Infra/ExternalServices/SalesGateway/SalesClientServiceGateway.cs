using AuthenticationService.Core.Domain.Gateways.Sales;
using AuthenticationService.Core.Domain.Requets;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Net.Http;
using AuthenticationService.Application.Contracts;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;


namespace AuthenticationService.Infra.ExternalServices.SalesGateway
{
    public class SalesClientServiceGateway : ISaleClientServiceGateway
    {
        private static IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SalesClientServiceGateway(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IAuthService authService,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<HttpClient> CreateHttpClientAsync()
        {
            var baseAddress = _configuration["SalesApi:baseAddress"];
            var httpClient = _httpClientFactory.CreateClient("SalesServiceClient");
            httpClient.BaseAddress = new Uri(baseAddress);
            var token = _httpContextAccessor.HttpContext.Request.Cookies["accessToken"];

            // Incluir o token no cabeçalho de autorização das requisições HTTP
            // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // Adicionar cabeçalho de autorização

            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                throw new UnauthorizedAccessException("Unable to obtain the authentication token.");
            }

            return httpClient;
        }

        public async Task<int> CreateClientAsync(ClientRequest client)
        {
            try
            {
                var httpClient = await CreateHttpClientAsync();
                var response = await httpClient.PostAsJsonAsync("api/Client", client);
                response.EnsureSuccessStatusCode();
                var createdClient = await response.Content.ReadFromJsonAsync<ClientRequest>();
                return createdClient?.Id ?? 0;
            }
            catch (Exception ex)
            {
                // Tratar exceção e registrar para monitoramento
                throw new Exception(ex.Message);
            }
        }

        public async Task<ClientRequest> GetClientByIdAsync(int id)
        {
            try
            {
                var httpClient = await CreateHttpClientAsync();
                var response = await httpClient.GetAsync($"api/Client/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ClientRequest>();
                }
                throw new HttpRequestException(response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                // Tratar exceção e registrar para monitoramento
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<ClientRequest>> GetAllClientsAsync()
        {
            try
            {
                var httpClient = await CreateHttpClientAsync();
                var response = await httpClient.GetAsync("api/Client");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<ClientRequest>>();
                }
                throw new HttpRequestException(response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                // Tratar exceção e registrar para monitoramento
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateClientAsync(ClientRequest client)
        {
            try
            {
                var httpClient = await CreateHttpClientAsync();
                var response = await httpClient.PutAsJsonAsync($"api/Client/{client.Id}", client);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Tratar exceção e registrar para monitoramento
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            try
            {
                var httpClient = await CreateHttpClientAsync();
                var response = await httpClient.DeleteAsync($"api/Client/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Tratar exceção e registrar para monitoramento
                throw new Exception(ex.Message);
            }
        }
    }


}
