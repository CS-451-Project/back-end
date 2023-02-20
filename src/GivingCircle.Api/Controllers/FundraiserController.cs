using GivingCircle.Api.Fundraiser.DataAccess;
using GivingCircle.Api.Fundraiser.DataAccess.Exceptions;
using GivingCircle.Api.Fundraiser.DataAccess.Responses;
using GivingCircle.Api.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
            // The fundraisers to return
            IEnumerable<GetFundraiserResponse> fundraisers;

            try
            {
                // Validate the provided user id
                Guid.Parse(userId);

                fundraisers = await _fundraiserRepository.ListFundraisersByUserIdAsync(userId);
            }
            catch (System.FormatException err)
            {
                _logger.LogError("Error listing fundraisers by user", err);
                return BadRequest("Invalid id");
            }
            catch (Exception err)
            {
                _logger.LogError("Error listing fundraisers by user", err);
                return StatusCode(500, "Something went wrong");
            }

            return Ok(fundraisers);
        }

        /// <summary>
        /// Filters fundraisers based on various criteria
        /// 
        /// Filter properties are title, tags, created date, end date
        /// 
        /// Order by properties are title, created date, planned end date, and by
        /// how close to the target goal the fundraiser is
        /// </summary>
        /// <param name="filterPropsRequest">The filter properties</param>
        /// <returns>A list of sorted and filtered fundraisers</returns>
        [HttpPost("filter")]
        public async Task<IActionResult> FilterFundraisers([FromBody] FilterFundraisersRequest filterPropsRequest)
        {
            // The valid order by columns. Used to map given val to table column name / order by method
            Dictionary<string, string> orderByClauseProps = new()
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

            try
            {
                // Check for the title props, add if they exist
                if (filterPropsRequest.Title != null)
                {
                    dbFilterProps.Add(nameof(filterPropsRequest.Title), new string[] { filterPropsRequest.Title });
                }

                // Check for the tag props, add if they exist
                if (filterPropsRequest.Tags != null)
                {
                    dbFilterProps.Add(nameof(filterPropsRequest.Tags), filterPropsRequest.Tags);
                }

                // Check for the created date props, calculate offset and add if they exist
                if (filterPropsRequest.CreatedDateOffset > 0.0)
                {
                    // Add a negative value to today to find the date time to use to compare against the db column
                    DateTime createdDataFilter = DateTime.Now.AddDays(-Math.Abs(filterPropsRequest.CreatedDateOffset));

                    dbFilterProps.Add(nameof(filterPropsRequest.CreatedDateOffset), new string[] { createdDataFilter.ToString() });
                }

                // Check for the end date props, calculate offset and add if they exist
                if (filterPropsRequest.EndDateOffset > 0.0)
                {
                    // Add a positive value to today to find the date time to use to compare against the db column
                    DateTime endDateFilter = DateTime.Now.AddDays(Math.Abs(filterPropsRequest.EndDateOffset));

                    dbFilterProps.Add(nameof(filterPropsRequest.EndDateOffset), new string[] { endDateFilter.ToString() });
                }

                // Check for the order by prop, add if they exist. Note that we can order by certain column names, and by being closest to the
                // target goal.
                if (filterPropsRequest.OrderBy != null && orderByClauseProps.ContainsKey(filterPropsRequest.OrderBy))
                {
                    dbFilterProps.Add(nameof(filterPropsRequest.OrderBy), new string[] { orderByClauseProps[filterPropsRequest.OrderBy] });
                    dbFilterProps.Add(nameof(filterPropsRequest.Ascending), new string[] { filterPropsRequest.Ascending ? "ASC" : "DESC" });
                }

                fundraisers = await _fundraiserRepository.FilterFundraisersAsync(dbFilterProps);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error filtering fundraisers", ex);
                return StatusCode(500, "Something went wrong");
            }

            return Ok(fundraisers);
        }

        /// <summary>
        /// Creates a fundraiser.
        /// </summary>
        /// <param name="request">The create fundraiser request <see cref="CreateFundraiserRequest"/></param>
        /// <returns>Status(201) if successful, failure codes otherwise</returns>
        [HttpPost]
        public async Task<IActionResult> CreateFundraiser([FromBody] CreateFundraiserRequest request)
        {
            // True if successfully created, false if there was an issue
            bool createdFundraiserResult;

            try
            {
                // Generate fundraiser id
                var fundraiserId = Guid.NewGuid().ToString();

                // Generate todays date
                var createdDate = DateTime.Now;

                // Assign the description if there is one
                var description = request.Description ?? "";

                // Initialize the starting balance
                var currentBalanceAmount = 0.0;

                // Try to parse the given planned end date
                var plannedEndDateParsed = DateTime.Parse(request.PlannedEndDate);

                // Note that we're not setting the GoalReachedDate or the ClosedDate
                // because they haven't happened yet.
                // Create the fundraiser object to be inserted 
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
            catch (Exception err)
            {
                _logger.LogError("Error creating fundraiser", err);
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
            // The deleted result. True if success, false if errors
            bool deletedFundraiserResult;

            try
            {
                // Validate the given id
                Guid.Parse(fundraiserId);

                deletedFundraiserResult = await _fundraiserRepository.DeleteFundraiserAsync(fundraiserId);
            }
            catch (System.FormatException err)
            {
                _logger.LogError("Error deleting fundraiser", err);
                return BadRequest("Invalid id");
            }
            catch (Exception err)
            {
                _logger.LogError("Error deleting fundraiser", err);
                return StatusCode(500, "Something went wrong");
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
