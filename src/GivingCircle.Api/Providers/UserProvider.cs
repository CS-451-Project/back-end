using GivingCircle.Api.DataAccess.Repositories;
using GivingCircle.Api.DataAccess.Responses;
using System.Threading.Tasks;

namespace GivingCircle.Api.Providers
{
    /// <inheritdoc/>
    public class UserProvider : IUserProvider
    {
        private readonly IUserRepository _userRepository;

        public UserProvider(IUserRepository userRepository) 
        { 
            _userRepository= userRepository;
        }

        public async Task<GetUserResponse> GetUserByEmailAsync(string email)
        {
            // The user we're trying to get
            GetUserResponse user;

            user = await _userRepository.GetUserByEmailAsync(email);

            return user ?? null;
        }

        public async Task<string> ValidateUserAsync(string email, string password)
        {
            // The user's id, if they exist
            string userId;

            userId = await _userRepository.ValidateUserAsync(email, password);

            return userId ?? null;
        }
    }
}
