using GivingCircle.Api.DataAccess;
using GivingCircle.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingCircle.Api.Services
{
    public class FundraiserService : IFundraiserService
    {
        private readonly IFundraiserRepository _fundraiserRepository;

        public FundraiserService(IFundraiserRepository fundraiserRepository) 
        { 
            _fundraiserRepository = fundraiserRepository;
        }

        public Task<IEnumerable<Fundraiser>> ListAllFundraisersAsync()
        {
            return _fundraiserRepository.ListAllFundraisersAsync();
        }
    }
}
