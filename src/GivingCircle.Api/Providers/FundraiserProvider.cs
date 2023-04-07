using GivingCircle.Api.DataAccess.Repositories;
using GivingCircle.Api.Models;
using System.Threading.Tasks;

namespace GivingCircle.Api.Providers
{
    /// <inheritdoc />
    public class FundraiserProvider : IFundraiserProvider
    {
        private readonly IFundraiserRepository _fundraiserRepository;

        /// <summary>
        /// A provider for managing logic for <see cref="Fundraiser"/>.
        /// </summary>
        /// <param name="fundraiserRepository">The fundraiser repository</param>
        public FundraiserProvider(IFundraiserRepository fundraiserRepository) 
        {
            _fundraiserRepository = fundraiserRepository;
        }

        public async Task<bool> MakeDonation(string fundraiserId, double amount)
        {
            bool result;

            result = await _fundraiserRepository.MakeDonation(fundraiserId, amount);

            return result;
        }

        public async Task<bool> UpdateFundraiserPictureId(string userId, string fundraiserId, string pictureId)
        {
            bool result;

            result = await _fundraiserRepository.UpdateFundraiserPictureIdAsync(userId, fundraiserId, pictureId);

            return result;
        }
    }
}
