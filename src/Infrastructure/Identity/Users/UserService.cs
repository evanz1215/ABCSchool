using Applocation.Exceptions;
using Applocation.Features.Identity.Users;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Constants;
using Infrastructure.Contexts;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity.Users;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantContextAccessor;

    public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext dbContext, IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantContextAccessor)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
        _tenantContextAccessor = tenantContextAccessor;
    }

    public async Task<string> ActivateOrDeactivateAsync(string userId, bool activation)
    {
        var userInDb = await GetUserAsync(userId);

        //if (userInDb == null)
        //{
        //    throw new NotFoundException(["User does not exist."]);
        //}

        userInDb.IsActive = activation;
        var result = await _userManager.UpdateAsync(userInDb);

        if (!result.Succeeded)
        {
            var errors = IdentityHelper.GetIdentityResultErrorDescriptions(result);
            throw new IdentityException(errors);
        }

        return userInDb.Id;
    }

    public async Task<string> AssignRolesAsync(string userId, UserRolesRequest request)
    {
        // 1) 取得使用者（若找不到會在 GetUserAsync 裡拋出 NotFoundException）
        var userInDb = await GetUserAsync(userId);

        // 2) 檢查是否正在移除 Admin 角色（保護性檢查）
        // 條件：使用者目前有 Admin 角色，且 request 中有把 Admin 的 IsAssigned 設為 false（意味移除）
        if (await _userManager.IsInRoleAsync(userInDb, RoleConstants.Admin) && request.UserRoles.Any(x => !x.IsAssigned && x.Name == RoleConstants.Admin))
        {
            // 取得目前屬於 Admin 角色的使用者數量（整個系統或租戶範圍視設定而定）
            var adminUsersCount = (await _userManager.GetUsersInRoleAsync(RoleConstants.Admin)).Count;

            // 特殊保護：若此使用者為 Root Tenant 的 root user（以 email 判斷）且目前執行緒為 Root Tenant
            // 不允許移除該 Root Tenant User 的 Admin 角色
            if (userInDb.Email == TenancyConstants.Root.Email)
            {
                if (_tenantContextAccessor.MultiTenantContext?.TenantInfo.Id == TenancyConstants.Root.Id)
                {
                    // 明確拒絕此種操作（回傳衝突例外）
                    throw new ConflictException(["Not allowed to remove Admin role for a Root Tenant User."]);
                }
            }
            // 若不是 root user，則再檢查 Admin 使用者數量是否過少（至少保留兩個 Admin）
            else if (adminUsersCount <= 2)
            {
                // 至少保留兩個 Admin 用戶：若移除會使 Admin 數量 < 2，則拒絕
                throw new ConflictException(["Tenan should have at least two Admin Users."]);
            }
        }

        // 3) 根據 request 中每個 role 的 IsAssigned 屬性做加入或移除
        foreach (var userRole in request.UserRoles)
        {
            if (userRole.IsAssigned)
            {
                // 若要指定此角色，先檢查使用者是否已經有此角色（避免重複呼叫 AddToRole）
                if (!await _userManager.IsInRoleAsync(userInDb, userRole.Name))
                {
                    // 加入角色
                    var result = await _userManager.AddToRoleAsync(userInDb, userRole.Name);

                    // 原本作者將錯誤處理註解掉了（可能是暫時忽略或預期 AddToRoleAsync 幾乎不會失敗）
                    // 若要恢復嚴格處理，解除下面註解並拋出 IdentityException：
                    // if (!result.Succeeded)
                    // {
                    //     var errors = IdentityHelper.GetIdentityResultErrorDescriptions(result);
                    //     throw new IdentityException(errors);
                    // }
                }
            }
            else
            {
                // 若 IsAssigned 為 false，則從使用者移除該角色（即取消分配）
                // RemoveFromRoleAsync 若使用者本來就沒有該角色，通常不會拋例外，但會回傳失敗結果。
                await _userManager.RemoveFromRoleAsync(userInDb, userRole.Name);
            }
        }

        // 4) 回傳使用者 Id 作為識別
        return userInDb.Id;
    }

    public async Task<string> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var userInDb = await GetUserAsync(request.UserId);

        if (request.NewPassword != request.ConfirmNewPassword)
        {
            throw new ConflictException(["Passwords do not match."]);
        }

        var result = await _userManager.ChangePasswordAsync(userInDb, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = IdentityHelper.GetIdentityResultErrorDescriptions(result);
            throw new IdentityException(errors);
        }

        return userInDb.Id;
    }

    public async Task<string> CreateAsync(CreateUserRequest request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            throw new ConflictException(["Passwords do not match."]);
        }

        if (await IsEmailTakenAsync(request.Email))
        {
            throw new ConflictException(["Email is already taken."]);
        }

        var newUser = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            IsActive = request.IsActive,
            UserName = request.Email,
            EmailConfirmed = true,
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            var errors = IdentityHelper.GetIdentityResultErrorDescriptions(result);
            throw new IdentityException(errors);
        }

        return newUser.Id;
    }

    public async Task<string> DeleteUserAsync(string userId)
    {
        var userInDb = await GetUserAsync(userId);

        if (userInDb.Email == TenancyConstants.Root.Email)
        {
            throw new ConflictException(["Not allowed to remove Admin User for a Root Tenant User."]);
        }

        _dbContext.Users.Remove(userInDb);

        await _dbContext.SaveChangesAsync();

        return userInDb.Id;
    }

    public async Task<List<UserResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var usersInDb = await _userManager.Users.ToListAsync(cancellationToken);

        return usersInDb.Adapt<List<UserResponse>>();
    }

    public async Task<UserResponse> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var userInDb = await GetUserAsync(userId);

        return userInDb.Adapt<UserResponse>();
    }

    public async Task<List<string>> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken)
    {
        var userInDb = await GetUserAsync(userId);

        var userRolesNames = await _userManager.GetRolesAsync(userInDb);

        if (userRolesNames == null || !userRolesNames.Any())
        {
            return new List<string>();
        }

        var permissions = new List<string>();

        foreach (var role in await _roleManager.Roles.Where(x => userRolesNames.Contains(x.Name)).ToListAsync(cancellationToken))
        {
            permissions.AddRange(await _dbContext.RoleClaims.Where(x => x.RoleId == role.Id && x.ClaimType == ClaimConstants.Permission).Select(x => x.ClaimValue).ToListAsync(cancellationToken));
        }

        return permissions.Distinct().ToList();

        // INFO: 效能優化參考
        // 保留存在檢查（會拋 NotFoundException）
        //var userInDb = await GetUserAsync(userId);

        //var permissions = await _dbContext.UserRoles
        //    .Where(ur => ur.UserId == userId)
        //    .Join(_dbContext.RoleClaims,
        //        userRole => userRole.RoleId,
        //        roleClaim => roleClaim.RoleId,
        //        (userRole, roleClaim) => roleClaim)
        //    .Where(rc => rc.ClaimType == ClaimConstants.Permission)
        //    .Select(rc => rc.ClaimValue)
        //    .Distinct()
        //    .ToListAsync(cancellationToken);

        //return permissions;
    }

    public async Task<List<UserRoleResponse>> GetUserRolesAsync(string userId, CancellationToken cancellationToken)
    {
        var userInDb = await GetUserAsync(userId);

        var userRoles = new List<UserRoleResponse>();

        var rolesInDb = await _roleManager.Roles.ToListAsync(cancellationToken);

        foreach (var role in rolesInDb)
        {
            userRoles.Add(new UserRoleResponse
            {
                RoleId = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsAssigned = await _userManager.IsInRoleAsync(userInDb, role.Name)
            });
        }

        return userRoles;
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        var result = await _userManager.FindByEmailAsync(email);

        return result is not null;
    }

    public async Task<bool> IsPermissionAssigedAsync(string userId, string permission, CancellationToken cancellationToken = default)
    {
        var userPermissions = await GetUserPermissionsAsync(userId, cancellationToken);
        var result = userPermissions.Contains(permission);

        return result;
    }

    public async Task<string> UpdateAsync(UpdateUserRequest request)
    {
        var userInDb = await GetUserAsync(request.Id);

        userInDb.FirstName = request.FirstName;
        userInDb.LastName = request.LastName;
        userInDb.PhoneNumber = request.PhoneNumber;

        var result = await _userManager.UpdateAsync(userInDb);

        if (!result.Succeeded)
        {
            var errors = IdentityHelper.GetIdentityResultErrorDescriptions(result);
            throw new IdentityException(errors);
        }

        return userInDb.Id;
    }

    private async Task<ApplicationUser> GetUserAsync(string userId)
    {
        var userInDb = await _userManager.FindByIdAsync(userId) ?? throw new NotFoundException(["User does not exist."]);

        return userInDb;
    }
}