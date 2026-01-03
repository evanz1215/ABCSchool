using Applocation.Wrappers;
using MediatR;

namespace Applocation.Features.Schools.Commands;

public class DeleteSchoolCommand : IRequest<IResponseWrapper>
{
    public int SchoolId { get; set; }
}

public class DeleteSchoolCommandHandler : IRequestHandler<DeleteSchoolCommand, IResponseWrapper>
{
    private readonly ISchoolService _schoolService;

    public DeleteSchoolCommandHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    public async Task<IResponseWrapper> Handle(DeleteSchoolCommand request, CancellationToken cancellationToken)
    {
        var schoolInDb = await _schoolService.GetByIdAsync(request.SchoolId);

        if (schoolInDb is not null)
        {
            var deleteSchoolId = await _schoolService.DeleteAsync(schoolInDb);
            var result = await ResponseWrapper<int>.SuccessAsync(deleteSchoolId, "School deleted successfully.");
            return result;
        }

        return await ResponseWrapper.FailAsync("School not found.");
    }
}