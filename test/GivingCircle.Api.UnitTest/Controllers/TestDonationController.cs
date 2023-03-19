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
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GivingCircle.Api.UnitTest.Controllers
{
    public class TestDonationController
    {
        [Fact]
        public async Task TestGetFundraiserDonationsHappyPath()
        {
            // Given
            var fundraiserId = Guid.NewGuid().ToString();

            var donations = new List<Donation>()
            {
                new Donation
                {
                    Amount = 5.00,
                    Date= DateTime.UtcNow,
                    DonationId = Guid.NewGuid().ToString(),
                    FundraiserId = fundraiserId,
                    Message = "test1",
                    UserId = Guid.NewGuid().ToString()
                },
                new Donation
                {
                    Amount = 10.00,
                    Date= DateTime.UtcNow,
                    DonationId = Guid.NewGuid().ToString(),
                    FundraiserId = fundraiserId,
                    Message = "test2",
                    UserId = Guid.NewGuid().ToString()
                },
                new Donation
                {
                    Amount = 7.00,
                    Date= DateTime.UtcNow,
                    DonationId = Guid.NewGuid().ToString(),
                    FundraiserId = fundraiserId,
                    Message = "test3",
                    UserId = Guid.NewGuid().ToString()
                }
            };

            var donationRepositoryMock = new Mock<IDonationRepository>();
            donationRepositoryMock
                .Setup(x => x.GetFundraiserDonations(fundraiserId))
                .ReturnsAsync(donations);

            var fundraiserProviderMock = new Mock<IFundraiserProvider>();

            var userProviderMock = new Mock<IUserProvider>();

            var loggerMock = new Mock<ILogger<DonationController>>();

            DonationController donationController = new(
                loggerMock.Object,
                donationRepositoryMock.Object,
                fundraiserProviderMock.Object,
                userProviderMock.Object
                );

            // When
            var result = await donationController.GetFundraiserDonations(fundraiserId) as OkObjectResult;

            // Then
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(donations, result.Value);
        }

        [Fact]
        public async Task TestMakeUserDonationHappyPath()
        {
            // Given
            var userId = Guid.NewGuid().ToString();

            var makeDonationRequest = new MakeDonationRequest
            {
                Amount = 5.00,
                FundraiserId = Guid.NewGuid().ToString(),
                Message = "test"
            };

            var getUserResponse = new GetUserResponse 
            {
                UserId = userId, 
                Email = "test@test.com", 
                FirstName = "test", 
                LastName = "test", 
                MiddleInitial = "t" 
            };

            var donationRepositoryMock = new Mock<IDonationRepository>();
            donationRepositoryMock
                .Setup(x => x.MakeDonation(It.IsAny<Donation>()))
                .ReturnsAsync(true);

            var fundraiserProviderMock = new Mock<IFundraiserProvider>();
            fundraiserProviderMock
                .Setup(x => x.MakeDonation(makeDonationRequest.FundraiserId, makeDonationRequest.Amount))
                .ReturnsAsync(true);

            var userProviderMock = new Mock<IUserProvider>();
            userProviderMock
                .Setup(x => x.GetUserAsync(userId))
                .ReturnsAsync(getUserResponse);

            var loggerMock = new Mock<ILogger<DonationController>>();

            DonationController donationController = new(
                loggerMock.Object,
                donationRepositoryMock.Object,
                fundraiserProviderMock.Object,
                userProviderMock.Object
                );

            // When

            var result = await donationController.MakeUserDonation(userId, makeDonationRequest) as NoContentResult;

            // Then

            Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
        }

        [Fact]
        public async Task TestMakeAnonymousDonationHappyPath()
        {
            // Given
            var makeDonationRequest = new MakeDonationRequest
            {
                Amount = 5.00,
                FundraiserId = Guid.NewGuid().ToString(),
                Message = "test"
            };

            var donationRepositoryMock = new Mock<IDonationRepository>();
            donationRepositoryMock
                .Setup(x => x.MakeDonation(It.IsAny<Donation>()))
                .ReturnsAsync(true);

            var fundraiserProviderMock = new Mock<IFundraiserProvider>();
            fundraiserProviderMock
                .Setup(x => x.MakeDonation(makeDonationRequest.FundraiserId, makeDonationRequest.Amount))
                .ReturnsAsync(true);

            var userProviderMock = new Mock<IUserProvider>();

            var loggerMock = new Mock<ILogger<DonationController>>();

            DonationController donationController = new(
                loggerMock.Object,
                donationRepositoryMock.Object,
                fundraiserProviderMock.Object,
                userProviderMock.Object
                );

            // When

            var result = await donationController.MakeAnonymousDonation(makeDonationRequest) as NoContentResult;

            // Then

            Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
        }
    }
}
