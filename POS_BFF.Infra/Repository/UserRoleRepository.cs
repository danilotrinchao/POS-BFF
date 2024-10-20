using POS_BFF.Core.Domain.Repositories;
using POS_BFF.Core.Domain.ValueObjects;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlTypes;

namespace POS_BFF.Infra.Repository
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly IDbConnection _dbConnection;

        public UserRoleRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task InsertAsync(Guid userId , Guid roleId)
        {
            var userRole = new UserRole();
            userRole.UserId = userId;
            userRole.RoleId = roleId;
            var query = @"INSERT INTO ""UserRole"" (""UserId"", ""RoleId"") VALUES (@userId, @roleId)";
            await _dbConnection.ExecuteAsync(query, userRole);
        }

        public async Task<List<UserRole>> GetByUserIdAsync(Guid userId)
        {
            var query = @"SELECT * FROM ""UserRole"" WHERE ""UserId"" = @UserId";
            var result =  await _dbConnection.QueryAsync<UserRole>(query, new { UserId = userId });
            return result.ToList();
        }

        public async Task<bool> DeleteAsync(Guid userId, Guid roleId)
        {
            var query = "DELETE FROM \"UserRole\" WHERE \"UserId\" = @UserId AND \"RoleId\" = @RoleId";
            var rowsAffected = await _dbConnection.ExecuteAsync(query, new { UserId = userId, RoleId = roleId });
            return rowsAffected > 0;
        }
    }

}
