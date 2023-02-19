using GivingCircle.Api.Controllers;
using GivingCircle.Api.Fundraiser.DataAccess;
using GivingCircle.Api.Fundraiser.DataAccess.Responses;
using GivingCircle.Api.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace GivingCircle.Api.UnitTest.Controllers
{
    public class TestFundraiserController
    {
        [Fact]
        public async void TestFilterFundraisersHappyPath()
        {
            // Given
            FilterFundraisersRequest filterPropsRequest = new()
            {
                Title = "test",
                Tags = new string[] { "environment", "test", "huricane relief" }
            };

            var fundraiserRepositoryMock = new Mock<IFundraiserRepository>();

            var loggerMock = new Mock<ILogger<FundraiserController>>();

            var controllerMock = new FundraiserController(
                loggerMock.Object,
                fundraiserRepositoryMock.Object
                );

            // When
            var result = await controllerMock.FilterFundraisers(filterPropsRequest);

            // Then
        }


        [Fact]
        public async void TestListFundraisersByUserIdHappyPath()
        {
            // Given
            var userId = Guid.NewGuid().ToString();

            var fundraisers = new List<GetFundraiserResponse>()
            {
                new GetFundraiserResponse()
                {
                    FundraiserId = Guid.NewGuid().ToString(),
                    OrganizerId = userId,
                    PictureId = Guid.NewGuid().ToString(),
                    Title = "Test title 1",
                    Description = "test dscription 1",
                    CreatedDate = DateTime.Now,
                    PlannedEndDate = DateTime.Now.AddDays(90),
                    GoalTargetAmount = 9000.0,
                    CurrentBalanceAmount = 500.0,
                    Tags = new string[] { "environment", "test tag" }
                },
                new GetFundraiserResponse()
                {
                    FundraiserId = Guid.NewGuid().ToString(),
                    OrganizerId = userId,
                    PictureId = Guid.NewGuid().ToString(),
                    Title = "Test title 2",
                    Description = "test dscription 1",
                    CreatedDate = DateTime.Now,
                    PlannedEndDate = DateTime.Now.AddDays(90),
                    GoalTargetAmount = 9000.0,
                    CurrentBalanceAmount = 500.0,
                    Tags = new string[] { "test", "test tag" }
                },
            };

            var fundraiserRepositoryMock = new Mock<IFundraiserRepository>();
            fundraiserRepositoryMock.Setup(r => r.ListFundraisersByUserIdAsync(userId))
                .ReturnsAsync(fundraisers);

            var loggerMock = new Mock<ILogger<FundraiserController>>();

            var controllerMock = new FundraiserController(
                loggerMock.Object,
                fundraiserRepositoryMock.Object
                );

            // When
            var result = await controllerMock.ListFundraisersByUserId(userId) as OkObjectResult;

            // Then
            Assert.Equal(fundraisers, result.Value);
        }

        [Fact]
        public async void TestDeleteFundraiserHappyPath()
        {
            // Given
            var userId = Guid.NewGuid().ToString();

            var fundraiserRepositoryMock = new Mock<IFundraiserRepository>();
            fundraiserRepositoryMock.Setup(r => r.DeleteFundraiserAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var loggerMock = new Mock<ILogger<FundraiserController>>();

            var controllerMock = new FundraiserController(
                loggerMock.Object,
                fundraiserRepositoryMock.Object
                );

            // When
            var result = await controllerMock.DeleteFundraiser(userId) as StatusCodeResult;

            // Then
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void TestCreateFundraiserHappyPath()
        {
            // Given
            var createFundraiserRequest = new CreateFundraiserRequest
            {
                OrganizerId = "489DA2DA-6885-4099-A241-01111CDBFEB3",
                BankInformationId = "f336eb4d-ace0-4f4b-9c90-ac3c16096acf",
                Description = "test fundraiser description",
                Title = "Test fundraiser",
                PlannedEndDate = "12/12/2024",
                GoalTargetAmount = 200.00,
                Tags = new string[] { "environment", "disaster" }
            };

            var fundraiserRepositoryMock = new Mock<IFundraiserRepository>();
            fundraiserRepositoryMock.Setup(r => r.CreateFundraiserAsync(It.IsAny<Fundraiser.Models.Fundraiser>()))
                .ReturnsAsync(true);

            var loggerMock = new Mock<ILogger<FundraiserController>>();

            var controllerMock = new FundraiserController(
                loggerMock.Object,
                fundraiserRepositoryMock.Object
                );

            // When
            var result = await controllerMock.CreateFundraiser(createFundraiserRequest) as StatusCodeResult;

            // Then
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        }
    }
}
