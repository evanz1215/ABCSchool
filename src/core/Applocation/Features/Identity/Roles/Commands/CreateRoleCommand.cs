using Applocation.Wrappers;
using MediatR;

namespace Applocation.Features.Identity.Roles.Commands;

public class CreateRoleCommand : IRequest<IResponseWrapper>
{
    public CreateRoleRequest CreateRole { get; set; }
}

public class CreateRoleCommandHander : IRequestHandler<CreateRoleCommand, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public CreateRoleCommandHander(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<IResponseWrapper> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var roleName = await _roleService.CreateAsync(request.CreateRole);

        return await ResponseWrapper<string>.SuccessAsync(data: roleName, $"Role '{roleName}' created successfully.");
    }
}