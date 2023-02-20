using System.ComponentModel.DataAnnotations;

namespace GivingCircle.Api.Requests.FundraiserService
{
    public class AddBankAccountRequest
    {
        [Required]
        public string Account_Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Zipcode { get; set; }
        [Required]
        public string Bank_Name { get; set; }
        [Required]
        public string Account_Num { get; set; }
        [Required]
        public string Routing_Num { get; set; }
        [Required]
        public string Account_Type { get; set; }
    }
}
