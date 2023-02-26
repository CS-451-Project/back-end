using System;

namespace GivingCircle.Api.DataAccess.Responses
{
    public class GetUserResponse
    {
        // The users's id
        public string UserId { get; set; }

        // The user's name items
        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }

        // The user's password
        public string Password { get; set; }

        // The user's email
        public string Email { get; set; }

    }
}