using GivingCircle.Api.DataAccess.Repositories;
using GivingCircle.Api.DataAccess.Responses;
using GivingCircle.Api.Providers;
using Moq;
using Xunit;

namespace GivingCircle.Api.UnitTest.Providers
{
    public class TestUserProvider
    {
        [Fact]
        public async void TestGetUserByEmailHappyPath()
        {
            // Given
            var testEmail = "test@test.com";

            GetUserResponse user = new() 
            { 
                Email= testEmail,
                FirstName = "test",
                LastName = "testlast",
                MiddleInitial = "z",
                Password = "test", 
                UserId = "1234546" 
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock
                .Setup(x => x.GetUserByEmailAsync(testEmail))
                .ReturnsAsync(user);

            var providerMock = new UserProvider(
                userRepositoryMock.Object
                );

            // When
            var result = await providerMock.GetUserByEmailAsync(testEmail);

            // Then
            Assert.Equal(user, result);
        }
    }
}
