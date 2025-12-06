namespace Applocation.Features.Identity.Tokens;

public class RefreshTokenRequest
{
    public string CurrentJwt { get; set; }
    public string CurrentRefreshToken { get; set; }
}