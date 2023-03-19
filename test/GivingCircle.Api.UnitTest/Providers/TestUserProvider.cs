using GivingCircle.Api.DataAccess.Repositories;
using GivingCircle.Api.DataAccess.Responses;
using GivingCircle.Api.Providers;
using Moq;
using System;
using Xunit;

namespace GivingCircle.Api.UnitTest.Providers
{
    public class TestUserProvider
    {
        [Fact]
        public async void TestGetUserHappyPath()
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
            userRepositoryMock
                .Setup(x => x.GetUserAsync(userId))
                .ReturnsAsync(userResponse);

            var providerMock = new UserProvider(
                userRepositoryMock.Object
                );

            // When
            var result = await providerMock.GetUserAsync(userId);

            // Then
            Assert.Equal(userResponse, result);
        }

        [Fact]
        public async void TestValidateUserHappyPath()
        {
            // Given
            var testEmail = "test@test.com";
            var password = "password";
            var userId = Guid.NewGuid().ToString();

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock
                .Setup(x => x.ValidateUserAsync(testEmail, password))
                .ReturnsAsync(userId);

            var providerMock = new UserProvider(
                userRepositoryMock.Object
                );

            // When
            var result = await providerMock.ValidateUserAsync(testEmail, password);

            // Then
            Assert.Equal(userId, result);
        }
    }
}
