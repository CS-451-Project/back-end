using FluentValidation;
using GivingCircle.Api.Requests;

namespace GivingCircle.Api.Validation
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator() 
        {
            RuleFor(x => x.FirstName).MinimumLength(1);
            RuleFor(x => x.LastName).MinimumLength(1);
            RuleFor(x => x.Password).MinimumLength(4);
            RuleFor(x => x.Email).EmailAddress();
        } 
    }
}
