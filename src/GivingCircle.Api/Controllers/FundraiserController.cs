using GivingCircle.Api.Fundraiser.DataAccess;
using GivingCircle.Api.Fundraiser.DataAccess.Exceptions;
using GivingCircle.Api.Fundraiser.DataAccess.Responses;
using GivingCircle.Api.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
            IEnumerable<GetFundraiserResponse> fundraisers;

            try
            {
                // Validate the provided user id
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

        /// <summary>
        /// Filters fundraiasers based on various criteria
        /// </summary>
        /// <param name="filterProps">The filter properties</param>
        /// <returns>A list of filtered fundraisers</returns>
        [HttpPost("filter")]
        public async Task<IActionResult> FilterFundraisers([FromBody] FilterFundraisersRequest filterProps)
        {
            // The valid order by columns. Used to map given val to table column name
            Dictionary<string, string> orderByColumns = new()
            {
                { "Title", "title" },
                { "CreatedDate", "created_date" },
                { "PlannedEndDate", "planned_end_date" },
                { "ClosestToTargetGoal", "current_balance_amount"}
            };

            // The response
            IEnumerable<GetFundraiserResponse> fundraisers;

            // The filter props to supply the repository with
            Dictionary<string, string[]> dbFilterProps = new ();

            // Check the title props
            if (filterProps.Title != null)
            {
                dbFilterProps.Add(nameof(filterProps.Title), new string[] { filterProps.Title });
            }

            // Check the tag props
            if (filterProps.Tags != null)
            {
                dbFilterProps.Add(nameof(filterProps.Tags), filterProps.Tags);
            }

            // Check the created date props
            if (filterProps.CreatedDateOffset > 0.0)
            {
                // Add a negative value to today to find the date time to use to compare against the db column
                DateTime createdDataFilter = DateTime.Now.AddDays(-Math.Abs(filterProps.CreatedDateOffset));
                dbFilterProps.Add(nameof(filterProps.CreatedDateOffset), new string[] { createdDataFilter.ToString() });
            }

            // Check the end date props
            if (filterProps.EndDateOffset > 0.0)
            {
                // Add a positive value to today to find the date time to use to compare against the db column
                DateTime endDateFilter = DateTime.Now.AddDays(Math.Abs(filterProps.EndDateOffset));
                dbFilterProps.Add(nameof(filterProps.EndDateOffset), new string[] { endDateFilter.ToString() });
            }

            // Check if there is an order by prop and for ascending or descending or for the closest to target goal
            if (filterProps.OrderBy != null && orderByColumns.ContainsKey(filterProps.OrderBy))
            {
                dbFilterProps.Add(nameof(filterProps.OrderBy), new string[] { orderByColumns[filterProps.OrderBy] });
                dbFilterProps.Add(nameof(filterProps.Ascending), new string[] { filterProps.Ascending ? "ASC" : "DESC" });
            }

            try
            {
                fundraisers = await _fundraiserRepository.FilterFundraisersAsync(dbFilterProps);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, "Something went wrong");
            }

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
                    GoalReachedDate = null,
                    ClosedDate = null,
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
        /// Deletes a single fundraiser. Note that we do a "soft delete", and so the fundraiser isn't physically
        /// deleted. We set the closed_date to a non null value to indicate deletion.
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
