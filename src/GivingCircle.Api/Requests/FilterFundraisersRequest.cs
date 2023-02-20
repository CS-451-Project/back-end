namespace GivingCircle.Api.Requests
{
    public class FilterFundraisersRequest
    {
        // Filter based on title contains
        public string Title { get; set; }

        // Filter based on tags equals
        public string[] Tags { get; set; }

        // Filter based on created date
        // The created date is in the past, so we'll filter like:
        // Created within the last day, week, two weeks, three weeks, or however we like.
        // We'll be comparing based off of an offset from today essentially
        public double CreatedDateOffset { get; set; }

        // Filter based on the end date
        // Same principle as created date offset, except that we'll be checking that
        // We're an amount before this date
        public double EndDateOffset { get; set; }

        // Represents which column we want to order by / which way
        // For example we can order by a column like the date, or by which
        // fundraisers are closet to their target goal
        public string OrderBy { get; set; }

        // ASC or DESC. True if ascending
        public bool Ascending { get; set; }
    }
}
