using Dapper;
using GivingCircle.Api.DataAccess.Client;
using GivingCircle.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GivingCircle.Api.DataAccess.Repositories
{
    /// <inheritdoc/>
    public class IdentityRoleRepository : IIdentityRoleRepository
    {
        private readonly PostgresClient _postgresClient;

        private readonly string _tableName = "identity_roles";

        /// <summary>
        /// Initializes an instance of the <see cref="IdentityRoleRepository"/> class
        /// </summary>
        /// <param name="postgresClient">The postgres client</param>
        public IdentityRoleRepository(PostgresClient postgresClient)
        {
            _postgresClient = postgresClient;
        }

        public async Task<bool> AddIdentityRoleAsync(IdentityRole identityRole)
        {
            // The string builder
            StringBuilder queryBuilder = new();

            // This represents the number of rows effected by our query
            int createdResult;

            // Construct the query
            var query = queryBuilder
                .Append($"INSERT INTO {_tableName} ")
                .Append("(user_id, resource_id, role ")
                .Append("VALUES (@UserId, @ResourceId, @Role)")
                .ToString();

            createdResult = await _postgresClient.ExecuteAsync(query, identityRole);

            // If we created 1 new identity role then we succeeded
            return (createdResult == 1);
        }

        public async Task<bool> DeleteIdentityRoleAsync(string userId, string resourceId)
        {
            // The query string builder
            StringBuilder queryBuilder = new();

            // The dynamic parameters to be supplied to the query
            DynamicParameters parameters = new();

            parameters.Add("@UserId", userId);
            parameters.Add("@ResourceId", resourceId);

            var query = queryBuilder
                .Append($"DELETE FROM {_tableName} ")
                .Append("WHERE user_id=@UserId AND resource_id=@ResourceId")
                .ToString();

            await _postgresClient.ExecuteAsync(query, parameters);

            return true;
        }

        public async Task<IEnumerable<IdentityRole>> GetIdentityRolesAsync(string userId)
        {
            // The identity roles to be returned
            IEnumerable<IdentityRole> identityRoles;

            // The query string builder
            StringBuilder queryBuilder = new();

            // The parameters to be given to the query
            DynamicParameters parameters = new();

            parameters.Add("@UserId", userId);

            // Construct the query
            var query = queryBuilder
                .Append($"SELECT * FROM {_tableName} ")
                .Append("WHERE user_id=@UserId ")
                .ToString();

            identityRoles = await _postgresClient.QueryAsync<IdentityRole>(query, parameters);

            return identityRoles ?? Enumerable.Empty<IdentityRole>();
        }
    }
}
