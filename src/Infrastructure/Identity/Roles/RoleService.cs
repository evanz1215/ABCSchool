using Applocation.Exceptions;
using Applocation.Features.Identity.Roles;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Constants;
using Infrastructure.Contexts;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        var isExists = await _roleManager.RoleExistsAsync(name);
        return isExists;
    }

    public async Task<List<RoleResponse>> GetRolesAsync(CancellationToken cancellationToken)
    {
        var roleInDb = await _roleManager.Roles.ToListAsync(cancellationToken);

        var resultMap = roleInDb.Adapt<List<RoleResponse>>();

        return resultMap;
    }

    public async Task<RoleResponse> GetRoleByIdAsync(string id, CancellationToken cancellationToken)
    {
        var roleInDb = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException(["Role does not exist."]);
        var resultMap = roleInDb.Adapt<RoleResponse>();
        return resultMap;
    }

    public async Task<RoleResponse> GetRoleWithPermissionsAsync(string id, CancellationToken cancellationToken)
    {
        var role = await GetRoleByIdAsync(id, cancellationToken);

        role.Permissions = await _dbContext.RoleClaims
            .Where(x => x.RoleId == id && x.ClaimType == ClaimConstants.Permission)
            .Select(x => x.ClaimValue).ToListAsync(cancellationToken);

        return role;
    }

    public async Task<string> UpdateAsync(UpdateRoleRequest request)
    {
        var roleInDb = await _roleManager.FindByIdAsync(request.Id) ?? throw new NotFoundException(["Role does not exist."]);

        if (RoleConstants.IsDefaultRole(roleInDb.Name))
        {
            throw new ConflictException([$"Changes not allowed on system role '{roleInDb.Name}'."]);
        }

        roleInDb.Name = request.Name;
        roleInDb.Description = request.Description;
        roleInDb.NormalizedName = request.Name.ToUpperInvariant();

        var result = await _roleManager.UpdateAsync(roleInDb);

        if (!result.Succeeded)
        {
            var errors = GetIdentityResultErrorDescriptions(result);
            throw new IdentityException(errors);
        }

        return roleInDb.Name;
    }

    public async Task<string> UpdatePermissionsAsync(UpdateRolePermissionsRequest request)
    {
        var roleInDb = await _roleManager.FindByIdAsync(request.RoId) ?? throw new NotFoundException(["Role does not exist."]);

        if (roleInDb.Name == RoleConstants.Admin)
        {
            throw new ConflictException([$"Not allowed to change permissions for '{roleInDb.Name}' role."]);
        }

        if (_tenantContextAccessor.MultiTenantContext.TenantInfo.Id != TenancyConstants.Root.Id)
        {
            request.NewPermissions.RemoveAll(x => x.StartsWith("Permission.Tenants."));
        }

        // Drop(Delete)

        var currentClaims = await _roleManager.GetClaimsAsync(roleInDb);

        foreach (var claim in currentClaims.Where(x => !request.NewPermissions.Any(y => y == x.Value)))
        {
            var result = await _roleManager.RemoveClaimAsync(roleInDb, claim);

            if (!result.Succeeded)
            {
                var errors = GetIdentityResultErrorDescriptions(result);
                throw new IdentityException(errors);
            }
        }

        // lift(Create)

        foreach (var newPermission in request.NewPermissions.Where(x => !currentClaims.Any(y => y.Value == x)))
        {
            await _dbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
            {
                RoleId = roleInDb.Id,
                ClaimType = ClaimConstants.Permission,
                ClaimValue = newPermission,
                Description = string.Empty,
                Group = string.Empty,
            });
        }

        await _dbContext.SaveChangesAsync();

        return "Permissions Updated Successfully.";
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