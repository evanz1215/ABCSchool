namespace Applocation;

public class JwtSettings
{
    public string Secret { get; set; }
    public int TokenExpiryTimeMinutes { get; set; }
    public int RefreshTokenExpiryTimeIsDays { get; set; }
}
