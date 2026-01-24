namespace Applocation.Features.Identity.Users;

public interface IUserService
{
    Task<string> CreateAsync(CreateUserRequest request);

    Task<string> UpdateAsync(UpdateUserRequest request);

    Task<string> DeleteUserAsync(string userId);

    Task<string> ActivateOrDeactivateAsync(string userId, bool activation);

    Task<string> ChangePasswordAsync(ChangePasswordRequest request);

    Task<string> AssignRolesAsync(string userId, UserRolesRequest request);

    Task<List<UserResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<UserResponse> GetUserByIdAsync(string userId, CancellationToken cancellationToken);

    Task<List<UserRoleResponse>> GetUserRolesAsync(string userId, CancellationToken cancellationToken);

    Task<bool> IsEmailTakenAsync(string email);

    Task<List<string>> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken);

    Task<bool> IsPermissionAssigedAsync(string userId, string permission, CancellationToken cancellationToken = default);
}