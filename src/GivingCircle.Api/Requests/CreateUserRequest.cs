using System.ComponentModel.DataAnnotations;

namespace GivingCircle.Api.Requests
{
    /// <summary>
    /// A create user request for 
    /// <see cref="Controllers.UserController.CreateUser(string, string, string, string, string, string)"/>
    /// </summary>
    public class CreateUserRequest
    {
        // The user's name items
        [Required]
        public string FirstName { get; set; }

        public string MiddleInitial { get; set; }

        [Required]
        public string LastName { get; set; }

        // The user's password
        [Required]
        public string Password { get; set; }

        // The users email
        [Required]
        public string Email { get; set; }

    }
}
