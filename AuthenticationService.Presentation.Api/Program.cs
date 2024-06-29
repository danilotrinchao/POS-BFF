using AuthenticationService.Application.Contracts;
using AuthenticationService.Application.Services;
using AuthenticationService.Core.Domain.Configurations;
using AuthenticationService.Core.Domain.Gateways.Cashier;
using AuthenticationService.Core.Domain.Gateways.Sales;
using AuthenticationService.Core.Domain.Repositories;
using AuthenticationService.Domain.Repositories;
using AuthenticationService.Infra.ExternalServices.SalesGateway;
using AuthenticationService.Infra.Repository;
using AuthenticationService.Infra.Utils;
using Autofac.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Data;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Configuração do ambiente
var configuration = builder.Configuration;
// Add services to the container.

// Load JwtSettings from configuration
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
builder.Services.AddSingleton(jwtSettings);

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudiences = jwtSettings.Audience.Split(';'), // Support multiple audiences
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserRepository, UserRepository>(); // Substitua UserRepository pelo seu próprio UserRepository
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
// Substitua AddressRepository pelo seu próprio AddressRepository
builder.Services.AddScoped<IDbConnection>(provider =>
    new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICryptography, Cryptography>();
builder.Services.AddScoped<HashAlgorithm, HMACSHA512>();

// Registro do serviço
builder.Services.AddScoped<IUserService, UserService>(); 
builder.Services.AddScoped<ITokenService , TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Configuração do HttpClient para ISaleServiceGateway
builder.Services.AddScoped<ISalesServiceUsageGateway, SalesServiceUsageGateway>();
builder.Services.AddTransient<ISaleClientServiceGateway, SalesClientServiceGateway>();
builder.Services.AddTransient<ISaleProductServiceGateway, SaleProductServiceGateway>();
builder.Services.AddTransient<ISaleOrderServiceGateway, SalesOrderServiceGateway>();
builder.Services.AddTransient<ICashierOrderServiceGateway, CashierOrderServiceGateway>();
builder.Services.AddHttpClient("SalesApi",
      c => c.BaseAddress = new Uri("http://localhost:7250/"));
builder.Services.AddHttpClient("CashierApi",
      c => c.BaseAddress = new Uri("http://localhost:7209/"));


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SellerOrAdmin", policy =>
        policy.RequireRole("Seller", "Admin"));
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});
builder.Services.AddMvc(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");
//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
