using AuthenticationService.Application.Contracts;
using AuthenticationService.Application.Services;
using AuthenticationService.Domain.Repositories;
using AuthenticationService.Infra.Repository;
using AuthenticationService.Infra.Utils;
using Autofac.Core;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
// Configuração do ambiente
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserRepository, UserRepository>(); // Substitua UserRepository pelo seu próprio UserRepository
builder.Services.AddScoped<IAddressRepository, AddressRepository>(); // Substitua AddressRepository pelo seu próprio AddressRepository
builder.Services.AddScoped<IDbConnection>(provider =>
    new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICryptography, Cryptography>();
builder.Services.AddScoped<HashAlgorithm, HMACSHA512>();

// Registro do serviço
builder.Services.AddScoped<IUserService, UserService>(); 
builder.Services.AddScoped<ITokenService , TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
