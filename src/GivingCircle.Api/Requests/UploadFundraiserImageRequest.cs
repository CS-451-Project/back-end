using Microsoft.AspNetCore.Http;

namespace GivingCircle.Api.Requests
{
    public class UploadFundraiserImageRequest
    {
        public IFormFile FundraiserImage { get; set; }
    }
}
