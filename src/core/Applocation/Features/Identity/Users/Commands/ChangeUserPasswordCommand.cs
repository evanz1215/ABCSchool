using Applocation.Wrappers;
using MediatR;

namespace Applocation.Features.Identity.Users.Commands;

public class ChangeUserPasswordCommand : IRequest<IResponseWrapper>
{
    public ChangePasswordRequest ChangePassword { get; set; }
}

public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, IResponseWrapper>
{
    private readonly IUserService _userService;

    public ChangeUserPasswordCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = await _userService.ChangePasswordAsync(request.ChangePassword);

        return await ResponseWrapper<string>.SuccessAsync(userId, "Password changed successfully.");
    }
}