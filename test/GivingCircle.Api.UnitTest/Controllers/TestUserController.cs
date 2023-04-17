using GivingCircle.Api.Controllers;
using GivingCircle.Api.DataAccess.Repositories;
using GivingCircle.Api.DataAccess.Responses;
using GivingCircle.Api.Models;
using GivingCircle.Api.Providers;
using GivingCircle.Api.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace GivingCircle.Api.UnitTest.Controllers
{
    public class TestUserController
    {
        [Fact]
        public async Task TestGetUserHappyPath()
        {
            // Given
            var userId = Guid.NewGuid().ToString();

            var userResponse = new GetUserResponse 
            { 
                UserId = userId, 
                Email = "test",
                FirstName = "test",
                LastName = "test",
                MiddleInitial = "z" 
            };

            var userRepositoryMock = new Mock<IUserRepository>();

            var userProviderMock = new Mock<IUserProvider>();
            userProviderMock
                .Setup(x => x.GetUserAsync(userId))
                .ReturnsAsync(userResponse);

            var loggerMock = new Mock<ILogger<UserController>>();

            var controllerMock = new UserController(
                loggerMock.Object,
                userRepositoryMock.Object,
                userProviderMock.Object
                );

            // When
            var result = await controllerMock.GetUser(userId) as OkObjectResult;

            // Then
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(userResponse, result.Value);
        }

        [Fact]
        public async Task TestCreateUserHappyPath()
        {
            // Given
            var createUserRequest = new CreateUserRequest 
            { 
                FirstName = "test",
                Email= "test", 
                LastName = "test",
                MiddleInitial = "t", 
                Password = "password" 
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock
                .Setup(x => x.CreateUserAsync(It.IsAny<User>()))
                .ReturnsAsync(true);

            var userProviderMock = new Mock<IUserProvider>();

            var loggerMock = new Mock<ILogger<UserController>>();

            var controllerMock = new UserController(
                loggerMock.Object,
                userRepositoryMock.Object,
                userProviderMock.Object
                );

            // When
            var result = await controllerMock.CreateUserAsync(createUserRequest) as CreatedResult;

            // Then
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
            Assert.Equal(typeof(string), result.Value.GetType());
        }

        [Fact]
        public async Task TestUpdateUserHappyPath()
        {
            // Given
            var updateUserRequest = new UpdateUserRequest
            {
                FirstName =  "James",
                MiddleInitial =  "X",
                LastName =  "Holden",
                Password =  "test"
            };

            var userId = Guid.NewGuid().ToString();

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock
                .Setup(x => x.UpdateUserAsync(It.IsAny<string>(), It.IsAny<User>()))
                .ReturnsAsync(true);

            var loggerMock = new Mock<ILogger<UserController>>();

            var userProviderMock = new Mock<IUserProvider>();

            var controllerMock = new UserController(
                loggerMock.Object,
                userRepositoryMock.Object,
                userProviderMock.Object
                );

            // When
            var result = await controllerMock.UpdateUser(userId, updateUserRequest) as StatusCodeResult;

            // Then
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async Task TestLoginHappyPath()
        {
            // Given
            var email = "test@test.com";
            var password = "password";
            var userId = Guid.NewGuid().ToString();

            var loginRequest = new LoginRequest 
            { 
                Email= email,
                Password = password 
            };

            var userRepositoryMock = new Mock<IUserRepository>();

            var userProviderMock = new Mock<IUserProvider>();
            userProviderMock
                .Setup(x => x.ValidateUserAsync(email, password))
                .ReturnsAsync(userId);

            var loggerMock = new Mock<ILogger<UserController>>();

            var controllerMock = new UserController(
                loggerMock.Object,
                userRepositoryMock.Object,
                userProviderMock.Object
                );

            // When
            var result = await controllerMock.Login(loginRequest) as OkObjectResult;

            // Then
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(userId, result.Value);
        }
    }
}
