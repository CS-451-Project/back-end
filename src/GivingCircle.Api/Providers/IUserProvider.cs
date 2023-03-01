using GivingCircle.Api.DataAccess.Responses;
using System.Threading.Tasks;

namespace GivingCircle.Api.Providers
{
    public interface IUserProvider
    {
        /// <summary>
        /// Gets the user by email
        /// </summary>
        /// <param name="email">The user's email</param>
        /// <returns>A user if they exist, null if they don't</returns>
        Task<GetUserResponse> GetUserByEmailAsync(string email);
    }
}
