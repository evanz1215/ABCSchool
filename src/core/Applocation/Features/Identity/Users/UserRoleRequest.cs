namespace Applocation.Features.Identity.Users;

public class UserRoleRequest
{
    public string RoleId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsAssigned { get; set; }
}