using Applocation.Features.Tenancy;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Contexts;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Tenancy;

public class TenantService : ITenantService
{
    private readonly IMultiTenantStore<ABCSchoolTenantInfo> _tenantStore;
    private readonly ApplicationDbSeeder _dbSeeder;
    private readonly IServiceProvider _serviceProvider;

    public TenantService(IMultiTenantStore<ABCSchoolTenantInfo> tenantStore, ApplicationDbSeeder dbSeeder, IServiceProvider serviceProvider)
    {
        _tenantStore = tenantStore;
        _dbSeeder = dbSeeder;
        _serviceProvider = serviceProvider;
    }

    public async Task<string> ActivateAsync(string id)
    {
        var tenantInDb = await _tenantStore.TryGetAsync(id);
        tenantInDb.IsActive = true;
        await _tenantStore.TryUpdateAsync(tenantInDb);

        return tenantInDb.Identifier;
    }

    public async Task<string> CreateTenantAsync(CreateTenantRequest createTenant, CancellationToken cancellationToken)
    {
        var newTenant = new ABCSchoolTenantInfo
        {
            Id = createTenant.Identifier,
            Identifier = createTenant.Identifier,
            Name = createTenant.Name,
            IsActive = createTenant.IsActive,
            ConnectionString = createTenant.ConnectionString,
            Email = createTenant.Email,
            FirstName = createTenant.FirstName,
            LastName = createTenant.LastName,
            ValidUpTo = createTenant.ValidUpTo,
        };

        await _tenantStore.TryAddAsync(newTenant);

        // Seeding tenant data
        using var scope = _serviceProvider.CreateScope();
        _serviceProvider.GetRequiredService<IMultiTenantContextSetter>()
            .MultiTenantContext = new MultiTenantContext<ABCSchoolTenantInfo>()
            {
                TenantInfo = newTenant,
            };

        await scope.ServiceProvider.GetRequiredService<ApplicationDbSeeder>()
            .InitializeDatabaseAsync(cancellationToken);

        return newTenant.Identifier;
    }

    public async Task<string> DeactiveAsync(string id)
    {
        var tenantInDb = await _tenantStore.TryGetAsync(id);
        tenantInDb.IsActive = false;
        await _tenantStore.TryUpdateAsync(tenantInDb);

        return tenantInDb.Identifier;
    }

    public async Task<TenantResponse> GetTenantByIdAsync(string id)
    {
        var tenantInDb = await _tenantStore.TryGetAsync(id);

        // Manual mapping
        //var tenantResponse = new TenantResponse
        //{
        //    Identifier = tenantInDb.Identifier,
        //    Name = tenantInDb.Name,
        //    ConnectionString = tenantInDb.ConnectionString,
        //    Email = tenantInDb.Email,
        //    FirstName = tenantInDb.FirstName,
        //    LastName = tenantInDb.LastName,
        //    IsActive = tenantInDb.IsActive,
        //    ValidUpTo = tenantInDb.ValidUpTo,
        //};
        //return tenantResponse;

        // Mapster
        return tenantInDb.Adapt<TenantResponse>();
    }

    public async Task<List<TenantResponse>> GetTenantsAsync()
    {
        var tenantsInDb = await _tenantStore.GetAllAsync();
        return tenantsInDb.Adapt<List<TenantResponse>>();
    }

    public async Task<string> UpdateSubscriptionAsync(UpdateTenantSubscriptionRequest updateTenantSubscription)
    {
        var tenantInDb = await _tenantStore.TryGetAsync(updateTenantSubscription.TenantId);

        tenantInDb.ValidUpTo = updateTenantSubscription.NewExpiryDate;

        await _tenantStore.TryUpdateAsync(tenantInDb);

        return tenantInDb.Identifier;
    }
}