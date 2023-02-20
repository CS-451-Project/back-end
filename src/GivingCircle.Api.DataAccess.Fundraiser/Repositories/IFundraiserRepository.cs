using GivingCircle.Api.Fundraiser.DataAccess.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingCircle.Api.Fundraiser.DataAccess
{
    public interface IFundraiserRepository
    {
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
        Task<bool> CreateFundraiserAsync(Models.Fundraiser fundraiser);

        /// <summary>
        /// Updates the fundraiser using the given object
        /// </summary>
        /// <param name="fundraiser">The fundraiser to update to</param>
        /// <returns>True if success, false or an error if un successful</returns>
        Task<bool> UpdateFundraiserAsync(Models.Fundraiser fundraiser);

        /// <summary>
        /// Deletes a fundraiser. Note that we perform a "soft delete", where the fundraiser isn't
        /// actually physically deleted, but we set the closed_date to a non null value to indicate that
        /// it is terminated.
        /// </summary>
        /// <param name="fundraiserId">The fundraiser's id</param>
        /// <returns>True if success, false or an error if not</returns>
        Task<bool> DeleteFundraiserAsync(string fundraiserId);
    }
}
