using AuthenticationService.Application.Contracts;
using AuthenticationService.Application.Services;
using AuthenticationService.Core.Domain.Configurations;
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
// Configura��o do ambiente
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
builder.Services.AddScoped<IUserRepository, UserRepository>(); // Substitua UserRepository pelo seu pr�prio UserRepository
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
// Substitua AddressRepository pelo seu pr�prio AddressRepository
builder.Services.AddScoped<IDbConnection>(provider =>
    new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICryptography, Cryptography>();
builder.Services.AddScoped<HashAlgorithm, HMACSHA512>();

// Registro do servi�o
builder.Services.AddScoped<IUserService, UserService>(); 
builder.Services.AddScoped<ITokenService , TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddHttpClient<ISaleClientServiceGateway, SalesClientServiceGateway>();
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
