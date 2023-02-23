using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http.Json;
using GivingCircle.Api.Requests;

namespace GivingCircle.Api.IntegrationTest.Services
{
    public class TestFundraiserService
    {
        [Fact]
        public async Task TestCreateSearchDeleteFundraiser()
        {
            // Given
            string url = "https://localhost:7000/api";

            var application = new WebApplicationFactory<Program>();

            var httpClient = application.CreateClient();

            var createFundraiserRequest = new CreateFundraiserRequest
            {
                OrganizerId = "489DA2DA-6885-4099-A241-01111CDBFEB3",
                BankInformationId = "f336eb4d-ace0-4f4b-9c90-ac3c16096acf",
                Description = "test fundraiserSerialized description",
                Title = "Test fundraiserSerialized",
                PlannedEndDate = DateTime.Now.AddMonths(2).ToString(),
                GoalTargetAmount = 200.00,
                Tags = new string[] { "test1", "test2" }
            };

            // Set the base address
            application.Server.BaseAddress = new Uri(url);

            // Create a fundraiser
            var response = await httpClient.PostAsJsonAsync(url + "/fundraiser", createFundraiserRequest);
            response.EnsureSuccessStatusCode();

            // Get its id
            var fundraiserId = response.Content.ReadAsStringAsync().Result.ToString();

            FilterFundraisersRequest filterFundraisersRequest = new()
            {
                Title = "Test fundraiserSerialized",
                Tags = new string[] { "test1", "blah" },
                CreatedDateOffset = 1.0
            };

            // Try to search
            response = await httpClient.PostAsJsonAsync(url + "/fundraiser/filter", filterFundraisersRequest);
            response.EnsureSuccessStatusCode();

            // get fundraiser we just created to try and delete
            response = await httpClient.GetAsync(url + $"/user/fundraiser/{fundraiserId}");

            // Try to update the fundraiser
            UpdateFundraiserRequest updateFundraiserRequest = new() {
                FundraiserId = fundraiserId,
                Description = "",
                Title = "",
                PlannedEndDate = "08/04/2023",
                GoalTargetAmount = 1000.00,
                Tags = new string[] { "testtesttest", }
            };

            response = await httpClient.PutAsJsonAsync(url + "/fundraiser", updateFundraiserRequest);
            response.EnsureSuccessStatusCode();

            // Soft Delete the created fundraiser
            response = await httpClient.DeleteAsync(url + "/fundraiser" + $"/{fundraiserId}/close");
            response.EnsureSuccessStatusCode();

            // Hard Delete the created fundraiserSerialized
            response = await httpClient.DeleteAsync(url + $"/fundraiser/{fundraiserId}");
            response.EnsureSuccessStatusCode();
        }
    }
}
