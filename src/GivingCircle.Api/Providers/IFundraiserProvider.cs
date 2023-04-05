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

        Task<string> GetFundraiserPictureId(string fundraiserId);

        Task<bool> UpdateFundraiserPictureId(string userId, string fundraiserId, string pictureId);
    }
}
