namespace Randevoo.Application.Features.Auth.Common;

public record AuthResult(
    long UserId,
    string MobileNumber,
    string Token,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);
