namespace Litrater.Infrastructure.Authentication;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public required string SecretKey { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public int ExpirationInMinutes { get; init; } = 60;
}