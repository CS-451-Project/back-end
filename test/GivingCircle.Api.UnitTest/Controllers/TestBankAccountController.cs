using GivingCircle.Api.Controllers;
using GivingCircle.Api.DataAccess.Repositories;
using GivingCircle.Api.Models;
using GivingCircle.Api.Requests.FundraiserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace GivingCircle.Api.UnitTest.Controllers
{
    public class TestBankAccountController
    {
        [Fact]
        public async void TestAddBankAccountAsync()
        {
            // Given
            var userId = Guid.NewGuid().ToString();

            var addBankAccountRequest = new AddBankAccountRequest
            {
                Account_Name = "Austin Nguyen",
                Address = "55st hollywood blvd",
                City = "Los Angeles",
                State = "Kansas",
                Zipcode = "10101",
                Bank_Name = "Austin Bank",
                Account_Num = "87870086",
                Routing_Num = "402984494",
                Account_Type = "Checkings",
            };

            var bankAccountRepositoryMock = new Mock<IBankAccountRepository>();
            bankAccountRepositoryMock.Setup(r => r.AddBankAccount(userId, It.IsAny<BankAccount>()))
                .ReturnsAsync(true);

            var loggerMock = new Mock<ILogger<BankAccountController>>();

            var controllerMock = new BankAccountController(
                loggerMock.Object,
                bankAccountRepositoryMock.Object);

            // When
            var result = await controllerMock.AddBankAccount(userId, addBankAccountRequest) as CreatedResult;

            // Then
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        }

        [Fact]
        public async void TestDeleteBankAccountAsync()
        {
            // Given
            var Bank_Account_Id = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid().ToString();

            var bankAccountRepositoryMock = new Mock<IBankAccountRepository>();
            bankAccountRepositoryMock.Setup(r => r.DeleteBankAccountAsync(userId, Bank_Account_Id))
                .ReturnsAsync(true);

            var loggerMock = new Mock<ILogger<BankAccountController>>();

            var controllerMock = new BankAccountController(
                loggerMock.Object,
                bankAccountRepositoryMock.Object);

            // When
            var result = await controllerMock.DeleteBankAccount(userId, Bank_Account_Id) as StatusCodeResult;

            // Then
            Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
        }

        [Fact]
        public async void TestGetBankAccountAsync()
        {
            // Given
            var bankAccountId = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid().ToString();

            var bankAccount = new BankAccount
            {
                Account_Name = "Austin Nguyen",
                Address = "55st hollywood blvd",
                City = "Los Angeles",
                State = "KS",
                Zipcode = "10101",
                Bank_Name = "Austin Bank",
                Account_Num = "87870086",
                Routing_Num = "402984494",
                Account_Type = "Checkings",
                Bank_Account_Id = bankAccountId,
            };

            var bankAccountRepositoryMock = new Mock<IBankAccountRepository>();
            bankAccountRepositoryMock
                .Setup(r => r.GetBankAccount(userId, bankAccountId))
                .ReturnsAsync(bankAccount);

            var loggerMock = new Mock<ILogger<BankAccountController>>();

            var controllerMock = new BankAccountController(
                loggerMock.Object,
                bankAccountRepositoryMock.Object);

            // When
            var result = await controllerMock.GetAccount(userId, bankAccountId) as OkObjectResult;

            // Then
            Assert.Equal(bankAccount, result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
