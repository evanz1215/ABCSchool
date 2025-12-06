using Finbuckle.MultiTenant;
using Infrastructure.Contexts;
using Infrastructure.Identity.Auth;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TenantDbContext>(options =>
        {
            options
            .UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        })
            .AddMultiTenant<ABCSchoolTenantInfo>()
            .WithHeaderStrategy(TenancyConstants.TenantIdName)
            .WithClaimStrategy(TenancyConstants.TenantIdName)
            .WithEFCoreStore<TenantDbContext, ABCSchoolTenantInfo>();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            ;
        })
            .AddTransient<ITenantDbSeeder, TenantDbSeeder>()
            .AddTransient<ApplicationDbSeeder>()
            .AddIdentityServices()
            .AddPermissions();

        return services;
    }

    public static async Task AddDatabaseInitializerAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();

        await scope.ServiceProvider
            .GetRequiredService<ITenantDbSeeder>()
            .InitializeDatabaseAsync(cancellationToken);
    }

    internal static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services
            .AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    internal static IServiceCollection AddPermissions(this IServiceCollection services)
    {
        services
            .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseMultiTenant();

        return app;
    }
}