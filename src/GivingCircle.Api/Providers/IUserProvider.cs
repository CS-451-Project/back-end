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
    }
}
