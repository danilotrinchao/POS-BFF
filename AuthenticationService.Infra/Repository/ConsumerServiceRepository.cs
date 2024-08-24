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
                            INSERT INTO ""consumerservice"" 
                            (""id"", ""userid"", ""orderid"", ""serviceid"", ""is_active"", ""totaltime"", ""servicename"")
                            VALUES (@id, @userId, @orderId, @serviceId, @is_Active, @totalTime, @serviceName)";

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
            var query = @"SELECT ""id"", ""userid"", ""orderid"", ""serviceid"", ""is_active"", ""totaltime"", ""servicename"" FROM ""consumerservice"" WHERE ""userid"" = @userid AND  ""is_active"" = true ";
            var result = await _dbConnection.QueryAsync<ConsumerService>(query, new { id = userId });
            return result;
        }
        public async Task<ConsumerService> GetConsumerServiceById(Guid id)
        {
            var query = @"SELECT * FROM ""consumerservice"" WHERE ""id"" = @id";
            var result = await _dbConnection.QuerySingleAsync<ConsumerService>(query, new { id = id });
            return result;
        }

        public async Task<bool> UpdateConsumerService(ConsumerService consumer)
        {
            var query = @"
                            UPDATE ""consumerservice"" 
                            SET 
                                ""userid"" = @userId, 
                                ""orderid"" = @orderId, 
                                ""servicename"" = @serviceName, 
                                ""is_active"" = @is_Active, 
                                ""totaltime"" = @totalTime
                            WHERE ""id"" = @id";
            var rowsAffected = await _dbConnection.ExecuteAsync(query, consumer);
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<ConsumerService>> GetActiveConsumerServicesAsync()
        {
            var query = @"
            SELECT * 
            FROM ""consumerservice"" 
            WHERE ""is_active"" = true";
            var result = await _dbConnection.QueryAsync<ConsumerService>(query);
            return result;
        }

        public async Task UpdateElapsedTimeAsync(Guid consumerId)
        {
            var query = @"
            UPDATE ""consumerservice"" 
            SET ""totaltime"" = EXTRACT(EPOCH FROM (CURRENT_TIMESTAMP - ""starttime""))
            WHERE ""id"" = @id";
            await _dbConnection.ExecuteAsync(query, new { id = consumerId });
        }
    }
}
