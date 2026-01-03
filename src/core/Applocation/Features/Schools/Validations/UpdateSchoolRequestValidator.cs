using Domain.Entities;
using FluentValidation;

namespace Applocation.Features.Schools.Validations;

internal class UpdateSchoolRequestValidator : AbstractValidator<UpdateSchoolRequest>
{
    public UpdateSchoolRequestValidator(ISchoolService schoolService)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(async (id, ct) => await schoolService.GetByIdAsync(id) is School schoolinDb && schoolinDb.Id == id)
            .WithMessage("School does not exist.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("School name is required.")
            .MaximumLength(60).WithMessage("School name must not exceed 60 characters.");

        RuleFor(x => x.EstablishedDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Date established cannot be future date.");
    }
}