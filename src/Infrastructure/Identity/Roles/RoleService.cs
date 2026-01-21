using Applocation.Exceptions;
using Applocation.Features.Identity.Roles;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Constants;
using Infrastructure.Contexts;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Roles;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantContextAccessor;

    public RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantContextAccessor)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _dbContext = dbContext;
        _tenantContextAccessor = tenantContextAccessor;
    }

    public async Task<string> CreateAsync(CreateRoleRequest request)
    {
        var newRole = new ApplicationRole()
        {
            Name = request.Name,
            Description = request.Description,
        };

        var result = await _roleManager.CreateAsync(newRole);

        if (!result.Succeeded)
        {
            var errors = GetIdentityResultErrorDescriptions(result);
            throw new IdentityException(errors);
        }

        return newRole.Name;
    }

    public async Task<string> DeleteAsync(string id)
    {
        var roleInDb = await _roleManager.FindByIdAsync(id) ?? throw new NotFoundException(["Role does not exist."]);

        if (RoleConstants.IsDefaultRole(roleInDb.Name))
        {
            throw new ConflictException([$"Not allowed to delete '{roleInDb.Name}' role."]);
        }

        if ((await _userManager.GetUsersInRoleAsync(roleInDb.Name)).Count > 0)
        {
            throw new ConflictException([$"Not allowed to delete '{roleInDb.Name}' role as is currently assigned to users."]);
        }

        var result = await _roleManager.DeleteAsync(roleInDb);

        if (!result.Succeeded)
        {
            var errors = GetIdentityResultErrorDescriptions(result);
            throw new IdentityException(errors);
        }

        return roleInDb.Name;
    }

    public async Task<bool> DoesItExistsAsync(string name)
    {
        throw new NotImplementedException();
    }

    public async Task<RoleResponse> GetRoleByIdAsync(string id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<List<RoleResponse>> GetRolesAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<RoleResponse> GetRoleWithPermissionsAsync(string id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<string> UpdateAsync(UpdateRoleRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<string> UpdatePermissionsAsync(UpdateRolePermissionsRequest request)
    {
        throw new NotImplementedException();
    }

    private List<string> GetIdentityResultErrorDescriptions(IdentityResult identityResult)
    {
        var errorDescriptions = new List<string>();

        foreach (var error in identityResult.Errors)
        {
            errorDescriptions.Add(error.Description);
        }

        return errorDescriptions;
    }
}