using POS_BFF.Application.Backgrounds;
using POS_BFF.Application.Contracts;
using POS_BFF.Application.Services;
using POS_BFF.Core.Domain.Configurations;
using POS_BFF.Core.Domain.Gateways.Cashier;
using POS_BFF.Core.Domain.Gateways.Sales;
using POS_BFF.Core.Domain.Interfaces;
using POS_BFF.Core.Domain.Repositories;
using POS_BFF.Domain.Repositories;
using POS_BFF.Infra.Cache;
using POS_BFF.Infra.ExternalServices.SalesGateway;
using POS_BFF.Infra.Notifications;
using POS_BFF.Infra.Repository;
using POS_BFF.Infra.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using StackExchange.Redis;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using POS_BFF.Infra.ExternalServices.CashierGateway;
using POS_BFF.Core.Domain.Gateways.Authentication;
using POS_BFF.Infra.ExternalServices.AuthenticationGateway;
using POS_BFF.Core.Domain.Gateways.Company;
using POS_BFF.Infra.ExternalServices.CompanyGateway;

var builder = WebApplication.CreateBuilder(args);

// Configura��o do ambiente
var configuration = builder.Configuration;

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

builder.Services.AddScoped<IDbConnection>(provider =>
    new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")));
// Register scoped and singleton services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IConsumerServiceRepository, ConsumerServiceRepository>();

builder.Services.AddScoped<ICryptography, Cryptography>();
builder.Services.AddScoped<HashAlgorithm, HMACSHA512>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IControllTimeService, ControlTimeService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Register notification services
builder.Services.AddSingleton<INotificationPublisher, NotificationPublisher>();
builder.Services.AddScoped<ITimerCache, TimerCache>();
var redisConnectionString = "localhost:6379,abortConnect=false,connectTimeout=10000";
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddScoped<IDatabase>(sp =>
{
    var connectionMultiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
    return connectionMultiplexer.GetDatabase();
});

// Register background service
builder.Services.AddScoped<ISaleProductServiceGateway, SaleProductServiceGateway>();
builder.Services.AddTransient<ISaleClientServiceGateway, SalesClientServiceGateway>();
builder.Services.AddScoped<ISalesServiceUsageGateway, SalesServiceUsageGateway>();
builder.Services.AddTransient<ISaleOrderServiceGateway, SalesOrderServiceGateway>();
builder.Services.AddTransient<ICashierOrderServiceGateway, CashierOrderServiceGateway>();
builder.Services.AddTransient<IAuthenticationTenantGateway, AuthenticationTenantGateway>();
builder.Services.AddTransient<ICompanyEmployeerGateway, CompanyEmployeerGateway>();
builder.Services.AddHttpClient("SalesApi", c => c.BaseAddress = new Uri("http://localhost:7250/"));
builder.Services.AddHttpClient("CashierApi", c => c.BaseAddress = new Uri("http://localhost:7209/"));
builder.Services.AddHttpClient("AuthenticationApi", c => c.BaseAddress = new Uri("http://localhost:7278/"));
builder.Services.AddHttpClient("CompanyApi", c => c.BaseAddress = new Uri("https://localhost:7223/"));

// Register the background service
builder.Services.AddHostedService<StockBackgroundService>();
builder.Services.AddHostedService<TimerBackgroundService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Configure MVC options
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
app.UseStaticFiles();
app.MapControllers();

app.Run();
