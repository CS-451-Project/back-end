namespace GivingCircle.Api.Models
{
    public class FundraiserPicture
    {
        // The picture's id
        public string PictureId { get; set; }

        // The fundraiser's id for the picture
        public string FundraiserId { get; set; }

        // The user who uploaded the image
        public string UserId { get; set; }

        // The picture's URL
        public string PictureUrl { get; set; }
    }
}
