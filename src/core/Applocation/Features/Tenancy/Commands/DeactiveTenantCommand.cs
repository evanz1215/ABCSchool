using Applocation.Wrappers;
using MediatR;

namespace Applocation.Features.Tenancy.Commands;

public class DeactiveTenantCommand : IRequest<IResponseWrapper>
{
    public string TenantId { get; set; }
}

public class DeactiveTenantCommandHandler : IRequestHandler<DeactiveTenantCommand, IResponseWrapper>
{
    private readonly ITenantService _tenantService;

    public DeactiveTenantCommandHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public async Task<IResponseWrapper> Handle(DeactiveTenantCommand request, CancellationToken cancellationToken)
    {
        var tenantId = await _tenantService.DeactiveAsync(request.TenantId);

        var result = await ResponseWrapper<string>.SuccessAsync(tenantId, "Tenant de-activation successfully.");

        return result;
    }
}