﻿using GivingCircle.Api.DataAccess.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingCircle.Api.DataAccess.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Gets a user by their id
        /// </summary>
        /// <param name="userId">The users id</param>
        /// <returns>the user object</returns>
        Task<GetUserResponse> GetUserAsync(string userId);

        /// <summary>
        /// Creates a user
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>True if success, false or an error if failure</returns>
        Task<bool> CreateUserAsync(Models.User user);


        /// <summary>
        /// Updates the user using the given object
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <param name="user">The user to update</param>
        /// <returns>True if success, false or an error if un successful</returns>
        Task<bool> UpdateUserAsync(string userId, Models.User user);

        /// <summary>
        /// Permanently deletes a user
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns>True if success, else false if an error</returns>
        Task<bool> DeleteUserAsync(string userId);

        /// <summary>
        /// Validates User
        /// </summary>
        /// <param name="email">The users email</param>
        /// <param name="password"> The users password</param>
        /// <returns>True if validated, else false</returns>
        Task<bool> ValidateUserAsync(string email, string password);


    }
}
