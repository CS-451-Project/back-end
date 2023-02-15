using Dapper;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace GivingCircle.Api.DataAccess.Client
{
    /// <summary>
    /// Postgres implementation for some basic methods for interacting with the database.
    /// </summary>
    public class PostgresClient : IPostgresClient
    {
        // The postgres client configuration
        private PostgresClientConfiguration _postgresClientConfiguration;

        /// <summary>
        /// Creates an instance of the <see cref="PostgresClient"/> class
        /// </summary>
        /// <param name="postgresClientConfiguration">The postgres client configuration</param>
        public PostgresClient(PostgresClientConfiguration postgresClientConfiguration)
        {
            _postgresClientConfiguration = postgresClientConfiguration;
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        /// <inheritdoc/>
        public async Task<int> ExecuteAsync(
            string query, 
            object parameters, 
            IDbTransaction transaction = null, 
            int? commandTimeout = null)
        {
            using (var connection = new NpgsqlConnection(_postgresClientConfiguration.ConnectionString))
            {
                return await connection.ExecuteAsync(query, parameters, transaction, commandTimeout);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<T>> QueryAsync<T>(
            string query, 
            object parameters = null, 
            IDbTransaction transaction = null, 
            int? commandTimeout = null)

            where T : class
        {
            using (var connection = new NpgsqlConnection(_postgresClientConfiguration.ConnectionString))
            {
                return await connection.QueryAsync<T>(query, parameters, transaction, commandTimeout);
            }
        }

        /// <inheritdoc/>
        public async Task<T> QuerySingleAsync<T>(
            string query, 
            object parameters = null, 
            IDbTransaction transaction = null, 
            int? commandTimeout = null) 
            
            where T : class
        {
            using (var connection = new NpgsqlConnection(_postgresClientConfiguration.ConnectionString))
            {
                return await connection.QuerySingleAsync<T>(query, parameters, transaction, commandTimeout);
            }
        }
    }
}
