using GivingCircle.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingCircle.Api.Providers
{
    public interface IIdentityRoleProvider
    {
        /// <summary>
        /// Retrieves the identity roles for a user
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns>A list of identity roles, if any</returns>
        Task<IEnumerable<IdentityRole>> GetIdentityRoles(string userId);

        /// <summary>
        /// Adds an identity role for a user
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <param name="resourceId">The resource id</param>
        /// <param name="roleName">The role name</param>
        /// <returns>The result, true if success</returns>
        Task<bool> AddIdentityRole(string userId, string resourceId, string roleName);

        /// <summary>
        /// Deletes an identity role for a user
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <param name="resourceId">The resource id</param>
        /// <returns>The result, true if success</returns>
        Task<bool> DeleteIdentityRole(string userId, string resourceId);
    }
}
