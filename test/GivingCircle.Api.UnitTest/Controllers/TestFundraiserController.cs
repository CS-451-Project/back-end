using GivingCircle.Api.Controllers;
using GivingCircle.Api.DataAccess.Repositories;
using GivingCircle.Api.DataAccess.Responses;
using GivingCircle.Api.Models;
using GivingCircle.Api.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace GivingCircle.Api.UnitTest.Controllers
{
    public class TestFundraiserController
    {
        [Fact]
        public async void TestGetFundraiserHappyPath()
        {
            // Given
            var userId = Guid.NewGuid().ToString();

            var fundraiserId = Guid.NewGuid().ToString();

            var fundraiser = new GetFundraiserResponse
            {
                FundraiserId = fundraiserId,
                OrganizerId = userId,
                PictureId = Guid.NewGuid().ToString(),
                Title = "Test title 1",
                Description = "test dscription 1",
                CreatedDate = DateTime.Now,
                PlannedEndDate = DateTime.Now.AddDays(90),
                GoalTargetAmount = 9000.0,
                CurrentBalanceAmount = 500.0,
                Tags = new string[] { "environment", "test tag" }
            };

            var fundraiserRepositoryMock = new Mock<IFundraiserRepository>();
            fundraiserRepositoryMock.Setup(r => r.GetFundraiserAsync(fundraiserId))
                .ReturnsAsync(fundraiser);

            var loggerMock = new Mock<ILogger<FundraiserController>>();

            var controllerMock = new FundraiserController(
                loggerMock.Object,
                fundraiserRepositoryMock.Object
                );

            // When
            var result = await controllerMock.GetFundraiser(fundraiserId) as OkObjectResult;

            // Then
            Assert.Equal(fundraiser, result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void TestUpdateFundraiserHappyPath()
        {
            // Given
            var updateFundraiserRequest = new UpdateFundraiserRequest
            {
                Description = "test fundraiser description",
                Title = "Test fundraiser",
                PlannedEndDate = "12/12/2024",
                GoalTargetAmount = 200.00,
                Tags = new string[] { "environment", "disaster" }
            };

            var fundraiserId = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid().ToString();

            var fundraiserRepositoryMock = new Mock<IFundraiserRepository>();
            fundraiserRepositoryMock
                .Setup(x => x.UpdateFundraiserAsync(userId, fundraiserId, It.IsAny<Fundraiser>()))
                .ReturnsAsync(true);

            var loggerMock = new Mock<ILogger<FundraiserController>>();

            var controllerMock = new FundraiserController(
                loggerMock.Object,
                fundraiserRepositoryMock.Object
                );

            // When
            var result = await controllerMock.UpdateFundraiser(userId, fundraiserId, updateFundraiserRequest) as StatusCodeResult;

            // Then
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void TestFilterFundraisersHappyPath()
        {
            // Given
            FilterFundraisersRequest filterFundraisersRequest = new()
            {
                Title = "test",
                Tags = new string[] { "environment", "test", "huricane relief" },
                CreatedDateOffset = 2.0,
                EndDateOffset = 11.0,
                OrderBy = "ClosestToTargetGoal",
                Ascending = true
            };

            var fundraisers = new List<GetFundraiserResponse>()
            {
                new GetFundraiserResponse()
                {
                    FundraiserId = Guid.NewGuid().ToString(),
                    OrganizerId = Guid.NewGuid().ToString(),
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
                    OrganizerId = Guid.NewGuid().ToString(),
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
            fundraiserRepositoryMock
                .Setup(x => x.FilterFundraisersAsync(It.IsAny<Dictionary<string, string[]>>()))
                .ReturnsAsync(fundraisers);

            var loggerMock = new Mock<ILogger<FundraiserController>>();

            var controllerMock = new FundraiserController(
                loggerMock.Object,
                fundraiserRepositoryMock.Object
                );

            // When
            var result = await controllerMock.FilterFundraisers(filterFundraisersRequest) as OkObjectResult;

            // Then
            Assert.Equal(fundraisers, result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
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
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void TestHardDeleteFundraisersHappyPath()
        {
            // Given
            var fundraiserId = Guid.NewGuid().ToString();

            var userId = Guid.NewGuid().ToString();

            var fundraiserRepositoryMock = new Mock<IFundraiserRepository>();
            fundraiserRepositoryMock.Setup(r => r.HardDeleteFundraiserAsync(userId, It.IsAny<string>()))
                .ReturnsAsync(true);

            var loggerMock = new Mock<ILogger<FundraiserController>>();

            var controllerMock = new FundraiserController(
                loggerMock.Object,
                fundraiserRepositoryMock.Object
                );

            // When
            var result = await controllerMock.DeleteFundraiser(userId, fundraiserId) as StatusCodeResult;

            // Then
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void TestDeleteFundraiserHappyPath()
        {
            // Given
            var fundraiserId = Guid.NewGuid().ToString();

            var userId = Guid.NewGuid().ToString();

            var fundraiserRepositoryMock = new Mock<IFundraiserRepository>();
            fundraiserRepositoryMock.Setup(r => r.DeleteFundraiserAsync(userId, It.IsAny<string>()))
                .ReturnsAsync(true);

            var loggerMock = new Mock<ILogger<FundraiserController>>();

            var controllerMock = new FundraiserController(
                loggerMock.Object,
                fundraiserRepositoryMock.Object
                );

            // When
            var result = await controllerMock.CloseFundraiser(userId, fundraiserId) as StatusCodeResult;

            // Then
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void TestCreateFundraiserHappyPath()
        {
            // Given
            var userId = Guid.NewGuid().ToString();

            var createFundraiserRequest = new CreateFundraiserRequest
            {
                Description = "test fundraiser description",
                Title = "Test fundraiser",
                PlannedEndDate = "12/12/2024",
                GoalTargetAmount = 200.00,
                Tags = new string[] { "environment", "disaster" }
            };

            var fundraiserRepositoryMock = new Mock<IFundraiserRepository>();
            fundraiserRepositoryMock.Setup(r => r.CreateFundraiserAsync(userId, It.IsAny<Fundraiser>()))
                .ReturnsAsync(true);

            var loggerMock = new Mock<ILogger<FundraiserController>>();

            var controllerMock = new FundraiserController(
                loggerMock.Object,
                fundraiserRepositoryMock.Object
                );

            // When
            var result = await controllerMock.CreateFundraiser(userId, createFundraiserRequest) as CreatedResult;

            // Then
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
            Assert.Equal(typeof(string), result.Value.GetType());
        }
    }
}
