using Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Identity.Auth;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var permission = context.User.Claims
            .Where(x => x.Type == ClaimConstants.Permission && x.Value == requirement.Permission);

        if (permission.Any())
        {
            context.Succeed(requirement);
            await Task.CompletedTask;
        }
    }
}