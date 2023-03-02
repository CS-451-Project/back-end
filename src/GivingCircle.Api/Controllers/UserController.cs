using GivingCircle.Api.Authorization;
using GivingCircle.Api.DataAccess.Repositories;
using GivingCircle.Api.DataAccess.Responses;
using GivingCircle.Api.Models;
using GivingCircle.Api.Providers;
using GivingCircle.Api.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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

        private readonly IUserProvider _userProvider;

        public UserController(
            ILogger<UserController> logger, 
            IUserRepository userRepository,
            IUserProvider userProvider)
        {
            _logger = logger;
            _userRepository = userRepository;
            _userProvider = userProvider;
        }

        /// <summary>
        /// Logs a user in with the given credentials. 
        /// </summary>
        /// <param name="email">The user's email</param>
        /// <param name="password">The user's password</param>
        /// <returns>The user's id if authentication was successful</returns>
        [AllowAnonymous]
        [HttpPost("user/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            string userId;

            try
            {
                userId = await _userProvider.ValidateUserAsync(request.Email, request.Password);
            }
            catch (Exception ex)
            {
                _logger.LogError("Something went wrong in the login endpoint");
                return StatusCode(500, ex.Message);
            }

            return Ok(userId);
        }

        [AllowAnonymous]
        [HttpPost("user")]
        public async Task<IActionResult> CreateUserAsync( [FromBody] CreateUserRequest request)
        {
            _logger.LogInformation("Received POST request");
            bool result;
            string userId;

            try
            {
                // Create the user id
                userId = Guid.NewGuid().ToString();

                //create user object
                User addUser = new()
                {
                    UserId = userId,
                    FirstName = request.FirstName,
                    MiddleInitial = request.MiddleInitial,
                    LastName = request.LastName,
                    Email = request.Email,
                    Password = request.Password,
                };

                result = await _userRepository.CreateUserAsync(addUser);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return StatusCode(500, err.Message);
            }

            return (result) ? Created("user", userId) : StatusCode(500, "Something went wrong");
        }

        [AllowAnonymous]
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
        public async Task<IActionResult> DeleteUser(string userId)
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
