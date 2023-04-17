using Amazon.Runtime.Internal;
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
        /// <param name="request"/>>The login request</param>
        /// <returns>The user's id if authentication was successful, else an error or nothing</returns>
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

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="request">The create user request</param>
        /// <returns>The user's id if successful, else an error code</returns>
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

        /// <summary>
        /// Gets a user by their id
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns>The user</returns>
        [AllowAnonymous]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            GetUserResponse user;

            _logger.LogInformation("Received GET request");
            try
            {
                user = await _userProvider.GetUserAsync(userId);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("Invalid id");
            }
            // If the sequence conatains no elements then this exception is thrown
            // That is equivalent to the user not existing
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex.Message);
                return NotFound("User does not exist");
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return StatusCode(500, err.Message);
            }

            return Ok(user);
        }

        [@Authorize]
        [HttpPut("user/{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, UpdateUserRequest request)
        {
            // True if successfully updated, false if there was an issue
            bool updateUserResult;

            try
            {
                // Create the User object
                User user = new()
                {
                    UserId = userId, 
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    MiddleInitial = request.MiddleInitial, 
                    Password = request.Password
                };

                updateUserResult = await _userRepository.UpdateUserAsync(userId, user);
            }
            catch (Exception err)
            {
                _logger.LogError("Error updating user", err);
                return StatusCode(500, "Something went wrong");
            }

            return (updateUserResult) ? StatusCode(200) : StatusCode(500, "Something went wrong");
        }

        [@Authorize]
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
