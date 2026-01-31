using Applocation.Wrappers;
using MediatR;

namespace Applocation.Features.Identity.Users.Commands;

public class UpdateUserRolesCommand : IRequest<IResponseWrapper>
{
    public string RoleId { get; set; }
    public UserRolesRequest UserRolesRequest { get; set; }
}

public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, IResponseWrapper>
{
    private readonly IUserService _userService;

    public UpdateUserRolesCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        var userId = await _userService.AssignRolesAsync(request.RoleId, request.UserRolesRequest);
        return await ResponseWrapper<string>.SuccessAsync(userId, "User roles updated successfully.");
    }
}