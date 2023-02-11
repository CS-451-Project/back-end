using GivingCircle.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GivingCircle.Api.Controllers
{
    [ApiController]
    [Route("api/fundraisers")]
    public class FundraiserController : ControllerBase
    {
        private readonly ILogger<FundraiserController> _logger;

        private readonly IFundraiserService _fundraiserService;

        public FundraiserController(
            ILogger<FundraiserController> logger,
            IFundraiserService fundraiserService)
        {
            _logger = logger;
            _fundraiserService = fundraiserService;

        }

        /// <summary>
        /// Lists all fundraisers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ListFundraisers()
        {
            _logger.LogInformation("Received GET request");

            var fundraisers = await _fundraiserService.ListAllFundraisersAsync();

            return Ok(fundraisers);
        }
    }
}
