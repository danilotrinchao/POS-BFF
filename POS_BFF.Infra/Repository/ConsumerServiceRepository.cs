using POS_BFF.Core.Domain.Entities;
using POS_BFF.Core.Domain.Repositories;
using Dapper;
using System.Data;
using Npgsql;
using POS_BFF.Infra.Context;

namespace POS_BFF.Infra.Repository
{
    public class ConsumerServiceRepository : IConsumerServiceRepository
    {
        private IDbConnection CreateConnection()
        {
            // Função para criar a conexão com a string de conexão apropriada para o tenant
            var connectionString = TenantContext.GetConnectionString();
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Connection string is missing.");
            }
            return new NpgsqlConnection(connectionString);
        }

        public async Task<Guid> CreateConsumerService(ConsumerService consumer)
        {
            using (var connection = CreateConnection())
            {
                var schema = TenantContext.GetSchema();  // Recupera o schema do TenantContext
                connection.Open();

                var query = $@"
                INSERT INTO ""{schema}"".""consumerservice"" 
                (""id"", ""userid"", ""orderid"", ""serviceid"", ""is_active"", ""totaltime"", ""servicename"")
                VALUES (@id, @userId, @orderId, @serviceId, @is_Active, @totalTime, @serviceName)";

                consumer.id = Guid.NewGuid();
                await connection.ExecuteAsync(query, consumer);
                return consumer.id;
            }
        }

        public async Task<bool> DeleteConsumerService(Guid consumerId)
        {
            using (var connection = CreateConnection())
            {
                var schema = TenantContext.GetSchema();  // Recupera o schema do TenantContext
                connection.Open();

                var query = $@"DELETE FROM ""{schema}"".""consumerservice"" WHERE ""id"" = @id";
                var rowsAffected = await connection.ExecuteAsync(query, new { id = consumerId });
                return rowsAffected > 0;
            }
        }

        public async Task<IEnumerable<ConsumerService>> GetAllConsumerService()
        {
            using (var connection = CreateConnection())
            {
                var schema = TenantContext.GetSchema();  // Recupera o schema do TenantContext
                connection.Open();

                var query = $@"SELECT * FROM ""{schema}"".""consumerservice""";
                var result = await connection.QueryAsync<ConsumerService>(query);
                return result;
            }
        }

        public async Task<IEnumerable<ConsumerService>> GetConsumerServiceByUserId(int userId)
        {
            using (var connection = CreateConnection())
            {
                var schema = TenantContext.GetSchema();  // Recupera o schema do TenantContext
                connection.Open();

                var query = $@"SELECT ""id"", ""userid"", ""orderid"", ""serviceid"", ""is_active"", ""totaltime"", ""servicename"" 
                           FROM ""{schema}"".""consumerservice"" 
                           WHERE ""userid"" = @userid";

                var result = await connection.QueryAsync<ConsumerService>(query, new { userid = userId });
                return result;
            }
        }

        public async Task<ConsumerService> GetConsumerServiceById(Guid id)
        {
            using (var connection = CreateConnection())
            {
                var schema = TenantContext.GetSchema();  // Recupera o schema do TenantContext
                connection.Open();

                var query = $@"SELECT * FROM ""{schema}"".""consumerservice"" WHERE ""id"" = @id";
                var result = await connection.QuerySingleAsync<ConsumerService>(query, new { id = id });
                return result;
            }
        }

        public async Task<bool> UpdateConsumerService(ConsumerService consumer)
        {
            using (var connection = CreateConnection())
            {
                var schema = TenantContext.GetSchema();  // Recupera o schema do TenantContext
                connection.Open();

                var query = $@"
                UPDATE ""{schema}"".""consumerservice"" 
                SET 
                    ""userid"" = @userId, 
                    ""orderid"" = @orderId, 
                    ""servicename"" = @serviceName, 
                    ""is_active"" = @is_Active, 
                    ""totaltime"" = @totalTime
                WHERE ""id"" = @id";

                var rowsAffected = await connection.ExecuteAsync(query, consumer);
                return rowsAffected > 0;
            }
        }

        public async Task<IEnumerable<ConsumerService>> GetActiveConsumerServicesAsync()
        {
            using (var connection = CreateConnection())
            {
                var schema = TenantContext.GetSchema();  // Recupera o schema do TenantContext
                connection.Open();

                var query = $@"SELECT * 
                           FROM ""{schema}"".""consumerservice"" 
                           WHERE ""is_active"" = true";
                var result = await connection.QueryAsync<ConsumerService>(query);
                return result;
            }
        }

        public async Task UpdateElapsedTimeAsync(Guid consumerId)
        {
            using (var connection = CreateConnection())
            {
                var schema = TenantContext.GetSchema();  // Recupera o schema do TenantContext
                connection.Open();

                var query = $@"
            UPDATE ""{schema}"".""consumerservice"" 
            SET ""totaltime"" = EXTRACT(EPOCH FROM (CURRENT_TIMESTAMP - ""starttime""))
            WHERE ""id"" = @id";
                await connection.ExecuteAsync(query, new { id = consumerId });
            }
        }
    }

}
