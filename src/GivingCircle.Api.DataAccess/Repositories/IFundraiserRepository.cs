using GivingCircle.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingCircle.Api.DataAccess.Repositories
{
    public interface IFundraiserRepository
    {
        Task<IEnumerable<Fundraiser>> ListAllFundraisersAsync();
    }
}
