using System.Threading.Tasks;

namespace GivingCircle.Api.Providers
{
    public interface IFundraiserProvider
    {
        /// <summary>
        /// Increments the amount column for a fundraiser
        /// </summary>
        /// <param name="fundraiserId">The fundraiser to increment</param>
        /// <param name="amount">The amount to increment by</param>
        /// <returns>True if success, false if error</returns>
        Task<bool> MakeDonation(string fundraiserId, double amount);

        /// <summary>
        /// Updates the picture id to the new picture URL
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="fundraiserId">The fundraiser id</param>
        /// <param name="pictureId">The picture URL</param>
        /// <returns>True if success</returns>
        Task<bool> UpdateFundraiserPictureId(string userId, string fundraiserId, string pictureId);
    }
}
