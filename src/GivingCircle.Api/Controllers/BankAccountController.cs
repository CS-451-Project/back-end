using GivingCircle.Api.Fundraiser.DataAccess;
using GivingCircle.Api.Fundraiser.Models;
using GivingCircle.Api.Fundraiser.Models.Models;
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

        public async Task<IActionResult> AddBankAccount([FromBody] BankAccount bankaccount)
        {
            _logger.LogInformation("Received POST request");
            try
            {
                // Create the bank account id
                var bankaccountid = Guid.NewGuid().ToString();
                BankAccount addBankAccount = new()
                {
                    Account_Name = bankaccount.Account_Name,
                    Address = bankaccount.Address,
                    City = bankaccount.City,
                    State = bankaccount.State,
                    Zipcode = bankaccount.Zipcode,
                    Bank_Name = bankaccount.Bank_Name,
                    Account_Num = bankaccount.Account_Num,
                    Routing_Num = bankaccount.Routing_Num,
                    Account_Type = bankaccount.Account_Type,
                    Bank_Account_Id = bankaccountid,
                  };
                var result = await _fundraiserRepository.AddBankAccount(addBankAccount);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
            }


            return Ok();
        }



    }
}
