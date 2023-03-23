using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http.Json;
using GivingCircle.Api.Requests;
using System.Net.Http.Headers;
using System.Text;

namespace GivingCircle.Api.IntegrationTest.Services
{
    public class TestFundraiserService
    {
        [Fact]
        public async Task TestCreateSearchDeleteFundraiser()
        {
            // Given
            string url = "https://localhost:7000/api";

            string userId = "575B1943-CD13-4771-B698-DE4E1F4E22A7";

            var application = new WebApplicationFactory<Program>();

            var httpClient = application.CreateClient();

            var createFundraiserRequest = new CreateFundraiserRequest
            {
                Description = "test fundraiser description",
                Title = "Test fundraiser",
                PlannedEndDate = DateTime.Now.AddMonths(2).ToString(),
                GoalTargetAmount = 200.00,
                Tags = new string[] { "test1", "test2" }
            };

            // Set the base address
            application.Server.BaseAddress = new Uri(url);

            // Create the authorization parameter
            var parameter = Convert.ToBase64String(Encoding.UTF8.GetBytes("azdummy@gmail.com:test"));

            // Set the authorization header
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("BasicAuthentication", parameter);

            // Create a fundraiser
            var response = await httpClient.PostAsJsonAsync(url + $"/user/{userId}/fundraiser", createFundraiserRequest);
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
            response = await httpClient.PostAsJsonAsync(url + "/fundraiser", filterFundraisersRequest);
            response.EnsureSuccessStatusCode();

            // get fundraiser we just created to try and delete
            response = await httpClient.GetAsync(url + $"/fundraiser/{fundraiserId}");
            response.EnsureSuccessStatusCode();

            // Try to update the fundraiser
            UpdateFundraiserRequest updateFundraiserRequest = new() 
            {
                Description = "",
                Title = "",
                PlannedEndDate = "08/04/2023",
                GoalTargetAmount = 1000.00,
                Tags = new string[] { "testtesttest", }
            };

            response = await httpClient.PutAsJsonAsync(url + $"/user/{userId}/fundraiser/{fundraiserId}", updateFundraiserRequest);
            response.EnsureSuccessStatusCode();

            // Soft Delete the created fundraiser
            response = await httpClient.DeleteAsync(url + $"/user/{userId}/fundraiser/{fundraiserId}/close");
            response.EnsureSuccessStatusCode();

            // Hard Delete the created fundraiserSerialized
            response = await httpClient.DeleteAsync(url + $"/user/{userId}/fundraiser/{fundraiserId}");
            response.EnsureSuccessStatusCode();
        }
    }
}
