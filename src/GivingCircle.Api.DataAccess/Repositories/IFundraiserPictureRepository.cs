using GivingCircle.Api.Models;
using System.Threading.Tasks;

namespace GivingCircle.Api.DataAccess.Repositories
{
    public interface IFundraiserPictureRepository
    {
        Task<bool> FundraiserPictureExists(string fundraiserId);

        Task<bool> AddFundraiserPicture(string userId, FundraiserPicture fundraiserPicture);

        Task<string> GetFundraiserPictureUrl(string fundraiserPictureId);
    }
}
