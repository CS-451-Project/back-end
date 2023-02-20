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
                Description = "test fundraiser description",
                Title = "Test fundraiser",
                PlannedEndDate = DateTime.Now.AddMonths(2).ToString(),
                GoalTargetAmount = 200.00,
                Tags = new string[] { "test1", "test2" }
            };

            // Set the base address
            application.Server.BaseAddress = new Uri(url);

            // When

            // Create a fundraiser
            var response = await httpClient.PostAsJsonAsync(url, createFundraiserRequest);

            // Then
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            // And retrieve the fundraiser and soft delete
            FilterFundraisersRequest filterFundraisersRequest = new()
            {
                Title = "Test fundraiser",
                Tags = new string[] { "test1", "blah" },
                CreatedDateOffset = 1.0
            };

            // Try to search for the fundraiser we just created
            response = await httpClient.PostAsJsonAsync(url + "/filter", filterFundraisersRequest);

            var fundraiserToDelete = await response.Content.ReadAsStringAsync();

            // deserilize the response
            IEnumerable<GetFundraiserResponse> getFundraiserResponse = JsonConvert.DeserializeObject<IEnumerable<GetFundraiserResponse>>(fundraiserToDelete);

            // get fundraiser we just created to try and delete
            var fundraiserIdToDelete = getFundraiserResponse.ElementAt(0).FundraiserId;

            // Soft Delete the created fundraiser
            response = await httpClient.DeleteAsync(url + $"/{fundraiserIdToDelete}");

            // Then
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            // And hard delete the fundraiser

            // Hard Delete the created fundraiser
            response = await httpClient.DeleteAsync(url + $"/delete/{fundraiserIdToDelete}");

            // Then
            response.EnsureSuccessStatusCode(); // Status Code 200-299
        }
    }
}
