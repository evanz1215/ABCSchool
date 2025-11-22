using Finbuckle.MultiTenant;
using Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class Startup
{
    public static IServiceCollection AddMultitenancyServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TenantDbContext>(options =>
        {
            options
            .UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        })
            .AddMultiTenant<ABCSchoolTenantInfo>()
            .WithHeaderStrategy("tenant")
            .WithClaimStrategy("tenant")
            .WithEFCoreStore<TenantDbContext, ABCSchoolTenantInfo>();

        return services;
    }
}