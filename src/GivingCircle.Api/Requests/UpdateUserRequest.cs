using System.ComponentModel.DataAnnotations;

namespace GivingCircle.Api.Requests
{
    public class UpdateUserRequest
    {
        // The User's Name
        [Required]
        public string FirstName { get; set; }

        public string MiddleInitial { get; set; }

        [Required]
        public string LastName { get; set; }

        // The user's password
        [Required]
        public string Password { get; set; }
    }
}
