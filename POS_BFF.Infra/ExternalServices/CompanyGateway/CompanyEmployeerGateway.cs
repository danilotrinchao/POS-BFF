using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using POS_BFF.Core.Domain.Gateways.Authentication;
using POS_BFF.Core.Domain.Gateways.Company;
using POS_BFF.Core.Domain.Requests;
using POS_BFF.Core.Domain.Requets;
using POS_BFF.Core.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Infra.ExternalServices.CompanyGateway
{
    public class CompanyEmployeerGateway: ICompanyEmployeerGateway
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticationTenantGateway _authenticationTenantGateway;

        public CompanyEmployeerGateway(IHttpClientFactory httpClientFactory,
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
            var baseAddress = _configuration["AuthenticationApi:baseAddress"];
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(baseAddress);
            return httpClient;
        }

        public async Task<Guid> CreateEmployeer(EmployeerDTO employeer, Guid TenantId)
        {
            try
            {
                var httpClient = await CreateHttpClientAsync();
                var cs = await _authenticationTenantGateway.GetConnectionStringByTenantIdAsync(TenantId);
                httpClient.DefaultRequestHeaders.Add("X-Connection-String", cs.ConnectionString);
                httpClient.DefaultRequestHeaders.Add("X-Schema", cs.Schema);
                var response = await httpClient.PostAsJsonAsync("api/createEmployeer", employeer);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    await _authenticationTenantGateway.CreateUserEmployeer(employeer, TenantId);
                }
                response.EnsureSuccessStatusCode();
                var createdClient = await response.Content.ReadFromJsonAsync<EmployeerDTO>();
                return createdClient?.Id ?? Guid.NewGuid();
            }
            catch (Exception ex)
            {
                // Tratar exceção e registrar para monitoramento
                throw new Exception(ex.Message);
            }
        }

        public async Task<EmployeerDTO> GetEmployeerById(Guid id, Guid TenantId)
        {
            try
            {
                var httpClient = await CreateHttpClientAsync();
                var cs = await _authenticationTenantGateway.GetConnectionStringByTenantIdAsync(TenantId);
                httpClient.DefaultRequestHeaders.Add("X-Connection-String", cs.ConnectionString);
                httpClient.DefaultRequestHeaders.Add("X-Schema", cs.Schema);
                var response = await httpClient.GetAsync($"employeer/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<EmployeerDTO>();
                }
                throw new HttpRequestException(response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                // Tratar exceção e registrar para monitoramento
                throw new Exception(ex.Message);
            }
        }
        public async Task<EmployeerDTO> GetEmployeerByEmailAsync(string email, Guid TenantId)
        {
            try
            {
                var httpClient = await CreateHttpClientAsync();
                var cs = await _authenticationTenantGateway.GetConnectionStringByTenantIdAsync(TenantId);
                httpClient.DefaultRequestHeaders.Add("X-Connection-String", cs.ConnectionString);
                httpClient.DefaultRequestHeaders.Add("X-Schema", cs.Schema);
                var response = await httpClient.GetAsync($"email/{email}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<EmployeerDTO>();
                }
                throw new HttpRequestException(response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                // Tratar exceção e registrar para monitoramento
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<EmployeerDTO>> GetAllEmployeers(Guid TenantId)
        {
            try
            {
                var httpClient = await CreateHttpClientAsync();
                var cs = await _authenticationTenantGateway.GetConnectionStringByTenantIdAsync(TenantId);
                httpClient.DefaultRequestHeaders.Add("X-Connection-String", cs.ConnectionString);
                httpClient.DefaultRequestHeaders.Add("X-Schema", cs.Schema);
                var response = await httpClient.GetAsync("/api/Employeer/getAllEmployeers");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<EmployeerDTO>>();
                }
                throw new HttpRequestException(response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                // Tratar exceção e registrar para monitoramento
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateEmployeerAsync(EmployeerDTO employeer, Guid TenantId)
        {
            try
            {
                var httpClient = await CreateHttpClientAsync();
                var cs = await _authenticationTenantGateway.GetConnectionStringByTenantIdAsync(TenantId);
                httpClient.DefaultRequestHeaders.Add("X-Connection-String", cs.ConnectionString);
                httpClient.DefaultRequestHeaders.Add("X-Schema", cs.Schema);
                var response = await httpClient.PutAsJsonAsync($"api/updateEmployeer/{employeer.Id}", employeer);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Tratar exceção e registrar para monitoramento
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteEmployeer(Guid id, Guid TenantId)
        {
            try
            {
                var httpClient = await CreateHttpClientAsync();
                var cs = await _authenticationTenantGateway.GetConnectionStringByTenantIdAsync(TenantId);
                httpClient.DefaultRequestHeaders.Add("X-Connection-String", cs.ConnectionString);
                httpClient.DefaultRequestHeaders.Add("X-Schema", cs.Schema);
                var response = await httpClient.DeleteAsync($"api/Client/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Tratar exceção e registrar para monitoramento
                throw new Exception(ex.Message);
            }
        }

        Task<EmployeerDTO> ICompanyEmployeerGateway.GetEmployeerByCPFAsync(string cpf, Guid TenantId)
        {
            throw new NotImplementedException();
        }

        Task<List<UserRole>> ICompanyEmployeerGateway.GetEmployeerRolesById(Guid id, Guid TenantId)
        {
            throw new NotImplementedException();
        }
    }


}
