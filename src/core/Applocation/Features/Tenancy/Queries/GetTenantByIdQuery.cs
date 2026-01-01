using Applocation.Wrappers;
using MediatR;

namespace Applocation.Features.Tenancy.Queries;

public class GetTenantByIdQuery : IRequest<IResponseWrapper>
{
    public string TenantId { get; set; }
}

public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, IResponseWrapper>
{
    private readonly ITenantService _tenantService;

    public GetTenantByIdQueryHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public async Task<IResponseWrapper> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        var tenantInDb = await _tenantService.GetTenantByIdAsync(request.TenantId);

        if (tenantInDb is not null)
        {
            return await ResponseWrapper<TenantResponse>.SuccessAsync(data: tenantInDb);
        }

        return await ResponseWrapper<TenantResponse>.FailAsync(message: "Tenant does not exist");
    }
}