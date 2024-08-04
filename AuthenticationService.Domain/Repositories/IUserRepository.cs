using AuthenticationService.Domain.Entities;

namespace AuthenticationService.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmail(string email);
        Task<User> GetByCPF(string cpf);
        Task CreateUserClientAsync(string username, string password, int availableTime, int userId);
        Task UpdateUserClientAvailableTimeAsync(int userId, int quantityHours);
        Task UpdateUserClientCredentialsAsync(int userId, string username, string password);
        Task<int> GetAvailableTime(int userid);
    }
}
