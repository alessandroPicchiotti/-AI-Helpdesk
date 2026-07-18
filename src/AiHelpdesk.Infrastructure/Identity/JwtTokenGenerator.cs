using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AiHelpdesk.Core.Entities;
using AiHelpdesk.Core.Interfaces.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AiHelpdesk.Infrastructure.Identity;

public class JwtTokenGenerator(IOptions<JwtOptions> options) : IJwtTokenGenerator
{
    public const string TenantIdClaimType = "TenantId";
    public const string RuoloClaimType = "Ruolo";

    public string GenerateToken(User user)
    {
        var jwtOptions = options.Value;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(RuoloClaimType, user.Role.ToString()),
        };

        if (user.TenantId.HasValue)
        {
            claims.Add(new Claim(TenantIdClaimType, user.TenantId.Value.ToString()));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtOptions.ExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
