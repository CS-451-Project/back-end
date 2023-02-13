using Dapper;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace GivingCircle.Api.DataAccess.Client
{
    /// <inheritdoc/>
    public class PostgresClient<T> : IPostgresClient<T>
    {
        // The postgres client configuration
        private PostgresClientConfiguration _postgresClientConfiguration;

        /// <summary>
        /// Creates an instance of the <see cref="PostgresClient{T}"/> class
        /// </summary>
        /// <param name="postgresClientConfiguration">The postgres client configuration</param>
        public PostgresClient(PostgresClientConfiguration postgresClientConfiguration)
        {
            _postgresClientConfiguration = postgresClientConfiguration;
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public async Task<int> ExecuteAsync(string query, object parameters, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            using (var connection = new NpgsqlConnection(_postgresClientConfiguration.ConnectionString))
            {
                return await connection.ExecuteAsync(query, parameters, transaction, commandTimeout);
            }
        }

        public async Task<IEnumerable<T>> QueryAsync(string query, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            using (var connection = new NpgsqlConnection(_postgresClientConfiguration.ConnectionString))
            {
                return await connection.QueryAsync<T>(query, parameters, transaction, commandTimeout);
            }
        }

        public async Task<T> QuerySingleAsync(string query, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            using (var connection = new NpgsqlConnection(_postgresClientConfiguration.ConnectionString))
            {
                return await connection.QuerySingleAsync<T>(query, parameters, transaction, commandTimeout);
            }
        }
    }
}
