using Applocation.Wrappers;
using Mapster;
using MediatR;

namespace Applocation.Features.Schools.Queries;

public class GetSchoolByNameQuery : IRequest<IResponseWrapper>
{
    public string Name { get; set; }
}

public class GetSchoolByNameQueryHandler : IRequestHandler<GetSchoolByNameQuery, IResponseWrapper>
{
    private readonly ISchoolService _schoolService;

    public GetSchoolByNameQueryHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    public async Task<IResponseWrapper> Handle(GetSchoolByNameQuery request, CancellationToken cancellationToken)
    {
        var schoolInDb = await _schoolService.GetByNameAsync(request.Name);

        if (schoolInDb is not null)
        {
            var result = schoolInDb.Adapt<SchoolResponse>();
            return await ResponseWrapper<SchoolResponse>.SuccessAsync(result);
        }

        return await ResponseWrapper<SchoolResponse>.FailAsync($"School with name {request.Name} not found.");
    }
}