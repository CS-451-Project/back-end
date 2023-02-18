using GivingCircle.Api.Controllers;
using GivingCircle.Api.Fundraiser.DataAccess;
using GivingCircle.Api.Requests.FundraiserService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GivingCircle.Api.UnitTest.Controllers
{
    public class TestFundraiserController
    {
        [Fact]
        public void TestCreateFundraiserHappyPath()
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
            var result = controllerMock.CreateFundraiser(createFundraiserRequest);

            // Then
            var ok = result.Result as CreatedResult;
        }
    }
}
