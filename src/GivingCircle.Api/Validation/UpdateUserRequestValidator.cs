using FluentValidation;
using GivingCircle.Api.Requests;

namespace GivingCircle.Api.Validation
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.FirstName).MinimumLength(1);
            RuleFor(x => x.LastName).MinimumLength(1);
            RuleFor(x => x.Password).MinimumLength(4);
        }
    }
}
