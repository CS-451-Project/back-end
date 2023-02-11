using GivingCircle.Api.Controllers;
using GivingCircle.Api.Models;
using GivingCircle.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace GivingCircle.Api.UnitTest.Controllers
{
    public class TestFundraiserController
    {
        [Fact]
        public void TestListFundraisers()
        {
            // Given
            var fundraiser = new Fundraiser
            {
                FundraiserId = "123456789",
                Name = "TestFundraiserController"
            };

            var fundraiserServiceMock = new Mock<IFundraiserService>();
            fundraiserServiceMock.Setup(service => service.ListAllFundraisersAsync())
                .ReturnsAsync(new List<Fundraiser> { fundraiser });

            var loggerMock = new Mock<ILogger<FundraiserController>>();

            var controllerMock = new FundraiserController(
                loggerMock.Object,
                fundraiserServiceMock.Object
                );

            // When
            var result = controllerMock.ListFundraisers();

            // Then
            var ok = result.Result as OkObjectResult;
            Assert.NotNull(ok);
        }
    }
}
