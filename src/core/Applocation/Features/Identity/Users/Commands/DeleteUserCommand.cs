using Applocation.Wrappers;
using MediatR;

namespace Applocation.Features.Identity.Users.Commands;

public class DeleteUserCommand : IRequest<IResponseWrapper>
{
    public string UserId { get; set; }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, IResponseWrapper>
{
    private readonly IUserService _userService;

    public DeleteUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userId = await _userService.DeleteUserAsync(request.UserId);

        return await ResponseWrapper<string>.SuccessAsync(userId, "User deleted successfully.");
    }
}