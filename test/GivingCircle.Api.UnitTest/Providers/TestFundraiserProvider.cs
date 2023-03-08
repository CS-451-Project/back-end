using GivingCircle.Api.DataAccess.Repositories;
using GivingCircle.Api.Providers;
using Moq;
using System;
using Xunit;

namespace GivingCircle.Api.UnitTest.Providers
{
    public class TestFundraiserProvider
    {
        [Fact]
        public async void TestMakeDonationHappyPath()
        {
            // Given
            var amount = 3.00;
            var fundraiserId = Guid.NewGuid().ToString();

            var fundraiserRepositoryMock = new Mock<IFundraiserRepository>();
            fundraiserRepositoryMock
                .Setup(x => x.MakeDonation(fundraiserId, amount))
                .ReturnsAsync(true);

            var providerMock = new FundraiserProvider(
                fundraiserRepositoryMock.Object
                );

            // When
            var result = await providerMock.MakeDonation(fundraiserId, amount);

            // Then
            Assert.True( result );
        }
    }
}
