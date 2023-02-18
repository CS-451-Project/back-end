using GivingCircle.Api.Fundraiser.DataAccess;
using GivingCircle.Api.Fundraiser.DataAccess.Exceptions;
using GivingCircle.Api.Requests.FundraiserService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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

        [HttpPost]
        public async Task<IActionResult> CreateFundraiser([FromBody] CreateFundraiserRequest request)
        {
            using (_logger.BeginScope("Received GET request: {user_id}", request.OrganizerId))
            {
                string description = null;
                var createdFundraiser = false;

                try
                {
                    // Create the fundraiser id
                    var fundraiserId = Guid.NewGuid().ToString();

                    // Set the created date
                    var createdDate = DateTime.Now;

                    // Try to parse the given planned end date
                    var plannedEndDateParsed = DateTime.Parse(request.PlannedEndDate);

                    // If description is null then set it to empty
                    if (request.Description == null)
                    {
                        description = "";
                    }
                    else
                    {
                        description = request.Description;
                    }

                    // Set the current balance
                    var currentBalanceAmount = 0.0;

                    // Note that we're not setting the GoalReachedDate or the ClosedDate
                    // because they haven't happened yet. They'll be created as null in the db this way.
                    // Can't set a date time object to null so have to do it that way.
                    Fundraiser.Models.Fundraiser fundraiser = new Fundraiser.Models.Fundraiser
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

                    createdFundraiser = await _fundraiserRepository.CreateFundraiserAsync(fundraiser);
                }
                catch (BankAccountIdInvalidException err)
                {
                    _logger.LogError(err.Message);
                    return StatusCode(500, err.Message);
                }

                if (createdFundraiser)
                {
                    return StatusCode(201);
                }
                else
                {
                    return StatusCode(500);
                }
            }
        }

        /// <summary>
        /// Lists fundraisers by 
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //public async Task<IActionResult> ListFundraisers()
        //{
        //    _logger.LogInformation("Received GET request");

        //    var fundraisers = await _fundraiserService.ListAllFundraisersAsync();

        //    return Ok(fundraisers);
        //}
    }
}
