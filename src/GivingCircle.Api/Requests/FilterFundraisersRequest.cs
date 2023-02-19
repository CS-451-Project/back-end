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
        public double EndDateOffset { get; set; }

        // Represents which column we want to order by
        public string OrderBy { get; set; }

        // ASC or DESC. True if ascending
        public bool Ascending { get; set; }
    }
}
