namespace Applocation.Features.Tenancy;

public interface ITenantService
{
    Task<string> CreateTenantAsync(CreateTenantRequest createTenant, CancellationToken cancellationToken);

    Task<string> ActivateAsync(string id);

    Task<string> DeactiveAsync(string id);

    Task<string> UpdateSubscriptionAsync(UpdateTenantSubscriptionRequest updateTenantSubscription);

    Task<List<TenantResponse>> GetTenantsAsync();

    Task<TenantResponse> GetTenantByIdAsync(string id);
}