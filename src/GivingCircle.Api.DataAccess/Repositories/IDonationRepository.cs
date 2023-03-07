using GivingCircle.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingCircle.Api.DataAccess.Repositories
{
    public interface IDonationRepository
    {
        /// <summary>
        /// Makes a donation
        /// </summary>
        /// <param name="donation">The donation</param>
        /// <returns>True if success, false if something went wrong</returns>
        Task<bool> MakeDonation(Donation donation);

        Task<IEnumerable<Donation>> GetFundraiserDonations(string fundraiserId);

        Task<IEnumerable<Donation>> GetUserDonations(string userId);
    }
}
