using GivingCircle.Api.Requests;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GivingCircle.Api.IntegrationTest.Services
{
    public class TestUserService
    {
        [Fact]
        public async Task TestCreateUserLoginAndDelete()
        {
            // Given
            string url = "https://localhost:7000/api";

            var application = new WebApplicationFactory<Program>();

            var httpClient = application.CreateClient();

            var createUserRequest = new CreateUserRequest
            {
                FirstName = "test",
                Email = "test@test.com",
                LastName = "test",
                MiddleInitial = "t",
                Password = "password"
            };

            // Set the base address
            application.Server.BaseAddress = new Uri(url);

            // Create a user
            var response = await httpClient.PostAsJsonAsync(url + "/user", createUserRequest);
            response.EnsureSuccessStatusCode();

            // Get its id
            var userId = response.Content.ReadAsStringAsync().Result.ToString();
            Assert.Equal(typeof(string), userId.GetType());

            var loginRequest = new LoginRequest 
            { 
                Email = createUserRequest.Email,  
                Password = createUserRequest.Password 
            };

            // Login
            response = await httpClient.PostAsJsonAsync(url + "/user/login", loginRequest);
            response.EnsureSuccessStatusCode();
            Assert.Equal(response.Content.ReadAsStringAsync().Result.ToString(), userId);

            // Create the authorization parameter
            var parameter = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{createUserRequest.Email}:{createUserRequest.Password}"));

            // Set the authorization header
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("BasicAuthentication", parameter);

            // Delete the user
            response = await httpClient.DeleteAsync(url + $"/user/{userId}");
            response.EnsureSuccessStatusCode();
        }
    }
}
