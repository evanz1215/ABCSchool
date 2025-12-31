using Applocation.Wrappers;
using MediatR;

namespace Applocation.Features.Tenancy.Commands;

public class UpdateTenantSubscriptionCommand : IRequest<IResponseWrapper>
{
    public UpdateTenantSubscriptionRequest UpdateTenantSubscription { get; set; }
}

public class UpdateTenantSubscriptionCommandHandler : IRequestHandler<UpdateTenantSubscriptionCommand, IResponseWrapper>
{
    private readonly ITenantService _tenantService;

    public UpdateTenantSubscriptionCommandHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public async Task<IResponseWrapper> Handle(UpdateTenantSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var tenantId = await _tenantService.UpdateSubscriptionAsync(request.UpdateTenantSubscription);
        var result = await ResponseWrapper<string>.SuccessAsync(tenantId, "Tenant subscription updated successfully.");
        return result;
    }
}