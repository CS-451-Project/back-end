using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace GivingCircle.Api.Validation
{
    public class FileValidator : AbstractValidator<IFormFile>
    {
        // 1,048,576 bytes are in a MB
        private const int MB = 1048576;

        public FileValidator()
        {
            // File must not be null and must be smaller than or equal to a megabyte
            RuleFor(x => x.Length).NotNull().LessThanOrEqualTo(MB)
                .WithMessage("File size is larger than 1 MB");

            // Only accepting jpg files
            RuleFor(x => x.ContentType).NotNull().Must(x => x.Equals("image/jpeg") || x.Equals("image/jpg"))
                .WithMessage("Acceptable file types are: image/jpeg, image/jpg");
        }
    }
}
