using Applocation.Wrappers;
using MediatR;

namespace Applocation.Features.Tenancy.Queries;

public class GetTenantsQuery : IRequest<IResponseWrapper>
{
}

public class GetTenantsQueryHandler : IRequestHandler<GetTenantsQuery, IResponseWrapper>
{
    private readonly ITenantService _tenantService;

    public GetTenantsQueryHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public async Task<IResponseWrapper> Handle(GetTenantsQuery request, CancellationToken cancellationToken)
    {
        var tenantsInDb = await _tenantService.GetTenantsAsync();
        return await ResponseWrapper<List<TenantResponse>>.SuccessAsync(data: tenantsInDb);
    }
}