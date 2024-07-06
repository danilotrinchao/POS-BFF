using AuthenticationService.Domain.Entities;
using AuthenticationService.Domain.Repositories;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AuthenticationService.Infra.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IDbConnection _dbConnection;

        public RoleRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            var query = "SELECT * FROM \"Role\"";
            return await _dbConnection.QueryAsync<Role>(query);
        }

        public async Task<Role> GetByIdAsync(Guid id)
        {
            var query = "SELECT * FROM \"Role\" WHERE \"Id\" = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<Role>(query, new { Id = id });
        }
        public async Task<Role> GetByGroupAsync(int group)
        {
            var query = @"SELECT * FROM ""Role"" WHERE ""group"" = @Group";
            return await _dbConnection.QueryFirstOrDefaultAsync<Role>(query, new { Group = group });
        }
        public async Task<Guid> InsertAsync(Role entity)
        {
            var query = @"INSERT INTO ""Role"" (""Id"", ""Name"", ""Description"", ""Group"", ""Inative"") VALUES (@Id, @Name, @Description, @Group, @Inative)";

            entity.Id = Guid.NewGuid();
            return await _dbConnection.ExecuteScalarAsync<Guid>(query, new
            {
                entity.Id,
                entity.Name,
                entity.Description,
                entity.Group,
                entity.Inative
            });
        }

        public async Task<bool> UpdateAsync(Role entity)
        {
            var query = @"UPDATE ""Role"" SET ""Name"" = @Name, ""Description"" = @Description, ""Group"" = @Group, ""Inative"" = @Inative WHERE ""Id"" = @Id";

            var rowsAffected = await _dbConnection.ExecuteAsync(query, new
            {
                entity.Name,
                entity.Description,
                entity.Group,
                entity.Inative,
                entity.Id
            });
            return rowsAffected > 0;
        }


        public async Task<bool> DeleteAsync(Guid id)
        {
            var query = "DELETE FROM \"Role\" WHERE \"Id\" = @Id";
            var rowsAffected = await _dbConnection.ExecuteAsync(query, new { Id = id });
            return rowsAffected > 0;
        }
    }
}
