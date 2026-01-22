using Applocation.Wrappers;
using MediatR;

namespace Applocation.Features.Identity.Roles.Queries;

public class GetRoleByIdQuery : IRequest<IResponseWrapper>
{
    public string RoleId { get; set; }
}

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public GetRoleByIdQueryHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<IResponseWrapper> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _roleService.GetRoleByIdAsync(request.RoleId, cancellationToken);

        return await ResponseWrapper<RoleResponse>.SuccessAsync(data: role);
    }
}