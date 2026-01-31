using Applocation.Wrappers;
using MediatR;

namespace Applocation.Features.Identity.Users.Commands;

public class UpdateUserCommand : IRequest<IResponseWrapper>
{
    public UpdateUserRequest UpdateUser { get; set; }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, IResponseWrapper>
{
    private readonly IUserService _userService;

    public UpdateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var userId = await _userService.UpdateAsync(request.UpdateUser);
        return await ResponseWrapper<string>.SuccessAsync(userId, "User updated successfully");
    }
}