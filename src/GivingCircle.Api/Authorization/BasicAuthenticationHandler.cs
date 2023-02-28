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
            // The user retrieved by the provided email
            GetUserResponse user;

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
                return AuthenticateResult.Fail("Unauthorized");
            }

            // Get the givenUsername and the givenPassword from the header 
            string[] credentialsArray = credentials.Split(":");
            string givenEmail = credentialsArray[0];
            string givenPassword = credentialsArray[1];

            try
            {
                // Get the user by the given email
                user = await _userProvider.GetUserByEmailAsync(givenEmail);
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex);
                return AuthenticateResult.Fail("Username password combo invalid");
            }
            
            if (user == null)
            {
                return AuthenticateResult.Fail("Username password combo invalid");
            }

            // If the returned user doesn't match what we're given, then return authentication failure
            if (givenEmail != user.Email || givenPassword != user.Password)
            {
                return AuthenticateResult.Fail("Username password combo invalid");
            }

            // Generate ticket
            // Add the user's id to the claims
            var claim = new[] { new Claim("UserId", user.UserId) }; 
            var identity = new ClaimsIdentity(claim, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
