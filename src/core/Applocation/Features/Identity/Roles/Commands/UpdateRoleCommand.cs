using Applocation.Wrappers;
using MediatR;

namespace Applocation.Features.Identity.Roles.Commands;

public class UpdateRoleCommand : IRequest<IResponseWrapper>
{
    public UpdateRoleRequest UpdateRole { get; set; }
}

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public UpdateRoleCommandHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<IResponseWrapper> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var updatedRole = await _roleService.UpdateAsync(request.UpdateRole);

        return await ResponseWrapper.SuccessAsync(message: $"Role '{updatedRole}' updated successfully.");
    }
}