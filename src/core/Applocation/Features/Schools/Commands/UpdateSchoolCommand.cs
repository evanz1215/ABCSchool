using Applocation.Wrappers;
using MediatR;

namespace Applocation.Features.Schools.Commands;

public class UpdateSchoolCommand : IRequest<IResponseWrapper>
{
    public UpdateSchoolRequest UpdateSchool { get; set; }
}

public class UpdateSchoolCommandHandler : IRequestHandler<UpdateSchoolCommand, IResponseWrapper>
{
    private readonly ISchoolService _schoolService;

    public UpdateSchoolCommandHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    public async Task<IResponseWrapper> Handle(UpdateSchoolCommand request, CancellationToken cancellationToken)
    {
        var schoolInDb = await _schoolService.GetByIdAsync(request.UpdateSchool.Id);

        if (schoolInDb is not null)
        {
            schoolInDb.Name = request.UpdateSchool.Name;
            schoolInDb.EstablishedDate = request.UpdateSchool.EstablishedDate;

            var schoolId = await _schoolService.UpdateAsync(schoolInDb);

            var result = await ResponseWrapper<int>.SuccessAsync(schoolId, "School updated successfully.");
            return result;
        }

        return await ResponseWrapper.FailAsync("School not found.");
    }
}