namespace GivingCircle.Api.Models
{
    public class IdentityRole
    {
        // The fundraiser's id
        public string UserId { get; set; }

        // The organizer's id
        public string ResourceId { get; set; }

        // The fundraiser's bank account information id
        public string Role { get; set; }
    }
}
