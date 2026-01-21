namespace Applocation.Features.Identity.Roles;

public interface IRoleService
{
    Task<string> CreateAsync(CreateRoleRequest request);

    Task<string> UpdateAsync(UpdateRoleRequest request);

    Task<string> DeleteAsync(string id);

    Task<bool> DoesItExistsAsync(string name);

    Task<string> UpdatePermissionsAsync(UpdateRolePermissionsRequest request);

    Task<List<RoleResponse>> GetRolesAsync(CancellationToken cancellationToken);

    Task<RoleResponse> GetRoleByIdAsync(string id, CancellationToken cancellationToken);

    Task<RoleResponse> GetRoleWithPermissionsAsync(string id, CancellationToken cancellationToken);
}