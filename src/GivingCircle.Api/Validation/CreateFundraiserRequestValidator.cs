using FluentValidation;
using GivingCircle.Api.Requests;
using System;

namespace GivingCircle.Api.Validation
{
    public class CreateFundraiserRequestValidator : AbstractValidator<CreateFundraiserRequest>
    {
        public CreateFundraiserRequestValidator()
        {
            RuleFor(x => x.Title).MinimumLength(1);
            RuleFor(x => x.GoalTargetAmount).GreaterThan(0.0);
            RuleFor(x => DateTime.Parse(x.PlannedEndDate)).GreaterThan(DateTime.Now);
            RuleFor(x => x.Tags.Length).LessThanOrEqualTo(5);
        }
    }
}
