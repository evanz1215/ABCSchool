namespace Applocation.Features.Identity.Roles;

public class UpdateRolePermissionsRequest
{
    public string RoId { get; set; }
    public List<string> NewPermissions { get; set; } = [];
}