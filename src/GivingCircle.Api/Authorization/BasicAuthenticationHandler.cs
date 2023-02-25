using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace GivingCircle.Api.Authorization
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock) 
        { 
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
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
            string givenUsername = credentialsArray[0];
            string givenPassword = credentialsArray[1];

            // Check if this user exists
            // var user = await _userRepository.GetUserByUsernameAsync(givenUsername);

            string userId = "489DA2DA-6885-4099-A241-01111CDBFEB3";

            //if (givenUsername != user.Username || givenPassword != user.Password)
            //{
            //    return AuthenticateResult.Fail("Username givenPassword combo invalid");
            //}

            // Generate ticket
            var claim = new[] { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claim, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
