using GivingCircle.Api.DataAccess.Repositories;
using GivingCircle.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GivingCircle.Api.Providers
{
    /// <inheritdoc/>
    public class IdentityRoleProvider : IIdentityRoleProvider
    {
        private readonly IIdentityRoleRepository _identityRoleRepository;

        /// <summary>
        /// Provides logic around the <see cref="IdentityRole"/> resource
        /// </summary>
        /// <param name="identityRoleRepository">The identity role repository</param>
        public IdentityRoleProvider(IIdentityRoleRepository identityRoleRepository) 
        {
            _identityRoleRepository = identityRoleRepository;
        }
        
        public async Task<bool> AddIdentityRole(string userId, string resourceId, string roleName)
        {
            bool result = false;

            IdentityRole identityRole = new()
            {
                UserId = userId,
                ResourceId = resourceId,
                Role = roleName
            };

            result = await _identityRoleRepository.AddIdentityRoleAsync(identityRole);

            return result;
        }

        public async Task<bool> DeleteIdentityRole(string userId, string resourceId)
        {
            bool result = false;

            result = await _identityRoleRepository.DeleteIdentityRoleAsync(userId, resourceId);

            return result;
        }

        public async Task<IEnumerable<IdentityRole>> GetIdentityRoles(string userId)
        {
            IEnumerable<IdentityRole> identityRoles;

            identityRoles = await _identityRoleRepository.GetIdentityRolesAsync(userId);

            return identityRoles ?? Enumerable.Empty<IdentityRole>();
        }
    }
}
