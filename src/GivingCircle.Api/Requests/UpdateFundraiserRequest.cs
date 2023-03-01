namespace GivingCircle.Api.Requests
{
    public class UpdateUserRequest
    {
        // The User's Name
        public string FirstName { get; set; }

  
        public string MiddleInitial { get; set; }

        public string LastName { get; set; }

        // The user's password
        public string Password { get; set; }

        // The user's email
        public double Email { get; set; }
    }
}
