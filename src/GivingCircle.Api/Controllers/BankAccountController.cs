using GivingCircle.Api.Authorization;
using GivingCircle.Api.DataAccess.Repositories;
using GivingCircle.Api.Models;
using GivingCircle.Api.Requests.FundraiserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GivingCircle.Api.Controllers
{
    [AuthorizeAttribute]
    [ApiController]
    [Route("api/fundraisers")]
    public class BankAccountController : ControllerBase
    {
        private readonly ILogger<BankAccountController> _logger;

        private readonly IBankAccountRepository _bankAccountRepository;

        public BankAccountController(
            ILogger<BankAccountController> logger, 
            IBankAccountRepository bankAccountRepository)
        {
            _logger = logger;
            _bankAccountRepository = bankAccountRepository;
        }

        [TypeFilter(typeof(Authorize))]
        [HttpPost("user/{userId}/bankaccount")]
        public async Task<IActionResult> AddBankAccount(string userId, [FromBody] AddBankAccountRequest bankaccount)
        {
            _logger.LogInformation("Received POST request");
            var result = false;
            string bankaccountid;

            try
            {
                // Create the bank account id
                bankaccountid = Guid.NewGuid().ToString();

                //Bank Account Object
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

                result = await _bankAccountRepository.AddBankAccount(addBankAccount);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return StatusCode(500, err.Message);
            }

            //return result ? StatusCode(201) : StatusCode(500);
            return (result) ? Created("user/{userId}/bankaccount", bankaccountid) : StatusCode(500, "Something went wrong");
        }

        [TypeFilter(typeof(Authorize))]
        [HttpGet("user/{userId}/bankaccount/{bankAccountId}")]
        public async Task<IActionResult> GetAccount(string userId, string bankAccountId)
        {

            BankAccount result;

            _logger.LogInformation("Received GET request");
            try
            {
                result = await _bankAccountRepository.GetBankAccount(bankAccountId);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return StatusCode(500, err.Message);
            }

            return Ok(result);

        }

        [TypeFilter(typeof(Authorize))]
        [HttpDelete("user/{userId}/bankaccount/{bankAccountId}")]
        public async Task<IActionResult> DeleteBankAccount(string userId, string bankAccountId)
        {
            _logger.LogInformation("Received DELETE request");

            bool result;

            try
            {
                result = await _bankAccountRepository.DeleteBankAccountAsync(bankAccountId);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return StatusCode(500, err.Message);
            }

            return result ? StatusCode(204) : StatusCode(500);
        }
    }
}
