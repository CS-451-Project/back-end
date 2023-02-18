using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GivingCircle.Api.Fundraiser.Models.Models
{
    public class BankAccount
    {
        //Some of these variables might be changed/modified in the future
        public string Account_Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string Bank_Name { get; set; }
        public string Account_Num { get; set; }
        public string Routing_Num { get; set; }
        public string Account_Type { get; set; }
        public string Bank_Account_Id { get; set; }

    }
}
