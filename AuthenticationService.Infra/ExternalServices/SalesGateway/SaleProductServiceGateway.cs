﻿using AuthenticationService.Application.Contracts;
using AuthenticationService.Core.Domain.Enums;
using AuthenticationService.Core.Domain.Gateways.Sales;
using AuthenticationService.Core.Domain.Interfaces;
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
        private readonly INotificationPublisher _notificationPublisher;
        private readonly HashSet<string> _notifiedProducts = new HashSet<string>();

        public SaleProductServiceGateway(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IAuthService authService,
            IHttpContextAccessor httpContextAccessor,
            INotificationPublisher notificationPublisher)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
            _notificationPublisher = notificationPublisher;
        }

        private async Task<HttpClient> CreateHttpClientAsync()
        {
            var baseAddress = _configuration["SalesApi:baseAddress"];
            var httpClient = _httpClientFactory.CreateClient("SalesServiceClient");
            httpClient.BaseAddress = new Uri(baseAddress);

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
            var response = await httpClient.PutAsJsonAsync($"api/Product/{id}/updateProduct", productDto);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> UpdateServiceAsync(Guid id, VirtualProductDTO serviceDto)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.PutAsJsonAsync($"api/Product/{id}/updateService", serviceDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.DeleteAsync($"api/Product/deleteProduct/{id}");
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> DeleteServiceAsync(Guid id)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.DeleteAsync($"api/Product/deleteService/{id}");
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

        public async Task<PhysiqueProductDTO> GetProductByBarCode(string barcode)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.GetAsync($"api/Product/getbybarcode/{barcode}");
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

        public async Task CheckAndNotifyStockAsync()
        {
            var products = await GetAllProductsAsync();

            var productsOutOfStock = products.Where(x => x.Quantity <= 0).ToList();
            var productsNearDueDate = products.Where(x => x.DueDate.HasValue &&
                                                          x.DueDate.Value.Date <= DateTime.UtcNow.AddDays(3).Date &&
                                                          x.DueDate.Value.Date >= DateTime.UtcNow.Date).ToList();

            foreach (var product in productsOutOfStock)
            {
                if (_notifiedProducts.Contains(product.Name))
                    continue; // Ignore products that have already been notified

                var message = $"Produto esgotado em estoque: {product.Name}";
                await _notificationPublisher.PublishAsync(message);
                _notifiedProducts.Add(product.Name); // Mark the product as notified
            }

            foreach (var product in productsNearDueDate)
            {
                if (_notifiedProducts.Contains(product.Name))
                    continue; // Ignore products that have already been notified

                var message = $"Produto próximo da data de vencimento: {product.Name}, Data de vencimento: {product.DueDate.Value.ToShortDateString()}";
                await _notificationPublisher.PublishAsync(message);
                _notifiedProducts.Add(product.Name); // Mark the product as notified
            }
        }

        public async Task<List<string>> GetNotifyStockAsync()
        {
            var notifications = new List<string>();

            var products = await GetAllProductsAsync();

            var productsOutOfStock = products.Where(x => x.Quantity <= 0).ToList();
            var productsNearDueDate = products.Where(x => x.DueDate.HasValue &&
                                                          x.DueDate.Value.Date <= DateTime.UtcNow.AddDays(3).Date &&
                                                          x.DueDate.Value.Date >= DateTime.UtcNow.Date).ToList();

            foreach (var product in productsOutOfStock)
            {
                if (_notifiedProducts.Contains(product.Name))
                    continue; // Ignora produtos já notificados

                var message = $"Produto esgotado em estoque: {product.Name}";
                notifications.Add(message); // Adiciona a mensagem à lista de notificações
                _notifiedProducts.Add(product.Name); // Marca o produto como notificado
            }

            foreach (var product in productsNearDueDate)
            {
                if (_notifiedProducts.Contains(product.Name))
                    continue; // Ignora produtos já notificados

                var message = $"Produto próximo da data de vencimento: {product.Name}, Data de vencimento: {product.DueDate.Value.ToShortDateString()}";
                notifications.Add(message); // Adiciona a mensagem à lista de notificações
                _notifiedProducts.Add(product.Name); // Marca o produto como notificado
            }

            return notifications; // Retorna a lista de notificações
        }



    }

}
