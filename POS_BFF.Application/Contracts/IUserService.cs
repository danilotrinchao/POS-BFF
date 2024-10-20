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
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(Guid id);
        Task<Guid> CreateUserAsync(UserDto user);
        Task<bool> UpdateUserAsync(UserDto userDto);
        Task<bool> DeleteUserAsync(Guid id);
        Task<User> GetUserByCPF(string cpf);
    }
}
