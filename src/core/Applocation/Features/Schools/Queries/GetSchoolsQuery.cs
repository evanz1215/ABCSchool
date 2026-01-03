using Applocation.Wrappers;
using Mapster;
using MediatR;

namespace Applocation.Features.Schools.Queries;

public class GetSchoolsQuery : IRequest<IResponseWrapper>
{
}

public class GetSchoolsQueryHandler : IRequestHandler<GetSchoolsQuery, IResponseWrapper>
{
    private readonly ISchoolService _schoolService;

    public GetSchoolsQueryHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    public async Task<IResponseWrapper> Handle(GetSchoolsQuery request, CancellationToken cancellationToken)
    {
        var schoolsInDb = await _schoolService.GetAllAsync();

        var schoolMappingResult = schoolsInDb?.Adapt<List<SchoolResponse>>() ?? new List<SchoolResponse>();
        return await ResponseWrapper<List<SchoolResponse>>.SuccessAsync(schoolMappingResult);
    }
}