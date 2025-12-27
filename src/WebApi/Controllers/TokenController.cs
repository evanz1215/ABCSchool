using Applocation.Features.Identity.Tokens;
using Applocation.Features.Identity.Tokens.Queries;
using Infrastructure.Constants;
using Infrastructure.Identity.Auth;
using Infrastructure.OpenApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class TokenController : BaseApiController
{
    [HttpPost("login")]
    [AllowAnonymous]
    [TenantHeader]
    [OpenApiOperation("Used to obtain jwt for login.")]
    public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequest tokenRequest)
    {
        var respons = await Sender.Send(new GetTokenQuery { TokenRequest = tokenRequest });

        if (respons.IsSuccessful)
        {
            return Ok(respons);
        }
        return Unauthorized(respons);
    }

    [HttpPost("refresh-token")]
    [OpenApiOperation("Used to obtain jwt for refresh token.")]
    [ShouldHavePermission(action: SchoolAction.RefreshToken, feature: SchoolFeature.Tokens)]
    public async Task<IActionResult> GetRefreshTokenAasync([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        var response = await Sender.Send(new GetRefreshTokenQuery { RefreshToken = refreshTokenRequest });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return Unauthorized(response);
    }
}