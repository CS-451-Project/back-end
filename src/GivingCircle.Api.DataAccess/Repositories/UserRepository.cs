using Dapper;
using GivingCircle.Api.DataAccess.Client;
using GivingCircle.Api.DataAccess.Responses;
using GivingCircle.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GivingCircle.Api.DataAccess.Repositories
{
    /// <inheritdoc/>
    public class UserRepository : IUserRepository
    {
        private readonly PostgresClient _postgresClient;

        /// <summary>
        /// Initializes an instance of the <see cref="UserRepository"/> class
        /// </summary>
        /// <param name="postgresClient">The postgres client</param>
        public UserRepository(PostgresClient postgresClient)
        {
            _postgresClient = postgresClient;
        }

        public async Task<GetUserResponse> GetUserAsync(string userId)
        {
            // Object to map the parameters to the query
            object parameters = new { UserId = userId };

            GetUserResponse user = await _postgresClient.QuerySingleAsync<GetUserResponse>("SELECT * FROM users WHERE user_id = @UserId", parameters);

            return user;
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            // The string builder
            StringBuilder queryBuilder = new();

            // This represents the number of rows effected by our query
            int createdResult;

            // Construct the query
            var query = queryBuilder
                .Append($"INSERT INTO users ")
                .Append("(user_id, first_name, middle_initial, last_name, password, email) ")
                .Append("VALUES (@UserId, @FirstName, @MiddleInitial, @LastName, @Password, @email) ")
                .ToString();

            createdResult = await _postgresClient.ExecuteAsync(query, user);

            // If we created 1 new user then we succeeded
            return (createdResult == 1);
        }

        public async Task<bool> UpdateUserAsync(string userId, User user)
        {
            // The string builder
            StringBuilder queryBuilder = new();

            // This represents the number of rows effected by our query
            int updatedResult;

            // The dynamic parameters bag
            Dictionary<string, object> parametersDictionary = new()
            {
                { "@FirstName",  user.FirstName},
                { "@MiddleInitial",  user.MiddleInitial},
                { "@LastName", user.LastName},
                { "@Password", user.Password },
                { "@UserId", userId }
            };

            // The query parameters
            DynamicParameters parameters = new(parametersDictionary);

            // Build the query
            var query = queryBuilder
                .Append($"UPDATE users ")
                .Append("SET first_name = @FirstName, ")
                .Append("middle_initial = @MiddleInitial, ")
                .Append("last_name = @LastName, ")
                .Append("password = @Password ")
                .Append("WHERE user_id = @UserId ")
                .ToString();

            updatedResult = await _postgresClient.ExecuteAsync(query, parameters);

            // If we created 1 new fundraiser then we succeeded
            return (updatedResult == 1);
        }
        public async Task<bool> DeleteUserAsync(string userId)
        {
            StringBuilder query = new StringBuilder();

            // Object to map the parameters to the query
            object parameters = new { User_Id = userId };

            // Will return 1 if successful
            var deleteUser = await _postgresClient.ExecuteAsync(query
                .Append("DELETE FROM users ")
                .Append("WHERE user_id = @User_Id ").ToString(),
                parameters);

            return deleteUser == 1 ? true : false;
        }

        public async Task<string> ValidateUserAsync(string email, string password)
        {
            // Object to map the parameters to the query
            object parameters = new { Email = email, Password = password };

            // Will return the user id if successful
            var userId = await _postgresClient.QuerySingleAsync<string>("SELECT u.user_id FROM users u WHERE email = @Email AND password = @Password", parameters);

            // Return the id if they exist or else null
            return userId ?? null;

        }

        public async Task<GetUserResponse> GetUserByEmailAsync(string email)
        {
            // The fundraiser to be returned
            GetUserResponse user;

            // The query string builder
            StringBuilder queryBuilder = new();

            // The parameters to be given to the query
            DynamicParameters parameters = new();

            parameters.Add("@Email", email);

            // Construct the query
            var query = queryBuilder
                .Append($"SELECT * FROM users ")
                .Append("WHERE email=@Email ")
                .ToString();

            user = await _postgresClient.QuerySingleAsync<GetUserResponse>(query, parameters);

            return user ?? null;
        }
    }


}

