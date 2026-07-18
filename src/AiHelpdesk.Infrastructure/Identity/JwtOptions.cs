namespace AiHelpdesk.Infrastructure.Identity;

public class JwtOptions
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string SigningKey { get; set; }
    public int ExpiryMinutes { get; set; } = 60;
}
