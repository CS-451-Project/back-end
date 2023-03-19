using FluentValidation;
using GivingCircle.Api.Requests;

namespace GivingCircle.Api.Validation
{
    public class MakeDonationRequestValidator : AbstractValidator<MakeDonationRequest>
    {
        public MakeDonationRequestValidator() 
        {
            RuleFor(x => x.Amount).GreaterThan(0.00);
            RuleFor(x => x.FundraiserId).Length(36);
        }
    }
}
