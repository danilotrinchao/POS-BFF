using AuthenticationService.Application.Contracts;
using AuthenticationService.Core.Domain.Enums;
using AuthenticationService.Core.Domain.Gateways.Sales;
using AuthenticationService.Core.Domain.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Infra.ExternalServices.SalesGateway
{
    public class SaleProductServiceGateway : ISaleProductServiceGateway
    {
        private static IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SaleProductServiceGateway(
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
            //var token = _httpContextAccessor.HttpContext.Request.Cookies["accessToken"];

            // Incluir o token no cabeçalho de autorização das requisições HTTP
            // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // Adicionar cabeçalho de autorização

            //if (!string.IsNullOrEmpty(token))
            //{
            //   //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //}
            //else
            //{
            //    throw new UnauthorizedAccessException("Unable to obtain the authentication token.");
            //}

            return httpClient;
        }

        public async Task<Guid> AddProductAsync(PhysiqueProductDTO productDto)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.PostAsJsonAsync("api/Product/physical", productDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Guid>();
        }
        public async Task<Guid> AddServicetAsync(VirtualProductDTO productDto)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.PostAsJsonAsync("api/Product/service", productDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Guid>();
        }

        public async Task<bool> UpdateProductAsync(Guid id, PhysiqueProductDTO productDto)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.PutAsJsonAsync($"api/Product/{id}/updatephysical", productDto);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> UpdateServiceAsync(Guid id, VirtualProductDTO serviceDto)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.PutAsJsonAsync($"api/Product/{id}/updateservice", serviceDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.DeleteAsync($"api/deleteProduct/{id}");
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> DeleteServiceAsync(Guid id)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.DeleteAsync($"api/deleteService/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<PhysiqueProductDTO> GetProductById(Guid id)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.GetAsync($"api/Product/PhysiqueProduct/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PhysiqueProductDTO>();
            }
            throw new HttpRequestException(response.ReasonPhrase);
        }

        public async Task<VirtualProductDTO> GetServiceById(Guid id)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.GetAsync($"api/Product/VirtualProduct/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<VirtualProductDTO>();
            }
            throw new HttpRequestException(response.ReasonPhrase);
        }

        public async Task<IEnumerable<PhysiqueProductDTO>> GetAllProductsAsync()
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.GetAsync("api/Product/products");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<PhysiqueProductDTO>>();
            }
            throw new HttpRequestException(response.ReasonPhrase);
        }
        public async Task<IEnumerable<VirtualProductDTO>> GetAllServicesAsync()
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.GetAsync("api/Product/services");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<VirtualProductDTO>>();
            }
            throw new HttpRequestException(response.ReasonPhrase);
        }

        public async Task<VirtualProductDTO> GetVirtualProductAsync(Guid serviceid)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.GetAsync($"api/Sales/GetVirtualProduct/{serviceid}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<VirtualProductDTO>();
            }
            throw new HttpRequestException(response.ReasonPhrase);
        }

        public async Task<PhysiqueProductDTO> GetPhysiqueProductAsync(Guid productid)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.GetAsync($"api/Sales/GetPhysiqueProduct/{productid}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PhysiqueProductDTO>();
            }
            throw new HttpRequestException(response.ReasonPhrase);
        }

    }

}
