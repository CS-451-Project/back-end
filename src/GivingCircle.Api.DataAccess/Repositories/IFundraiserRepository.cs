using GivingCircle.Api.DataAccess.Responses;
using GivingCircle.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingCircle.Api.DataAccess.Repositories
{
    public interface IFundraiserRepository
    {
        /// <summary>
        /// Increments the amount column for a fundraiser
        /// </summary>
        /// <param name="fundraiserId">The fundraiser's id</param>
        /// <param name="amount">The amount to add</param>
        /// <returns>True if success, else failure</returns>
        Task<bool> MakeDonation(string fundraiserId, double amount);

        /// <summary>
        /// Gets a fundraiser by its id
        /// </summary>
        /// <param name="fundraiserId">The users id</param>
        /// <returns>A list of fundraisers, if any</returns>
        Task<GetFundraiserResponse> GetFundraiserAsync(string fundraiserId);

        /// <summary>
        /// Gets a list of fundraisers associated with a user's id
        /// </summary>
        /// <param name="userId">The users id</param>
        /// <returns>A list of fundraisers, if any</returns>
        Task<IEnumerable<GetFundraiserResponse>> ListFundraisersByUserIdAsync(string userId);

        /// <summary>
        /// Sorts and filters fundraisers based on various criteria
        /// 
        /// Filter properties are title, tags, created date, end date
        /// 
        /// Order by properties are title, created date, planned end date, and by
        /// how close to the target goal the fundraiser is
        /// </summary>
        /// <param name="filterProps">The filter props</param>
        /// <returns>A list of fundraisers sorted and filtered by the given criteria</returns>
        Task<IEnumerable<GetFundraiserResponse>> FilterFundraisersAsync(Dictionary<string, string[]> filterProps);

        /// <summary>
        /// Creates a fundraiser
        /// </summary>
        /// <param name="fundraiser">The given fundraiser</param>
        /// <returns>True if success, false or an error if failure</returns>
        Task<bool> CreateFundraiserAsync(string userId, Fundraiser fundraiser);

        /// <summary>
        /// Updates the fundraiser using the given object
        /// </summary>
        /// <param name="fundraiserId">The fundraiser's id</param>
        /// <param name="fundraiser">The fundraiser to update to</param>
        /// <returns>True if success, false or an error if un successful</returns>
        Task<bool> UpdateFundraiserAsync(string userId, string fundraiserId, Fundraiser fundraiser);

        /// <summary>
        /// Deletes a fundraiser. Note that we perform a "soft delete", where the fundraiser isn't
        /// actually physically deleted, but we set the closed_date to a non null value to indicate that
        /// it is terminated.
        /// </summary>
        /// <param name="fundraiserId">The fundraiser's id</param>
        /// <returns>True if success, else false or an error</returns>
        Task<bool> DeleteFundraiserAsync(string userId, string fundraiserId);

        /// <summary>
        /// Permanently deletes a fundraiser
        /// </summary>
        /// <param name="fundraiserId">The fundraiser's id</param>
        /// <returns>True if success, else false if an error</returns>
        Task<bool> HardDeleteFundraiserAsync(string userId, string fundraiserId);

        Task<string> GetFundraiserPictureIdAsync(string fundraiserId);

        /// <summary>
        /// Updates the picture id to the new picture URL
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="fundraiserId">The fundraiser id</param>
        /// <param name="pictureId">The picture URL</param>
        /// <returns>True if success</returns>
        Task<bool> UpdateFundraiserPictureIdAsync(string userId, string fundraiserId, string pictureId);
    }
}
