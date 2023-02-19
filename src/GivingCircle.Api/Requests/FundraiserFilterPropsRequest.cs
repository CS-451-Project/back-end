using System;

namespace GivingCircle.Api.Requests
{
    public class FundraiserFilterPropsRequest
    {
        // Filter based on title contains?
        public string Title;

        // Filter based on tags
        public string[] Tags;

        // Filter based on created date
        // DateTime CreatedDate;
    }
}
