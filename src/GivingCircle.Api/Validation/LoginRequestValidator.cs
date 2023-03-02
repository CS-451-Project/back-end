using FluentValidation;
using GivingCircle.Api.Requests;

namespace GivingCircle.Api.Validation
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator() 
        {
            RuleFor(x => x.Password).MinimumLength(4);
            RuleFor(x => x.Email).EmailAddress();
        }
    }
}
