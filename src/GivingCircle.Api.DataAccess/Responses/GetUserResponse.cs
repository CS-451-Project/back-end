namespace GivingCircle.Api.DataAccess.Responses
{
    /// <summary>
    /// The db response for a user. Note that we should never return the password.
    /// </summary>
    public class GetUserResponse
    {
        // The users's id
        public string UserId { get; set; }

        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }

        // The user's email
        public string Email { get; set; }

    }
}