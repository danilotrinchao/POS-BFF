using AuthenticationService.Core.Domain.ValueObjects;
using AuthenticationService.Domain.Entities;
using AuthenticationService.Domain.Repositories;
using Dapper;
using System.Data;

namespace AuthenticationService.Infra.Repository
{
    public class UserRepository : IRepository<User>, IUserRepository
    {
        private readonly IDbConnection _dbConnection;

        public UserRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var query = "SELECT * FROM \"User\"";
            return await _dbConnection.QueryAsync<User>(query);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM \"User\" WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { Id = id });
        }

         public async Task<User> GetByEmail(string email)
        {
            var query = "SELECT * FROM \"User\" WHERE Email = @email";
            var user = await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { email = email });
            return user;

        }

        public async Task<int> InsertAsync(User entity)
        {
            var query = @"
        INSERT INTO ""User"" (Nome, Sobrenome, DtNascimento, Email, CPF, Phone, UserType, AddressId, ""PasswordHash"", Inative)
        VALUES (@Nome, @Sobrenome, @DtNascimento, @Email, @CPF, @Phone, @UserType, @AddressId, @PasswordHash,@Inative)
        RETURNING Id";

            return await _dbConnection.ExecuteScalarAsync<int>(query, new
            {
                entity.Nome,
                entity.Sobrenome,
                entity.DtNascimento,
                entity.Email,
                entity.CPF,
                entity.Phone,
                entity.UserType,
                AddressId = entity.Address.Id,
                entity.PasswordHash,
                entity.Inative
            });
        }



        public async Task<bool> UpdateAsync(User entity)
        {
            var query = @"
                        UPDATE ""User"" SET Nome = @Nome, Sobrenome = @Sobrenome, DtNascimento = @DtNascimento,
                        Email = @Email, CPF = @CPF, Phone = @Phone, UserType = @UserType, AddressId = @AddressId,
                        PasswordHash = @PasswordHash, Inative = @Inative WHERE Id = @Id";

            var rowsAffected = await _dbConnection.ExecuteAsync(query, new
            {
                entity.Nome,
                entity.Sobrenome,
                entity.DtNascimento,
                entity.Email,
                entity.CPF,
                entity.Phone,
                entity.UserType,
                AddressId = entity.Address.Id,
                entity.PasswordHash,
                entity.Inative,
                entity.Id
            });
            return rowsAffected > 0;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var query = $"DELETE FROM \"User\" WHERE Id = @Id";
            var rowsAffected = await _dbConnection.ExecuteAsync(query, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task AddUserToRoleAsync(UserRole userRole)
        {
            var query = "INSERT INTO UserRole (UserId, RoleId) VALUES (@UserId, @RoleId)";
            await _dbConnection.ExecuteAsync(query, userRole);
        }
    }
}
