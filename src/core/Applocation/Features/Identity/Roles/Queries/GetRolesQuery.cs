using Applocation.Wrappers;
using MediatR;

namespace Applocation.Features.Identity.Roles.Queries;

public class GetRolesQuery : IRequest<IResponseWrapper>
{
}

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public GetRolesQueryHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<IResponseWrapper> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleService.GetRolesAsync(cancellationToken);

        return await ResponseWrapper<List<RoleResponse>>.SuccessAsync(data: roles);
    }
}