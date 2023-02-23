using System;

namespace GivingCircle.Api.DataAccess.Responses
{
    public class GetFundraiserResponse
    {
        // The fundraiser's id
        public string FundraiserId { get; set; }

        // The organizer's id
        public string OrganizerId { get; set; }

        // The fundraiser's picture's id
        public string PictureId { get; set; }

        // The fundraiser description
        public string Description { get; set; }

        // The fundraisers displayed name / title
        public string Title { get; set; }

        // The created date
        public DateTime CreatedDate { get; set; }

        // The planned end date
        public DateTime PlannedEndDate { get; set; }

        // The date the goal was last reached
        public DateTime GoalReachedDate { get; set; }

        // The fundraiser's target amount to raise
        public double GoalTargetAmount { get; set; }

        // The fundraiser's current balance
        public double CurrentBalanceAmount { get; set; }

        // The tags 
        public string[] Tags { get; set; }
    }
}