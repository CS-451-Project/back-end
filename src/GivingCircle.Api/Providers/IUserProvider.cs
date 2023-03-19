using GivingCircle.Api.DataAccess.Responses;
using System.Threading.Tasks;

namespace GivingCircle.Api.Providers
{
    public interface IUserProvider
    {
        /// <summary>
        /// Validates a user
        /// </summary>
        /// <param name="email">The users email</param>
        /// <param name="password">The user's password</param>
        /// <returns>The user's id if authentication success</returns>
        Task<string> ValidateUserAsync(string email, string password);

        /// <summary>
        /// Gets a user by their id
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns>A get user response</returns>
        Task<GetUserResponse> GetUserAsync(string userId);
    }
}
