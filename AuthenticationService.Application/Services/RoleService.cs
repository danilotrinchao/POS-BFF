using AuthenticationService.Application.Contracts;
using AuthenticationService.Domain.Entities;
using AuthenticationService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<Role> GetRoleByIdAsync(Guid id)
        {
            return await _roleRepository.GetByIdAsync(id);
        }

        public async Task<Guid> CreateRoleAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            role.Id = Guid.NewGuid();
            return await _roleRepository.InsertAsync(role);
        }

        public async Task<bool> UpdateRoleAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            var existingRole = await _roleRepository.GetByIdAsync(role.Id);
            if (existingRole == null)
                return false;

            existingRole.Name = role.Name;

            return await _roleRepository.UpdateAsync(existingRole);
        }

        public async Task<bool> DeleteRoleAsync(Guid id)
        {
            var existingRole = await _roleRepository.GetByIdAsync(id);
            if (existingRole == null)
                return false;

            return await _roleRepository.DeleteAsync(id);
        }
    }
}
