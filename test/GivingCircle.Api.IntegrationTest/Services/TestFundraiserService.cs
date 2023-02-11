using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Threading.Tasks;
using Xunit;

namespace GivingCircle.Api.IntegrationTest.Services
{
    public class TestFundraiserService
    {
        [Fact]
        public async Task TestListFundraisersHappyPath()
        {
            // Given
            string baseUrl = "https://localhost:3000/api";

            var application = new WebApplicationFactory<Program>();

            var httpClient = application.CreateClient();

            // Reset the base address
            application.Server.BaseAddress = new Uri(baseUrl);

            string url = baseUrl + "/fundraisers";

            // When
            var response = await httpClient.GetAsync(url);

            // Then
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            Assert.NotEmpty(await response.Content.ReadAsStringAsync());
        }
    }
}
