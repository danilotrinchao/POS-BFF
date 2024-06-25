﻿using AuthenticationService.Application.Contracts;
using AuthenticationService.Core.Domain.Enums;
using AuthenticationService.Core.Domain.Gateways.Sales;
using AuthenticationService.Core.Domain.Requests;
using AuthenticationService.Domain.Repositories;
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
    public class SalesOrderServiceGateway : ISaleOrderServiceGateway
    {
        private static IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISaleProductServiceGateway _saleProductServiceGateway;
        private readonly IUserRepository _userRepository;

        public SalesOrderServiceGateway(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IAuthService authService,
            IHttpContextAccessor httpContextAccessor,
            ISaleProductServiceGateway saleProductServiceGateway,
            IUserRepository userRepository)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _saleProductServiceGateway = saleProductServiceGateway;
            _userRepository = userRepository;
        }

        private async Task<HttpClient> CreateHttpClientAsync()
        {
            var baseAddress = _configuration["SalesApi:baseAddress"];
            var httpClient = _httpClientFactory.CreateClient("SalesServiceClient");
            httpClient.BaseAddress = new Uri(baseAddress);
          
            return httpClient;
        }

        public async Task<Guid> CreateSaleAsync(SaleDTO saleDto)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.PostAsJsonAsync("api/Sales", saleDto);
            response.EnsureSuccessStatusCode();
            var saleId = await response.Content.ReadFromJsonAsync<Guid>();
            return saleId;
        }

        public async Task<SaleDTO> GetSaleByIdAsync(Guid id)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.GetAsync($"api/Sales/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<SaleDTO>();
            }
            throw new HttpRequestException(response.ReasonPhrase);
        }

        public async Task<IEnumerable<SaleDTO>> GetAllSalesAsync()
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.GetAsync("api/Sales");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<SaleDTO>>();
            }
            throw new HttpRequestException(response.ReasonPhrase);
        }

        public async Task<bool> CompleteSaleAsync(Guid id)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.PutAsJsonAsync($"api/Sales/{id}/complete", id);
            if (response.IsSuccessStatusCode)
            {
                var order = await GetSaleByIdAsync(id);
                
                foreach (var item in order.Produtos)
                {
                    var service = await _saleProductServiceGateway.GetVirtualProductAsync(item.ProductId);
                    if (item.ProductType == EProductType.VirtualProduct && service.Name.Equals("Pc Gamer", StringComparison.OrdinalIgnoreCase))
                        await _userRepository.UpdateUserClientAvailableTimeAsync(order.ClientId, item.Quantity);
                        
                }
            }
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CancelSaleAsync(Guid id)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.PutAsJsonAsync($"api/Sales/{id}/cancel", id);
            return response.IsSuccessStatusCode;
        }
    }
}
