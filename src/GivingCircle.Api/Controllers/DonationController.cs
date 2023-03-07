using GivingCircle.Api.Authorization;
using GivingCircle.Api.DataAccess.Repositories;
using GivingCircle.Api.Models;
using GivingCircle.Api.Providers;
using GivingCircle.Api.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GivingCircle.Api.Controllers
{
    [AuthorizeAttribute]
    [ApiController]
    [Route("api")]
    public class DonationController : ControllerBase
    {
        private readonly ILogger<DonationController> _logger;

        private readonly IDonationRepository _donationRepository;

        private readonly IFundraiserProvider _fundraiserProvider;

        public DonationController(
            ILogger<DonationController> logger,
            IDonationRepository donationRepository,
            IFundraiserProvider fundraiserProvider) 
        {
            _logger = logger;
            _donationRepository = donationRepository;
            _fundraiserProvider = fundraiserProvider;
        }

        /// <summary>
        /// Makes a donation for a user
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <param name="request">The request</param>
        /// <returns>No content if the donation was successfully make, an error status otherwise</returns>
        [@Authorize]
        [HttpPost("user/{userId}/donate")]
        public async Task<IActionResult> MakeUserDonation(string userId, [FromBody] MakeDonationRequest request)
        {
            bool result;
            string donationId;

            try
            {
                // Generate the donation id
                donationId = Guid.NewGuid().ToString();

                // Generate todays date
                var date = DateTime.Now;

                Donation donation = new()
                {
                    DonationId = donationId,
                    Amount = request.Amount,
                    Date = date,
                    FundraiserId = request.FundraiserId,
                    Message = request.Message,
                    UserId = userId
                };

                // Create the donation
                result = await _donationRepository.MakeDonation(donation);

                // Increment the amount in the fundraiser itself
                result = await _fundraiserProvider.MakeDonation(donation.FundraiserId, donation.Amount);
            }
            catch (Exception ex) 
            {
                _logger.LogError("Error making a donation", ex.Message);
                return StatusCode(500, "Something went wrong");
            }

            return (result) ? NoContent() : StatusCode(500, "Something went wrong");
        }
    }
}
