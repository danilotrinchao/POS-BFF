using AuthenticationService.Core.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Core.Domain.Repositories
{
    public interface IUserRoleRepository
    {
        Task InsertAsync(int userId, Guid roleId);
        Task<List<UserRole>> GetByUserIdAsync(int userId);
        Task<bool> DeleteAsync(int userId, Guid roleId);
    }
}
