using GivingCircle.Api.Authorization;
using GivingCircle.Api.DataAccess.Repositories;
using GivingCircle.Api.DataAccess.Responses;
using GivingCircle.Api.Models;
using GivingCircle.Api.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingCircle.Api.Controllers
{
    [AuthorizeAttribute]
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        private readonly IUserRepository _userRepository;

        public UserController(
            ILogger<UserController> logger, 
            IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        [TypeFilter(typeof(Authorize))]
        [HttpPost("user/{userId}/users")]
        public async Task<IActionResult> CreateUserAsync( [FromBody] CreateUserRequest user)
        {
            _logger.LogInformation("Received POST request");
            var result = false;
            string userId;

            try
            {
                // Create the bank account id
                userId = Guid.NewGuid().ToString();

                //create user object
                User addUser = new()
                {
                    UserId = userId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Password = user.Password,
                };

                result = await _userRepository.CreateUserAsync(addUser);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return StatusCode(500, err.Message);
            }

            //return result ? StatusCode(201) : StatusCode(500);
            return (result) ? Created("user/{userId}/users", userId) : StatusCode(500, "Something went wrong");
        }

        [TypeFilter(typeof(Authorize))]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserAsync(string email)
        {

            GetUserResponse user;

            _logger.LogInformation("Received GET request");
            try
            {
                user = await _userRepository.GetUserAsync(email);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return StatusCode(500, err.Message);
            }

            return Ok(user);

        }

        [TypeFilter(typeof(Authorize))]
        [HttpDelete("user/{userId}")]
        public async Task<IActionResult> DeleteBankAccount(string userId)
        {
            _logger.LogInformation("Received DELETE request");

            bool result;

            try
            {
                result = await _userRepository.DeleteUserAsync(userId);
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
