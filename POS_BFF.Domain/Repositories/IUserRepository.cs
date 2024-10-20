﻿using POS_BFF.Domain.Entities;

namespace POS_BFF.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmail(string email);
        Task<User> GetByCPF(string cpf);
        Task CreateUserClientAsync(string username, string password, int availableTime, Guid userId);
        Task UpdateUserClientAvailableTimeAsync(Guid userId, int quantityHours);
        Task UpdateUserClientCredentialsAsync(Guid userId, string username, string password);
        Task<int> GetAvailableTime(Guid userid);
    }
}
