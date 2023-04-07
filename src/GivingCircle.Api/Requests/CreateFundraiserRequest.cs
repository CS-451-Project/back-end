using System.ComponentModel.DataAnnotations;

namespace GivingCircle.Api.Requests
{
    /// <summary>
    /// A create fundraiser request
    /// </summary>
    public class CreateFundraiserRequest
    {
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
