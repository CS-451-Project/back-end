using FluentValidation;
using GivingCircle.Api.Requests.FundraiserService;

namespace GivingCircle.Api.Validation.FundraiserService
{
    public class AddBankAccountRequestValidator : AbstractValidator<AddBankAccountRequest>
    {
        public AddBankAccountRequestValidator()
        {
            RuleFor(x => x.Account_Name).MinimumLength(1);
            RuleFor(x => x.Address).MinimumLength(1);
            RuleFor(x => x.City).MinimumLength(1);
            RuleFor(x => x.State).MinimumLength(1);
            RuleFor(x => x.Zipcode).MinimumLength(5);
            RuleFor(x => x.Account_Num).Length(12);
            RuleFor(x => x.Routing_Num).MinimumLength(9);
        }
    }
}
