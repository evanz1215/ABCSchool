using FluentValidation;

namespace Applocation.Features.Schools.Validations;

internal class CreateSchoolRequestValidator : AbstractValidator<CreateSchoolRequest>
{
    public CreateSchoolRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("School name is required.")
            .MaximumLength(60).WithMessage("School name must not exceed 60 characters.");

        RuleFor(x => x.EstablishedDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Date established cannot be future date.");
    }
}