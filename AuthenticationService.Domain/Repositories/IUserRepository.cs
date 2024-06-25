using AuthenticationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmail(string email);
        Task<User> GetByCPF(string cpf);
        Task CreateUserClientAsync(string username, string password, int availableTime);
        Task UpdateUserClientAvailableTimeAsync(int userId, int quantityHours);
    }
}
