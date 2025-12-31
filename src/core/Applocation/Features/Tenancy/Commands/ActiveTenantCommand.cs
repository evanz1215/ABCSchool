using Applocation.Wrappers;
using MediatR;

namespace Applocation.Features.Tenancy.Commands;

public class ActiveTenantCommand : IRequest<IResponseWrapper>
{
    public string TenantId { get; set; }
}

public class ActiveTenantCommandHandler : IRequestHandler<ActiveTenantCommand, IResponseWrapper>
{
    private readonly ITenantService _tenantService;

    public ActiveTenantCommandHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public async Task<IResponseWrapper> Handle(ActiveTenantCommand request, CancellationToken cancellationToken)
    {
        var tenantId = await _tenantService.ActivateAsync(request.TenantId);

        var result = await ResponseWrapper<string>.SuccessAsync(tenantId, "Tenant activation successfully.");

        return result;
    }
}