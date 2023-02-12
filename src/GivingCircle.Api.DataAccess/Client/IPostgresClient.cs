using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace GivingCircle.Api.DataAccess.Client
{
    /// <summary>
    /// In order to standardize transactions with the database, this interface outlines the methods we can call against
    /// our repositories.
    /// </summary>
    /// <typeparam name="T">The type you will be querying against</typeparam>
    public interface IPostgresClient<T>
    {
        /// <summary>
        /// Runs queries on the database.
        /// </summary>
        /// <param name="query">The sql query</param>
        /// <param name="parameters">The parameters, if any</param>
        /// <param name="transaction">The database transaction</param>
        /// <param name="commandTimeout">The timeout</param>
        /// <returns>A list if objects returned from the database, or nothing</returns>
        Task<IEnumerable<T>> QueryAsync(string query, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);

        /// <summary>
        /// Runs a query for a single object in the database.
        /// </summary>
        /// <param name="query">The sql query</param>
        /// <param name="parameters">The parameters, if any</param>
        /// <param name="transaction">The database transaction</param>
        /// <param name="commandTimeout">The timeout</param>
        /// <returns>A single object, or nothing</returns>
        Task<T> QuerySingleAsync(string query, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);

        /// <summary>
        /// Exeutes a query against the database, eg, INSERT or DELETE.
        /// </summary>
        /// <param name="query">The sql query</param>
        /// <param name="parameters">The parameters, if any</param>
        /// <param name="transaction">The database transaction</param>
        /// <param name="commandTimeout">The timeout</param>
        /// <returns>The number of rows affected by this action</returns>
        Task<int> ExecuteAsync(string sql, object parameters, IDbTransaction transaction = null, int? commandTimeout = null);
    }
}
