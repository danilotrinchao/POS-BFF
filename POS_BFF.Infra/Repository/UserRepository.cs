using POS_BFF.Core.Domain.ValueObjects;
using POS_BFF.Domain.Entities;
using POS_BFF.Domain.Repositories;
using Dapper;
using Serilog;
using System.Data;

namespace POS_BFF.Infra.Repository
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

        public async Task<User> GetByIdAsync(Guid id)
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
        public async Task<User> GetByCPF(string cpf)
        {
            var query = @"SELECT * FROM ""User"" WHERE Cpf = @cpf";
            var user = await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { CPF = cpf });
            return user;

        }
        public async Task<Guid> InsertAsync(User entity)
        {
            var query = @"
                        INSERT INTO ""User"" (Nome, Sobrenome, DtNascimento, Email, CPF, Phone, UserType, AddressId, ""PasswordHash"", Inative)
                        VALUES (@Nome, @Sobrenome, @DtNascimento, @Email, @CPF, @Phone, @UserType, @AddressId, @PasswordHash,@Inative)
                        RETURNING Id";

            return await _dbConnection.ExecuteScalarAsync<Guid>(query, new
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
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Insert or update the address first
                    var addressQuery = @"
                                           INSERT INTO public.""address"" (""id"", ""zipcode"", ""cityname"", ""state"", ""road"", ""number"")
                                           VALUES (@Id, @ZipCode, @CityName, @State, @Road, @Number)
                                           ON CONFLICT (""id"") DO UPDATE
                                           SET ""zipcode"" = EXCLUDED.""zipcode"",
                                               ""cityname"" = EXCLUDED.""cityname"",
                                               ""state"" = EXCLUDED.""state"",
                                               ""road"" = EXCLUDED.""road"",
                                               ""number"" = EXCLUDED.""number""
                                           RETURNING ""id"";
                                       ";

                    var addressId = await _dbConnection.ExecuteScalarAsync<int>(addressQuery, new
                    {
                        Id = entity.Address.Id,
                        entity.Address.ZipCode,
                        entity.Address.CityName,
                        entity.Address.State,
                        entity.Address.Road,
                        entity.Address.Number
                    }, transaction);

                    // Ensure that the address ID is valid
                    // if (addressId <= 0)
                    // {
                    //     transaction.Rollback();
                    //     return false;
                    // }

                    // Update the user record
                    var userQuery = @"
                UPDATE public.""User""
                SET ""nome"" = @Nome,
                    ""sobrenome"" = @Sobrenome,
                    ""dtnascimento"" = @DtNascimento,
                    ""email"" = @Email,
                    ""cpf"" = @CPF,
                    ""phone"" = @Phone,
                    ""usertype"" = @UserType,
                    ""addressid"" = @AddressId,
                    ""PasswordHash"" = @PasswordHash,
                    ""inative"" = @Inative
                WHERE ""id"" = @Id";

                    var rowsAffected = await _dbConnection.ExecuteAsync(userQuery, new
                    {
                        entity.Nome,
                        entity.Sobrenome,
                        entity.DtNascimento,
                        entity.Email,
                        entity.CPF,
                        entity.Phone,
                        entity.UserType,
                        AddressId = addressId,
                        entity.PasswordHash,
                        entity.Inative,
                        entity.Id
                    }, transaction);

                    transaction.Commit();
                    return rowsAffected > 0;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }




        public async Task<bool> DeleteAsync(Guid id)
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

        public async Task CreateUserClientAsync(string username, string password, int availableTime, Guid userId)
        {
            string sql = @"
                            INSERT INTO public.""UserClient""
                            (
                                ""Username"", 
                                ""Password"", 
                                ""AvailableTime"",
                                ""UserId""
                            )
                            VALUES
                            (@Username, @Password, @AvailableTime, @UserId);";

            await _dbConnection.ExecuteAsync(sql, new { Username = username, Password = password, AvailableTime = availableTime, UserId = userId });

        }

        public async Task UpdateUserClientAvailableTimeAsync(Guid userId, int quantityHours)
        {
            string sql = @"
                            UPDATE public.""UserClient""
                            SET ""AvailableTime"" = ""AvailableTime"" + @QuantityHours
                            WHERE ""UserId"" = @UserId;";

            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }

            try
            {
                Log.Information("Atualizando o tempo disponível do usuário {UserId} em {QuantityHours} horas.", userId, quantityHours);
                int rowsAffected = await _dbConnection.ExecuteAsync(sql, new { UserId = userId, QuantityHours = quantityHours });
                Log.Information("Atualização concluída. Linhas afetadas: {RowsAffected}", rowsAffected);
            }
            catch (Exception ex)
            {
                Log.Error("Erro ao atualizar o tempo disponível do usuário {UserId}: {ExceptionMessage}", userId, ex.Message);
                throw;
            }
        }

        public async Task UpdateUserClientCredentialsAsync(Guid userId, string username, string password)
        {
            string sql = @"
                           UPDATE public.""UserClient""
                           SET ""Username"" = @Username,
                               ""Password"" = @Password
                           WHERE ""UserId"" = @UserId;";

            if (_dbConnection.State != ConnectionState.Open)
            {
                 _dbConnection.Open();
            }

            try
            {
                Log.Information("Atualizando credenciais do usuário {UserId}. Novo Username: {Username}", userId, username);

                // Execute a query de atualização
                int rowsAffected = await _dbConnection.ExecuteAsync(sql, new { UserId = userId, Username = username, Password = password });

                Log.Information("Atualização concluída. Linhas afetadas: {RowsAffected}", rowsAffected);
            }
            catch (Exception ex)
            {
                Log.Error("Erro ao atualizar as credenciais do usuário {UserId}: {ExceptionMessage}", userId, ex.Message);
                throw;
            }
        }

        public async Task<int> GetAvailableTime(Guid userid)
        {
           var query = @"SELECT ""AvailableTime"" FROM public.""UserClient"" WHERE ""userid"" = @userid";
           return  _dbConnection.QuerySingle<int>(query, new { id = userid });
        }



    }
}
