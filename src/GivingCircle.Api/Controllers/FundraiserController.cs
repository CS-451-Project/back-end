using GivingCircle.Api.Fundraiser.DataAccess;
using GivingCircle.Api.Fundraiser.DataAccess.Exceptions;
using GivingCircle.Api.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace GivingCircle.Api.Controllers
{
    [ApiController]
    [Route("api/fundraisers")]
    public class FundraiserController : ControllerBase
    {
        private readonly ILogger<FundraiserController> _logger;

        private readonly IFundraiserRepository _fundraiserRepository;

        public FundraiserController(
            ILogger<FundraiserController> logger,
            IFundraiserRepository fundraiserRepository)
        {
            _logger = logger;
            _fundraiserRepository = fundraiserRepository;

        }

        /// <summary>
        /// Lists the fundraisers tied to a user's id
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns>A list of fundraisers if they exist, an empty list otherwise</returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> ListFundraisersByUserId(string userId)
        {
            IEnumerable<Fundraiser.Models.Fundraiser> fundraisers;

            try
            {
                Guid.Parse(userId);

                fundraisers = await _fundraiserRepository.ListFundraisersByUserIdAsync(userId);
            }
            catch (System.FormatException err)
            {
                _logger.LogError(err.Message);
                return BadRequest("Invalid id");
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return StatusCode(500, "Something went wrong");
            }

            return Ok(fundraisers);
        }

        [HttpPost("filter")]
        public async Task<IActionResult> FilterFundraisers([FromBody] FundraiserFilterPropsRequest filterProps)
        {
            IEnumerable<Fundraiser.Models.Fundraiser> fundraisers;
            Dictionary<string, string[]> dbFilterProps = new ();

            if (filterProps.Title != null)
            {
                dbFilterProps.Add("Title", new string[] { filterProps.Title });
            }
            else
            {
                dbFilterProps.Add("Title", null);
            }

            if (filterProps.Tags != null)
            {
                dbFilterProps.Add("Tags", filterProps.Tags);
            }
            else
            {
                dbFilterProps.Add("Tags", null);
            }

            fundraisers = await _fundraiserRepository.FilterFundraisersAsync(dbFilterProps);

            return Ok(fundraisers);
        }

        /// <summary>
        /// Create fundraiser
        /// </summary>
        /// <param name="request">The create fundraiser request <see cref="CreateFundraiserRequest"/></param>
        /// <returns>Status(201) if successful, failure codes otherwise</returns>
        [HttpPost]
        public async Task<IActionResult> CreateFundraiser([FromBody] CreateFundraiserRequest request)
        {
            bool createdFundraiserResult;

            try
            {
                var fundraiserId = Guid.NewGuid().ToString();
                var createdDate = DateTime.Now;
                var description = request.Description ?? "";
                var currentBalanceAmount = 0.0;

                // Try to parse the given planned end date
                var plannedEndDateParsed = DateTime.Parse(request.PlannedEndDate);

                // Note that we're not setting the GoalReachedDate or the ClosedDate
                // because they haven't happened yet. They'll be created as null in the db this way.
                // Can't set a date time object to null so have to do it that way.
                Fundraiser.Models.Fundraiser fundraiser = new()
                {
                    FundraiserId = fundraiserId,
                    OrganizerId = request.OrganizerId,
                    BankInformationId = request.BankInformationId,
                    PictureId = request.PictureId,
                    Description = description,
                    Title = request.Title,
                    CreatedDate = createdDate,
                    PlannedEndDate = plannedEndDateParsed,
                    GoalTargetAmount = request.GoalTargetAmount,
                    CurrentBalanceAmount = currentBalanceAmount,
                    Tags = request.Tags
                };

                createdFundraiserResult = await _fundraiserRepository.CreateFundraiserAsync(fundraiser);
            }
            catch (InvalidBankAccountIdException err)
            {
                _logger.LogError(err.Message);
                return StatusCode(500, err.Message);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return StatusCode(500, "Something went wrong");
            }

            return (createdFundraiserResult) ? StatusCode(201) : StatusCode(500, "Something went wrong");
        }

        /// <summary>
        /// Deletes a single fundraiser
        /// </summary>
        /// <param name="fundraiserId">The fundraiser's id</param>
        /// <returns>Status 200 if success, error codes if failure</returns>
        [HttpDelete("{fundraiserId}")]
        public async Task<IActionResult> DeleteFundraiser(string fundraiserId)
        {
            bool deletedFundraiserResult;

            try
            {
                Guid.Parse(fundraiserId);

                deletedFundraiserResult = await _fundraiserRepository.DeleteFundraiserAsync(fundraiserId);
            }
            catch (System.FormatException err)
            {
                _logger.LogError(err.Message);
                return BadRequest("Invalid id");
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return StatusCode(500);
            }

            if (deletedFundraiserResult)
            {
                _logger.LogInformation("Successfully deleted {fundraiserId}", fundraiserId);
                return Ok();
            }
            else
            {
                _logger.LogInformation("Unable to delete {fundraiserId}", fundraiserId);
                return StatusCode(500, "Something went wrong");
            }
        }
    }
}
