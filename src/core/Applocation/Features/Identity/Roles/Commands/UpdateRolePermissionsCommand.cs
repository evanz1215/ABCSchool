using Applocation.Wrappers;
using MediatR;

namespace Applocation.Features.Identity.Roles.Commands;

public class UpdateRolePermissionsCommand : IRequest<IResponseWrapper>
{
    public UpdateRolePermissionsRequest UpdateRolePermissions { get; set; }
}

public class UpdateRolePermissionsCommandHandler : IRequestHandler<UpdateRolePermissionsCommand, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public UpdateRolePermissionsCommandHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<IResponseWrapper> Handle(UpdateRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        var message = await _roleService.UpdatePermissionsAsync(request.UpdateRolePermissions);

        return await ResponseWrapper.SuccessAsync(message: message);
    }
}