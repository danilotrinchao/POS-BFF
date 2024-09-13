using POS_BFF.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Application.Contracts
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(Guid id);
        Task<Guid> CreateRoleAsync(Role role);
        Task<bool> UpdateRoleAsync(Role role);
        Task<bool> DeleteRoleAsync(Guid id);
    }
}
