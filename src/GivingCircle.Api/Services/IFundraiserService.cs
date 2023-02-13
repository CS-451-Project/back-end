using GivingCircle.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingCircle.Api.Services
{
    public interface IFundraiserService
    {
        Task<IEnumerable<Fundraiser>> ListAllFundraisersAsync();
    }
}
