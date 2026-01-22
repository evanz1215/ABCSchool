using Applocation.Wrappers;
using MediatR;

namespace Applocation.Features.Identity.Roles.Queries;

public class GetRoleWithPermissionsQuery : IRequest<IResponseWrapper>
{
    public string RoleId { get; set; }
}

public class GetRoleWithPermissionsQueryHandler : IRequestHandler<GetRoleWithPermissionsQuery, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public GetRoleWithPermissionsQueryHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<IResponseWrapper> Handle(GetRoleWithPermissionsQuery request, CancellationToken cancellationToken)
    {
        var roleWithPermissions = await _roleService.GetRoleWithPermissionsAsync(request.RoleId, cancellationToken);

        return await ResponseWrapper<RoleResponse>.SuccessAsync(data: roleWithPermissions);
    }
}