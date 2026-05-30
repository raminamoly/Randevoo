using Randevoo.Domain.Entities;

namespace Randevoo.Application.Interfaces.Auth;

public interface IJwtTokenService
{
    JwtTokenResult CreateToken(User user);
}

public record JwtTokenResult(string Token, DateTime ExpiresAtUtc);
