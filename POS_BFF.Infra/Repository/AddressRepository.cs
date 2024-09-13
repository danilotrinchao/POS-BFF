using POS_BFF.Domain.Repositories;
using POS_BFF.Domain.ValueObjects;
using Dapper;
using System.Data;
using System.Data.Common;

namespace POS_BFF.Infra.Repository
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        private readonly IDbConnection _dbConnection;
        public AddressRepository(IDbConnection dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection;
        }


        public async Task<IEnumerable<Address>> GetAllAsync()
        {
            var query = "SELECT * FROM Address";
            return await _dbConnection.QueryAsync<Address>(query);
        }

        public async Task<Address> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM Address WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<Address>(query, new { Id = id });
        }

        public async Task<int> InsertAsync(Address entity)
        {
            var query = @"INSERT INTO Address (ZipCode, CityName, State, Road, Number)
                      VALUES (@ZipCode, @CityName, @State, @Road, @Number)
                      RETURNING Id";
            return await _dbConnection.ExecuteScalarAsync<int>(query, entity);
        }

        public async Task<bool> UpdateAsync(Address entity)
        {
            var query = @"UPDATE ""address"" SET zipcode = @ZipCode, cityname = @CityName,
                      state = @State, road = @Road, number = @Number
                      WHERE ""id"" = @Id";
            var rowsAffected = await _dbConnection.ExecuteAsync(query, entity);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var query = "DELETE FROM Address WHERE Id = @Id";
            var rowsAffected = await _dbConnection.ExecuteAsync(query, new { Id = id });
            return rowsAffected > 0;
        }
    }
}
