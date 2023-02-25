using GivingCircle.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingCircle.Api.DataAccess.Repositories
{
    public interface IIdentityRoleRepository
    {
        /// <summary>
        /// Retrieves identity roles for a user
        /// </summary>
        /// <param name="userId" <see cref="IdentityRole"/>>The user's id</param>
        /// <returns>A list of identity roles, if any</returns>
        Task<IEnumerable<IdentityRole>> GetIdentityRolesAsync(string userId);

        /// <summary>
        /// Inserts an identity role for user
        /// </summary>
        /// <param name="identityRole" <see cref="IdentityRole"/>>The identity role</param>
        /// <returns>The result, true if success</returns>
        Task<bool> AddIdentityRoleAsync(IdentityRole identityRole);

        /// <summary>
        /// Deletes an identity role for a user
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <param name="resourceId">The resource's id</param>
        /// <returns>The result, true if success</returns>
        Task<bool> DeleteIdentityRoleAsync(string userId, string resourceId);
    }
}
