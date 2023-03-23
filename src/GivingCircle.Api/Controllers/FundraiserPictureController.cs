using Amazon.S3;
using GivingCircle.Api.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GivingCircle.Api.Controllers
{
    [AuthorizeAttribute]
    [ApiController]
    [Route("api")]
    public class FundraiserPictureController : ControllerBase
    {
        private readonly ILogger _logger;

        private readonly IAmazonS3 _s3Client;

        public FundraiserPictureController(
            ILogger<FundraiserController> logger,
            IAmazonS3 s3Client) 
        {
            _logger = logger;
            _s3Client = s3Client;
        }

        [AllowAnonymous]
        [HttpPost("user/{userId}/fundraiser/{fundraiserId}/image")]
        public async Task<IActionResult> UploadFundraiserPicture(
            string userId, 
            string fundraiserId, 
            [FromForm] UploadFundraiserImageRequest request)
        {
            _logger.LogInformation("Stuff: ");

            var path = "C:\\temp";

            try
            {
                if (request.FundraiserImage.Length > 0)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (FileStream filestream = System.IO.File.Create(path + "\\" + request.FundraiserImage.FileName))
                    {
                        request.FundraiserImage.CopyTo(filestream);
                        filestream.Flush();
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return Ok(request.FundraiserImage);
        }
    }
}
