using Amazon.Runtime.Internal;
using Amazon.S3;
using Amazon.S3.Model;
using GivingCircle.Api.Authorization;
using GivingCircle.Api.DataAccess.Repositories;
using GivingCircle.Api.Models;
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

        private readonly IFundraiserPictureRepository _fundraiserPictureRepository;

        private readonly IFundraiserProvider _fundraiserProvider;

        private const string _bucketName = "fundraiser-images-ac-3n681tgoywf1swqxb7cygj9th633huse2b-s3alias";

        public FundraiserPictureController(
            ILogger<FundraiserController> logger,
            IAmazonS3 s3Client,
            IFundraiserPictureRepository fundraiserPictureRepository,
            IFundraiserProvider fundraiserProvider) 
        {
            _logger = logger;
            _s3Client = s3Client;
            _fundraiserPictureRepository = fundraiserPictureRepository;
            _fundraiserProvider = fundraiserProvider;
        }

        //[HttpGet("user/{userId}/fundraiser/{fundraiserId}/image")]
        //public async Task<IActionResult> GetFundraiserPicture(string userId, string fundraiserId)
        //{
        //    // Get the url
        //    try
        //    {
        //        var pictureId = await _fundraiserProvider.GetFundraiserPictureId(fundraiserId);

        //        GetObjectRequest getObjectRequest = new() 
        //        { 
        //            BucketName= _bucketName,
        //            Key = pictureId
        //        };

        //        var getObjectResponse = await _s3Client.GetObjectAsync(getObjectRequest);

        //        if (getObjectResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
        //        {
        //            return StatusCode(500);
        //        }

        //        var fundraiserPicture = getObjectResponse.Key;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message, ex);
        //    }

        //    // Use the url to get the image and return the image
        //    return Ok();
        //}

        [@Authorize]
        [HttpPost("user/{userId}/fundraiser/{fundraiserId}/image")]
        public async Task<IActionResult> UploadFundraiserPicture(
            string userId, 
            string fundraiserId, 
            [FromForm] UploadFundraiserImageRequest request)
        {
            PutObjectResponse putObjectResponse = null;  // The response from S3
            bool result = false; // The db operation result

            // Generate the picture's id
            var pictureId = Guid.NewGuid().ToString();

            try
            {
                var filePath = SaveImageFileLocally(pictureId, request);

                putObjectResponse = await PutObjectAsync(filePath, pictureId);

                if (putObjectResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    return StatusCode(500);
                }

                var pictureUrl = $"https://fundraiser-images.s3.us-east-2.amazonaws.com/{pictureId}";

                result = await _fundraiserProvider.UpdateFundraiserPictureId(userId, fundraiserId, pictureUrl);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return (result) ? Created("user/{userId}/fundraiser/{fundraiserId}/image", pictureId) : StatusCode(500, "Something went wrong");
        }

        // Puts the object into S3 and deletes the file locally
        private async Task<PutObjectResponse> PutObjectAsync(string filePath, string pictureId)
        {
            // Create AWS S3 put object request
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                FilePath = filePath,
                Key = pictureId
            };

            var putObjectResponse = await _s3Client.PutObjectAsync(putObjectRequest);

            // Delete the file locally
            System.IO.File.Delete(filePath);

            return putObjectResponse;
        }

        // Saves the image locally and returns the image path
        private static string SaveImageFileLocally(string fundraiserPictureId, UploadFundraiserImageRequest request)
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

            return filePath;
        }
    }
}
