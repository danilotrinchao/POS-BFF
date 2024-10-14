using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_BFF.Infra.Context
{
    public static class TenantContext
    {
        private static AsyncLocal<string> _connectionString = new AsyncLocal<string>();
        private static AsyncLocal<string> _schema = new AsyncLocal<string>();

        public static string GetConnectionString() => _connectionString.Value;
        public static void SetConnectionString(string connectionString) => _connectionString.Value = connectionString;

        public static string GetSchema() => _schema.Value ?? "default_schema"; // Fallback para um schema padrão
        public static void SetSchema(string schema)
        {
            // Validação simples para evitar SQL Injection
            if (IsValidSchema(schema))
            {
                _schema.Value = schema;
            }
            else
            {
                throw new ArgumentException("Invalid schema.");
            }
        }

        private static bool IsValidSchema(string schema)
        {
            // Implementar lógica de validação, como verificar se o schema existe
            return !string.IsNullOrWhiteSpace(schema); // Exemplo básico
        }
    }


}
