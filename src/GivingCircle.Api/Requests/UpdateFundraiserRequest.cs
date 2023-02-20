using System.ComponentModel.DataAnnotations;

namespace GivingCircle.Api.Requests
{
    public class UpdateFundraiserRequest
    {
        // The fundraiser's id
        [Required]
        public string FundraiserId { get; set; }

        // The fundraiser description
        public string Description { get; set; }

        // The fundraisers displayed name / title
        public string Title { get; set; }

        // The planned end date
        public string PlannedEndDate { get; set; }

        // The fundraiser's target amount to raise
        public double GoalTargetAmount { get; set; }

        // The tags 
        public string[] Tags { get; set; }
    }
}
