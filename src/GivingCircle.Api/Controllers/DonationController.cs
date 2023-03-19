using GivingCircle.Api.Authorization;
using GivingCircle.Api.DataAccess.Repositories;
using GivingCircle.Api.Models;
using GivingCircle.Api.Providers;
using GivingCircle.Api.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly IUserProvider _userProvider;

        /// <summary>
        /// Controller for the <see cref="Donation"/> resource.
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="donationRepository">The donation repository</param>
        /// <param name="fundraiserProvider">The fundraiser provider</param>
        public DonationController(
            ILogger<DonationController> logger,
            IDonationRepository donationRepository,
            IFundraiserProvider fundraiserProvider,
            IUserProvider userProvider) 
        {
            _logger = logger;
            _donationRepository = donationRepository;
            _fundraiserProvider = fundraiserProvider;
            _userProvider = userProvider;
        }

        /// <summary>
        /// Gets the donations for a fundraiser
        /// </summary>
        /// <param name="fundraiserId">The fundraiser id</param>
        /// <returns>A list of donations</returns>
        [AllowAnonymous]
        [HttpGet("fundraiser/{fundraiserId}/donation")]
        public async Task<IActionResult> GetFundraiserDonations(string fundraiserId)
        {
            // The fundraiser's donations to return
            IEnumerable<Donation> donations;

            try
            {
                // Validate the guid
                Guid.Parse(fundraiserId);

                // Get the donations, if any
                donations = await _donationRepository.GetFundraiserDonations(fundraiserId);
            }
            catch (FormatException ex)
            {
                _logger.LogError("Bad fundraiser id", ex.Message);
                return BadRequest("Invalid id");
            }
            catch (Exception ex)
            {
                _logger.LogError("Something went wrong", ex.Message);
                return StatusCode(500, "Something went wrong");
            }

            return Ok(donations) ?? Ok(Enumerable.Empty<Donation>());
        }

        /// <summary>
        /// Makes an anonymous donation
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <param name="request">The request</param>
        /// <returns>No content if the donation was successfully make, an error status otherwise</returns>
        [AllowAnonymous]
        [HttpPost("donation")]
        public async Task<IActionResult> MakeAnonymousDonation([FromBody] MakeDonationRequest request)
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
                    UserId = null,
                    Name = null
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

        /// <summary>
        /// Makes a donation for a user
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <param name="request">The request</param>
        /// <returns>No content if the donation was successfully make, an error status otherwise</returns>
        [@Authorize]
        [HttpPost("user/{userId}/donation")]
        public async Task<IActionResult> MakeUserDonation(string userId, [FromBody] MakeDonationRequest request)
        {
            bool result;
            string donationId;

            try
            {
                // Validate user id
                Guid.Parse(userId);

                // Generate the donation id
                donationId = Guid.NewGuid().ToString();

                // Generate todays date
                var date = DateTime.Now;

                // Get the user's name
                var user = await _userProvider.GetUserAsync(userId);
                var name = user.FirstName + " " + user.LastName;

                Donation donation = new()
                {
                    DonationId = donationId,
                    Amount = request.Amount,
                    Date = date,
                    FundraiserId = request.FundraiserId,
                    Message = request.Message,
                    UserId = userId,
                    Name = name
                };

                // Create the donation
                result = await _donationRepository.MakeDonation(donation);

                // Increment the amount in the fundraiser itself
                result = await _fundraiserProvider.MakeDonation(donation.FundraiserId, donation.Amount);
            }
            catch (FormatException ex)
            {
                _logger.LogError("Bad user id", ex.Message);
                return BadRequest("Invalid id");
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
