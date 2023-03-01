using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using GivingCircle.Api.Providers;
using GivingCircle.Api.Models;
using GivingCircle.Api.DataAccess.Responses;

namespace GivingCircle.Api.Authorization
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserProvider _userProvider;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserProvider userProvider)
            : base(options, logger, encoder, clock) 
        { 
            _userProvider = userProvider;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // The user id returned, if the user exists
            string userId;

            // Reject if there isn't an authorization header
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("No header found");
            }

            // Get the authorization header values
            var headerValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

            // Fail if the authorization credentials are null
            if (headerValue.Parameter == null)
            {
                return AuthenticateResult.Fail("Null authorization header");
            }

            // Convert the base64 format to regular format
            var headerValueBytes = Convert.FromBase64String(headerValue.Parameter);
            var credentials = Encoding.UTF8.GetString(headerValueBytes);

            // If bad credentials then unauthorized
            if (string.IsNullOrEmpty(credentials))
            {
                return AuthenticateResult.Fail("Email password combo invalid");
            }

            // Get the givenUsername and the givenPassword from the header 
            string[] credentialsArray = credentials.Split(":");
            string givenEmail = credentialsArray[0];
            string givenPassword = credentialsArray[1];

            try
            {
                // See if the user is valid
                userId = await _userProvider.ValidateUserAsync(givenEmail, givenPassword);
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex);
                return AuthenticateResult.Fail("Email password combo invalid");
            }
            
            if (userId == null || string.Equals(userId, ""))
            {
                return AuthenticateResult.Fail("Email password combo invalid");
            }

            // Generate ticket
            // Add the user's id to the claims
            var claim = new[] { new Claim("UserId", userId) }; 
            var identity = new ClaimsIdentity(claim, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
