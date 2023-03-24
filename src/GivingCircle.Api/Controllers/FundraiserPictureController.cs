using Amazon.S3;
using Amazon.S3.Model;
using GivingCircle.Api.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Security.AccessControl;
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

        private const string _bucketName = "fundraiser-images-ac-3n681tgoywf1swqxb7cygj9th633huse2b-s3alias";

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
            var fundraiserPictureId = Guid.NewGuid().ToString();

            PutObjectResponse putObjectResponse = null;

            try
            {
                var path = "C:\\temp";

                var filePath = path + "\\" + fundraiserPictureId + ".jpg";

                if (request.FundraiserImage.Length > 0)
                {
                    // Check if temp directory exists, create it if not
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    // Save file locally
                    using (FileStream filestream = System.IO.File.Create(filePath))
                    {
                        request.FundraiserImage.CopyTo(filestream);
                        filestream.Flush();
                    }
                }

                // Create AWS S3 put object request
                var putObjectRequest = new PutObjectRequest 
                { 
                    BucketName = _bucketName, 
                    FilePath = filePath, 
                    Key = fundraiserPictureId 
                };

                putObjectResponse = await _s3Client.PutObjectAsync(putObjectRequest);

                // Delete the file locally
                System.IO.File.Delete(filePath);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            if (putObjectResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"Successfully uploaded {fundraiserPictureId} to {_bucketName}.");
                return Ok(fundraiserPictureId);
            }
            else
            {
                Console.WriteLine($"Could not upload {fundraiserPictureId} to {_bucketName}.");
                return StatusCode(500);
            }
        }
    }
}
