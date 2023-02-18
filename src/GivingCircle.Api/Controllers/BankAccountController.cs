using GivingCircle.Api.Fundraiser.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GivingCircle.Api.Controllers
{
    [ApiController]
    [Route("api/fundraisers/bankaccount")]


    public class BankAccountController : ControllerBase
    {
        private readonly ILogger<BankAccountController> _logger;

        private readonly IFundraiserRepository _fundraiserRepository;
        public BankAccountController(ILogger<BankAccountController> logger, IFundraiserRepository fundraiserRepository)
        {
            _logger = logger;
            _fundraiserRepository = fundraiserRepository;

        }

    }
}
