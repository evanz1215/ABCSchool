using Applocation.Wrappers;
using MediatR;

namespace Applocation.Features.Tenancy.Commands;

public class CreateTenantCommand : IRequest<IResponseWrapper>
{
    public CreateTenantRequest CreateTenant { get; set; }
}

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, IResponseWrapper>
{
    private readonly ITenantService _tenantService;

    public CreateTenantCommandHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public async Task<IResponseWrapper> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenantId = await _tenantService.CreateTenantAsync(request.CreateTenant, cancellationToken);

        var result = await ResponseWrapper<string>.SuccessAsync(tenantId, "Tenant created successfully.");

        return result;
    }
}