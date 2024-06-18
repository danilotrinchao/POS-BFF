using AuthenticationService.Core.Domain.Repositories;
using AuthenticationService.Core.Domain.ValueObjects;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Infra.Repository
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly IDbConnection _dbConnection;

        public UserRoleRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task InsertAsync(int userId , Guid roleId)
        {
            var userRole = new UserRole();
            userRole.UserId = userId;
            userRole.RoleId = roleId;
            var query = @"INSERT INTO ""UserRole"" (""UserId"", ""RoleId"") VALUES (@UserId, @RoleId)";
            await _dbConnection.ExecuteAsync(query, userRole);
        }

        public async Task<List<UserRole>> GetByUserIdAsync(int userId)
        {
            var query = @"SELECT * FROM ""UserRole"" WHERE ""UserId"" = @UserId";
            var result =  await _dbConnection.QueryAsync<UserRole>(query, new { UserId = userId });
            return result.ToList();
        }

        public async Task<bool> DeleteAsync(int userId, Guid roleId)
        {
            var query = "DELETE FROM \"UserRole\" WHERE \"UserId\" = @UserId AND \"RoleId\" = @RoleId";
            var rowsAffected = await _dbConnection.ExecuteAsync(query, new { UserId = userId, RoleId = roleId });
            return rowsAffected > 0;
        }
    }

}
