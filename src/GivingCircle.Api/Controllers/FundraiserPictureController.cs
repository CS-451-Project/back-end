using Amazon.S3;
using Amazon.S3.Model;
using GivingCircle.Api.Authorization;
using GivingCircle.Api.Providers;
using GivingCircle.Api.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
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

        private readonly IFundraiserProvider _fundraiserProvider;

        private const string _bucketName = "fundraiser-images-ac-3n681tgoywf1swqxb7cygj9th633huse2b-s3alias";

        public FundraiserPictureController(
            ILogger<FundraiserController> logger,
            IAmazonS3 s3Client,
            IFundraiserProvider fundraiserProvider) 
        {
            _logger = logger;
            _s3Client = s3Client;
            _fundraiserProvider = fundraiserProvider;
        }

        /// <summary>
        /// Uploads a new image to S3 and saves that objects URL to the associated Fundraiser
        /// </summary>
        /// <param name="userId">The calling user's id</param>
        /// <param name="fundraiserId">The fundraiser id</param>
        /// <param name="request">The request</param>
        /// <returns>The id of the picture if upload was successful</returns>
        [@Authorize]
        [HttpPost("user/{userId}/fundraiser/{fundraiserId}/image")]
        public async Task<IActionResult> UploadFundraiserPicture(
            string userId, 
            string fundraiserId, 
            [FromForm] UploadFundraiserImageRequest request)
        {
            PutObjectResponse putObjectResponse;  // The response from S3
            bool result = false; // The db operation result

            // Generate the picture's id
            var pictureId = Guid.NewGuid().ToString();

            try
            {
                // The temp folder
                var path = "C:\\temp";

                // The file path for the image
                var filePath = path + "\\" + pictureId + ".jpg";

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

                // Create AWS S3 put object request
                var putObjectRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    FilePath = filePath,
                    Key = pictureId
                };

                // Upload the object to S3
                putObjectResponse = await _s3Client.PutObjectAsync(putObjectRequest);

                // Delete the file locally
                System.IO.File.Delete(filePath);

                // Check if the file upload was successful
                if (putObjectResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    return StatusCode(500);
                }

                // The S3 URL
                var pictureUrl = $"https://fundraiser-images.s3.us-east-2.amazonaws.com/{pictureId}";

                // Update the fundraiser object in the db with the new picture URL
                result = await _fundraiserProvider.UpdateFundraiserPictureId(userId, fundraiserId, pictureUrl);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return (result) ? Created("user/{userId}/fundraiser/{fundraiserId}/image", pictureId) : StatusCode(500, "Something went wrong");
        }
    }
}
