using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Randevoo.Application.Interfaces.Auth;
using Randevoo.Domain.Entities;

namespace Randevoo.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public JwtTokenService(IConfiguration configuration, IHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    public JwtTokenResult CreateToken(User user)
    {
        var secret = _configuration["Jwt:Secret"];
        if (string.IsNullOrWhiteSpace(secret))
        {
            if (!_environment.IsDevelopment() && !_environment.IsEnvironment("Testing"))
                throw new InvalidOperationException("Jwt:Secret is required.");

            secret = "development-secret-key-change-me-with-at-least-32-chars";
        }
        var issuer = _configuration["Jwt:Issuer"] ?? "Randevoo";
        var audience = _configuration["Jwt:Audience"] ?? "Randevoo";
        var expiresMinutes = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var minutes) ? minutes : 15;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("mobile_number", user.MobileNumber),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiresMinutes);
        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new JwtTokenResult(new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}
