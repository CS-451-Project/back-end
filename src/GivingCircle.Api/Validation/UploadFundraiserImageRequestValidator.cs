using FluentValidation;
using GivingCircle.Api.Requests;

namespace GivingCircle.Api.Validation
{
    public class UploadFundraiserImageRequestValidator : AbstractValidator<UploadFundraiserImageRequest>
    {
        public UploadFundraiserImageRequestValidator() 
        {
            RuleFor(x => x.FundraiserImage).SetValidator(new FileValidator());
        }
    }
}
