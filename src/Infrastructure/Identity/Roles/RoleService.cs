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
        /// <summary>
        /// 更新指定角色的權限集合（claims）。
        /// 行為：
        /// 1. 驗證角色存在；若不存在拋出 NotFoundException。
        /// 2. 不允許對 Admin 系統角色變更權限（拋出 ConflictException）。
        /// 3. 若當前租戶非 root，會移除所有以 "Permission.Tenants." 開頭的權限，防止非 root 租戶變更租戶範圍權限。
        /// 4. 移除角色目前有但不在新權限集合中的 claim（透過 RoleManager.RemoveClaimAsync）。
        /// 5. 對於新權限中但目前不存在的權限，建立新的 ApplicationRoleClaim 實體並透過 DbContext 新增，最後 SaveChangesAsync。
        /// 注意：
        /// - 移除使用 RoleManager（Identity store API），新增使用 DbContext（EF）直接寫入 RoleClaims，兩者不一致可能導致同步或交易性問題。
        /// - 此方法會就地修改傳入的 request.NewPermissions（因為呼叫了 RemoveAll）。
        /// </summary>
        /// <param name="request">包含目標 Role Id（RoId）及新的權限列表（NewPermissions）</param>
        /// <returns>成功訊息字串</returns>

        // 1) 取出目標角色，若不存在則拋例外
        var roleInDb = await _roleManager.FindByIdAsync(request.RoId) ?? throw new NotFoundException(["Role does not exist."]);

        // 2) 禁止對 Admin 角色變更權限
        if (roleInDb.Name == RoleConstants.Admin)
        {
            throw new ConflictException([$"Not allowed to change permissions for '{roleInDb.Name}' role."]);
        }

        // 3) 多租戶檢查：若非 root 租戶，移除所有 Tenant-specific 權限，避免非 root 租戶修改這些權限
        //    注意：此處會直接修改 request.NewPermissions（in-place）
        if (_tenantContextAccessor.MultiTenantContext.TenantInfo.Id != TenancyConstants.Root.Id)
        {
            request.NewPermissions.RemoveAll(x => x.StartsWith("Permission.Tenants."));
        }

        // -------- Drop (Delete) ----------
        // 4) 取得角色目前的 claims（包含所有 claim type）
        var currentClaims = await _roleManager.GetClaimsAsync(roleInDb);

        // 5) 對於目前存在但不在新集合中的 claim，逐一呼叫 RemoveClaimAsync 刪除
        foreach (var claim in currentClaims.Where(x => !request.NewPermissions.Any(y => y == x.Value)))
        {
            var result = await _roleManager.RemoveClaimAsync(roleInDb, claim);

            if (!result.Succeeded)
            {
                // 將 IdentityResult 的錯誤描述整理後拋出，供上層處理或回傳給前端
                var errors = GetIdentityResultErrorDescriptions(result);
                throw new IdentityException(errors);
            }
        }

        // -------- Lift (Create) ----------
        // 6) 對於新集合中但目前不存在的權限，建立 ApplicationRoleClaim 並使用 DbContext 新增（批次新增，之後 SaveChanges）
        //    注意：可以考慮改用 _roleManager.AddClaimAsync(roleInDb, new Claim(...)) 以保持與 Remove 的對稱性
        foreach (var newPermission in request.NewPermissions.Where(x => !currentClaims.Any(y => y.Value == x)))
        {
            await _dbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
            {
                RoleId = roleInDb.Id,
                ClaimType = ClaimConstants.Permission,
                ClaimValue = newPermission,
                Description = string.Empty, // TODO:應該由UpdateRolePermissionsRequest的NewPermissions提供,需修改NewPermissions類型
                Group = string.Empty,
            });
        }

        // 7) 儲存所有透過 DbContext 新增的 claim
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