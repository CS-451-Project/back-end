using GivingCircle.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingCircle.Api.DataAccess.Repositories
{
    public interface IDonationRepository
    {
        /// <summary>
        /// Makes a donation. The donation will be made whether or not a user id
        /// is set for the donation.
        /// </summary>
        /// <param name="donation">The donation</param>
        /// <returns>True if success, false if something went wrong</returns>
        Task<bool> MakeDonation(Donation donation);

        /// <summary>
        /// Gets the donations for a fundraiser
        /// </summary>
        /// <param name="fundraiserId">The fundraiser's id</param>
        /// <returns>A list of donations</returns>
        Task<IEnumerable<Donation>> GetFundraiserDonations(string fundraiserId);

        /// <summary>
        /// Gets the donations for a fundraiser
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns>A list of donations</returns>
        Task<IEnumerable<Donation>> GetUserDonations(string userId);
    }
}
