using Applocation.Features.Schools.Commands;
using FluentValidation;

namespace Applocation.Features.Schools.Validations;

public class CreateSchoolCommandValidator : AbstractValidator<CreateSchoolCommand>
{
    public CreateSchoolCommandValidator()
    {
        RuleFor(x => x.CreateSchool)
            .SetValidator(new CreateSchoolRequestValidator());
    }
}