using System.ComponentModel.DataAnnotations;

namespace GivingCircle.Api.Requests.FundraiserService
{
    /// <summary>
    /// A create fundraiser request for 
    /// <see cref="Controllers.FundraiserController.CreateFundraiser(string, string, string, double, string, string, string, string[])"/>
    /// </summary>
    public class CreateFundraiserRequest
    {
        // The organizer's id
        [Required]
        public string OrganizerId { get; set; }

        // The fundraiser's bank account information id
        [Required]
        public string BankInformationId { get; set; }

        // The fundraiser's picture's id
        public string PictureId { get; set; }

        // The fundraiser description
        public string Description { get; set; }

        // The fundraisers displayed name / title
        [Required]
        public string Title { get; set; }

        // The planned end date
        [Required]
        public string PlannedEndDate { get; set; }

        // The fundraiser's target amount to raise
        [Required]
        public double GoalTargetAmount { get; set; }

        // The tags 
        public string[] Tags { get; set; }
    }
}
