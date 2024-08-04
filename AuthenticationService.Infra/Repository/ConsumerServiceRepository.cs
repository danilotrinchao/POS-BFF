using AuthenticationService.Core.Domain.Entities;
using AuthenticationService.Core.Domain.Repositories;
using Dapper;
using System.Data;

namespace AuthenticationService.Infra.Repository
{
    public class ConsumerServiceRepository : IConsumerServiceRepository
    {
        private readonly IDbConnection _dbConnection;

        public ConsumerServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Guid> CreateConsumerService(ConsumerService consumer)
        {
            var query = @"
            INSERT INTO ""ConsumerService"" (""id"", ""userId"", ""orderId"", ""serviceName"", ""Active"", ""totalTime"")
            VALUES (@id, @userId, @orderId, @serviceName, @Active, @totalTime)";
            consumer.id = Guid.NewGuid();
            await _dbConnection.ExecuteAsync(query, consumer);
            return consumer.id;
        }

        public async Task<bool> DeleteConsumerService(Guid consumerId)
        {
            var query = @"DELETE FROM ""ConsumerService"" WHERE ""id"" = @id";
            var rowsAffected = await _dbConnection.ExecuteAsync(query, new { id = consumerId });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<ConsumerService>> GetAllConsumerService()
        {
            var query = @"SELECT * FROM ""ConsumerService""";
            var result = await _dbConnection.QueryAsync<ConsumerService>(query);
            return result;
        }

        public async Task<IEnumerable<ConsumerService>> GetConsumerServiceByUserId(int userId)
        {
            var query = @"SELECT * FROM ""ConsumerService"" WHERE ""userid"" = @userid";
            var result = await _dbConnection.QueryAsync<ConsumerService>(query, new { id = userId });
            return result;
        }
        public async Task<ConsumerService> GetConsumerServiceById(Guid id)
        {
            var query = @"SELECT * FROM ""ConsumerService"" WHERE ""id"" = @id";
            var result = await _dbConnection.QuerySingleAsync<ConsumerService>(query, new { id = id });
            return result;
        }

        public async Task<bool> UpdateConsumerService(ConsumerService consumer)
        {
            var query = @"
            UPDATE ""ConsumerService"" 
            SET 
                ""userId"" = @userId, 
                ""orderId"" = @orderId, 
                ""serviceName"" = @serviceName, 
                ""Active"" = @Active, 
                ""totalTime"" = @totalTime
            WHERE ""id"" = @id";
            var rowsAffected = await _dbConnection.ExecuteAsync(query, consumer);
            return rowsAffected > 0;
        }
    }
}
