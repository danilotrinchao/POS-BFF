using Dapper;
using System.Data;

namespace AuthenticationService.Infra.Repository
{
    public class Repository<TEntity> where TEntity : class
    {
        private readonly IDbConnection _dbConnection;

        public Repository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public TEntity GetById(int id)
        {
            return _dbConnection.QueryFirstOrDefault<TEntity>($"SELECT * FROM {typeof(TEntity).Name} WHERE Id = @Id", new { Id = id });
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _dbConnection.Query<TEntity>($"SELECT * FROM {typeof(TEntity).Name}");
        }

        public void Insert(TEntity entity)
        {
            _dbConnection.Execute(GenerateInsertQuery(), entity);
        }

        public void Update(TEntity entity)
        {
            _dbConnection.Execute(GenerateUpdateQuery(), entity);
        }

        public void Delete(int id)
        {
            _dbConnection.Execute($"DELETE FROM {typeof(TEntity).Name} WHERE Id = @Id", new { Id = id });
        }

        private string GenerateInsertQuery()
        {
            var properties = typeof(TEntity).GetProperties().Where(p => p.Name != "Id");
            var columns = string.Join(",", properties.Select(p => p.Name));
            var values = string.Join(",", properties.Select(p => "@" + p.Name));
            return $"INSERT INTO {typeof(TEntity).Name} ({columns}) VALUES ({values})";
        }

        private string GenerateUpdateQuery()
        {
            var properties = typeof(TEntity).GetProperties().Where(p => p.Name != "Id");
            var columns = string.Join(",", properties.Select(p => $"{p.Name} = @{p.Name}"));
            return $"UPDATE {typeof(TEntity).Name} SET {columns} WHERE Id = @Id";
        }
    }
}
