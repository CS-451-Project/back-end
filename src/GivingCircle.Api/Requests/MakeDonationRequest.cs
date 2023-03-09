using System.ComponentModel.DataAnnotations;

namespace GivingCircle.Api.Requests
{
    public class MakeDonationRequest
    {
        // The fundraiser's id
        [Required]
        public string FundraiserId { get; set; }

        // The message left with the donation
        public string Message { get; set; }

        // The amount donated to the fundraiser
        [Required]
        public double Amount { get; set; }
    }
}
