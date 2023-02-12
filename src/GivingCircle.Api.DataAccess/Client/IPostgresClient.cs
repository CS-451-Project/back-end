using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace GivingCircle.Api.DataAccess.Client
{
    public interface IPostgresClient<T>
    {
        Task<IEnumerable<T>> QueryAsync(string query, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);

        Task<T> QuerySingleAsync(string query, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null);

        Task<int> ExecuteAsync(string sql, object parameters, IDbTransaction transaction = null, int? commandTimeout = null);
    }
}
