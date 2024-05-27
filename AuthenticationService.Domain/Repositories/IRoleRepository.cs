using AuthenticationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Domain.Repositories
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role> GetByIdAsync(Guid id);
        Task<Guid> InsertAsync(Role entity);
        Task<bool> UpdateAsync(Role entity);
        Task<bool> DeleteAsync(Guid id);
    }
}
