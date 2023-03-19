using GivingCircle.Api.DataAccess.Repositories;
using GivingCircle.Api.DataAccess.Responses;
using GivingCircle.Api.Models;
using System;
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

        public async Task<GetUserResponse> GetUserAsync(string userId)
        {
            GetUserResponse user;

            // Validate the user id
            Guid.Parse(userId);

            user = await _userRepository.GetUserAsync(userId);

            return user;
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
