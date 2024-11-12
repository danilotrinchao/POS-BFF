using POS_BFF.Application.DTOs;
using POS_BFF.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Application.Contracts
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync(Guid tenantId);
        Task<User> GetUserByIdAsync(Guid id, Guid tenantId);
        Task<Guid> CreateUserAsync(UserDto user, Guid tenantId);
        Task<bool> UpdateUserAsync(UserDto userDto, Guid tenantId);
        Task<bool> DeleteUserAsync(Guid id, Guid tenantId);
        Task<User> GetUserByCPF(string cpf, Guid tenantId);
    }
}
