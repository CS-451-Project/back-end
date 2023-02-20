using FluentValidation;
using GivingCircle.Api.Requests.FundraiserService;

namespace GivingCircle.Api.Validation.FundraiserService
{
    public class AddBankAccountRequestValidator : AbstractValidator<AddBankAccountRequest>
    {
        public AddBankAccountRequestValidator()
        {
            RuleFor(x => x.Account_Name).Length(36);
            RuleFor(x => x.Account_Num).Length(36);
            RuleFor(x => x.Routing_Num).MinimumLength(1);
            //RuleFor(x => x.Account_Name).GreaterThan(0.0);
        }
    }
}
