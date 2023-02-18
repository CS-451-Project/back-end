using GivingCircle.Api.Fundraiser.DataAccess;
using Microsoft.AspNetCore.Http;
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

        public async Task<IActionResult> CreateFundraiser(
            string userId,
            string bankInformationId,
            string title, 
            double goalAmount,
            string plannedEndDate,
            string fundraiserPictureId = null,
            string description = null)
        {
            using (_logger.BeginScope("Received GET request: {user_id}", userId))
            {
                Fundraiser.Models.Fundraiser fundraiser;

                try
                {
                    // Create the fundraiser id
                    var fundraiserId = Guid.NewGuid().ToString();

                    // Set the created date
                    var createdDate = DateTime.Now;

                    // Try to parse the given planned end date
                    var plannedEndDateParsed = DateTime.Parse(plannedEndDate);

                    // If description is null then set it to empty
                    if (description == null)
                    {
                        description = "";
                    }

                    fundraiser = new Fundraiser.Models.Fundraiser
                    {
                        OrganizerId = userId,
                        BankInformationId = bankInformationId,
                        Title = title,
                        Description = description,
                        GoalTargetAmount = goalAmount,
                        CreatedDate = createdDate,
                        PlannedEndDate = plannedEndDateParsed
                    };

                    var createFundraiser = _fundraiserRepository.CreateFundraiserAsync(fundraiser);
                }
                catch (Exception err)
                {
                    _logger.LogError(err.Message);
                }
                return Ok();
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
