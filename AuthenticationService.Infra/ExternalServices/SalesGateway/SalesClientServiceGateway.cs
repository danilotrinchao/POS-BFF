using AuthenticationService.Application.Contracts;
using AuthenticationService.Core.Domain.Gateways;
using AuthenticationService.Core.Domain.Gateways.Sales;
using AuthenticationService.Core.Domain.Requets;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Infra.ExternalServices.SalesGateway
{
    public class SalesClientServiceGateway : ISaleClientServiceGateway
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public SalesClientServiceGateway(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["SalesApi:BaseUrl"]);
        }

        public async Task<int> CreateClientAsync(ClientRequest client)
        {
            var response = await _httpClient.PostAsJsonAsync("clients", client);
            response.EnsureSuccessStatusCode();
            var createdClient = await response.Content.ReadFromJsonAsync<ClientRequest>();
            return createdClient?.Id ?? 0;
        }

        public async Task<ClientRequest> GetClientByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"clients/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ClientRequest>();
            }
            return null;
        }

        public async Task<IEnumerable<ClientRequest>> GetAllClientsAsync()
        {
            var response = await _httpClient.GetAsync("clients");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<ClientRequest>>();
            }
            return null;
        }

        public async Task<bool> UpdateClientAsync(ClientRequest client)
        {
            var response = await _httpClient.PutAsJsonAsync($"clients/{client.Id}", client);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"clients/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
