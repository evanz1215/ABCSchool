using Applocation.Wrappers;
using MediatR;

namespace Applocation.Features.Identity.Tokens.Queries;

public class GetRefreshTokenQuery : IRequest<IResponseWrapper>
{
    public RefreshTokenRequest RefreshToken { get; set; }
}

public class GetRefreshTokenQueryHandler : IRequestHandler<GetRefreshTokenQuery, IResponseWrapper>
{
    private readonly ITokenService _tokenService;

    public GetRefreshTokenQueryHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<IResponseWrapper> Handle(GetRefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var token = await _tokenService.RefreshTokenAsync(request.RefreshToken);
        var result = await ResponseWrapper<TokenResponse>.SuccessAsync(data: token);
        return result;
    }
}