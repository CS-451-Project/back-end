using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http.Json;
using GivingCircle.Api.Requests;
using GivingCircle.Api.Fundraiser.DataAccess.Responses;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace GivingCircle.Api.IntegrationTest.Services
{
    public class TestFundraiserService
    {
        [Fact]
        public async Task TestCreateSearchDeleteFundraiser()
        {
            // Given
            string url = "https://localhost:7000/api/fundraisers";

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

            // When

            // Create fundraiser
            var response = await httpClient.PostAsJsonAsync(url, createFundraiserRequest);

            // Then
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            // And when

            FilterFundraisersRequest filterFundraisersRequest = new()
            {
                Title = "Test fundraiserSerialized",
                Tags = new string[] { "test1", "blah" },
                CreatedDateOffset = 1.0
            };

            // Try to search for the fundraiserSerialized we just created
            response = await httpClient.PostAsJsonAsync(url + "/filter", filterFundraisersRequest);

            var fundraiserSerialized = await response.Content.ReadAsStringAsync();

            // deserilize the response
            IEnumerable<GetFundraiserResponse> getFundraiserResponse = JsonConvert.DeserializeObject<IEnumerable<GetFundraiserResponse>>(fundraiserSerialized);

            // get fundraiserSerialized we just created to try and delete
            var fundraiserId = getFundraiserResponse.ElementAt(0).FundraiserId;

            // And when

            // Update the fundraiser
            UpdateFundraiserRequest updateFundraiserRequest = new() {
                FundraiserId = fundraiserId,
                Description = "",
                Title = "",
                PlannedEndDate = "08/04/2023",
                GoalTargetAmount = 1000.00,
                Tags = new string[] { "testtesttest", }
            };

            response = await httpClient.PutAsJsonAsync(url, updateFundraiserRequest);

            // Then
            response.EnsureSuccessStatusCode();

            // And when

            // Soft Delete the created fundraiserSerialized
            response = await httpClient.DeleteAsync(url + $"/{fundraiserId}");

            // Then
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            // And when

            // Hard Delete the created fundraiserSerialized
            response = await httpClient.DeleteAsync(url + $"/delete/{fundraiserId}");

            // Then
            response.EnsureSuccessStatusCode(); // Status Code 200-299
        }
    }
}
