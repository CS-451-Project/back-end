using System.Threading.Tasks;

namespace GivingCircle.Api.Providers
{
    public interface IFundraiserProvider
    {
        Task<bool> MakeDonation(string fundraiserId, double amount);
    }
}
