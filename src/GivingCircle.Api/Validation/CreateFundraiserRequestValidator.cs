﻿using FluentValidation;
using GivingCircle.Api.Requests;

namespace GivingCircle.Api.Validation
{
    public class CreateFundraiserRequestValidator : AbstractValidator<CreateFundraiserRequest>
    {
        public CreateFundraiserRequestValidator()
        {
            RuleFor(x => x.OrganizerId).Length(36);
            RuleFor(x => x.BankInformationId).Length(36);
            RuleFor(x => x.Title).MinimumLength(1);
            RuleFor(x => x.GoalTargetAmount).GreaterThan(0.0);
        }
    }
}