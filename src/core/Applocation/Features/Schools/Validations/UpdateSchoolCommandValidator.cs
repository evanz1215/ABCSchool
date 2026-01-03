using Applocation.Features.Schools.Commands;
using FluentValidation;

namespace Applocation.Features.Schools.Validations;

public class UpdateSchoolCommandValidator : AbstractValidator<UpdateSchoolCommand>
{
    public UpdateSchoolCommandValidator(ISchoolService schoolService)
    {
        RuleFor(x => x.UpdateSchool)
            .SetValidator(new UpdateSchoolRequestValidator(schoolService));
    }
}