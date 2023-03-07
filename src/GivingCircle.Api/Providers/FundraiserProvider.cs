using GivingCircle.Api.DataAccess.Repositories;
using System.Threading.Tasks;

namespace GivingCircle.Api.Providers
{
    public class FundraiserProvider : IFundraiserProvider
    {
        private readonly IFundraiserRepository _fundraiserRepository;

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
    }
}
