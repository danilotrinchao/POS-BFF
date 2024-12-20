﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using POS_BFF.Application.DTOs;
using POS_BFF.Core.Domain.Entities;
using POS_BFF.Core.Domain.Gateways.Authentication;
using POS_BFF.Core.Domain.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Infra.ExternalServices.AuthenticationGateway
{
    public class AuthenticationTenantGateway:IAuthenticationTenantGateway
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationTenantGateway(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<HttpClient> CreateHttpClientAsync()
        {
            var baseAddress = _configuration["AuthenticationApi:baseAddress"];
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(baseAddress);
            return httpClient;
        }


        public async Task<Tenant> GetConnectionStringByTenantIdAsync(Guid tenantId)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.GetAsync($"/connection-string?tenantId={tenantId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Tenant>();
            }

            throw new HttpRequestException(response.ReasonPhrase);
        }

        public async Task<Guid> CreateUserEmployeer(EmployeerDTO employeer, Guid TenantId)
        {
            try
            {
                var register = new RegisterUserDTO
                {
                    Email = employeer.Email,
                    Password = employeer.PasswordHash,
                    TenantId = TenantId
                };
                var httpClient = await CreateHttpClientAsync();
                var response = await httpClient.PostAsJsonAsync("/register", register);
                response.EnsureSuccessStatusCode();
                var createdClient = await response.Content.ReadFromJsonAsync<RegisterUserDTO>();
                return Guid.NewGuid();
            }
            catch (Exception ex)
            {
                // Tratar exceção e registrar para monitoramento
                throw new Exception(ex.Message);
            }
        }


    }
}
