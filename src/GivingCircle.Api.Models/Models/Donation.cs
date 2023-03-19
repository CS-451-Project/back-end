using System;

namespace GivingCircle.Api.Models
{
    public class Donation
    {
        // The donation's id
        public string DonationId { get; set; }

        // The fundraiser's id
        public string FundraiserId { get; set; }

        // The donor's id
        public string UserId { get; set; }

        // The message left with the donation
        public string Message{ get; set; }

        // The donation date
        public DateTime Date { get; set; }

        // The amount donated to the fundraiser
        public double Amount { get; set; }

        // The donor's name, may be null
        public string Name { get; set; }
    }
}
