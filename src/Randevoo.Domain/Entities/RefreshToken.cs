using Randevoo.Domain.Common;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class RefreshToken : BaseEntity, IAggregateRoot
{
    public long UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }
    public User User { get; private set; } = null!;

    public bool IsActive => RevokedAt == null && ExpiresAt > DateTime.UtcNow;

    private RefreshToken() { }

    public RefreshToken(long userId, string tokenHash, DateTime expiresAtUtc)
    {
        UserId = userId;
        TokenHash = GuardAgainst.String.NullOrWhiteSpace(tokenHash, nameof(tokenHash));
        ExpiresAt = expiresAtUtc;
    }

    public void Rotate(string replacementTokenHash, DateTime nowUtc)
    {
        EnsureActive(nowUtc);
        RevokedAt = nowUtc;
        ReplacedByTokenHash = GuardAgainst.String.NullOrWhiteSpace(replacementTokenHash, nameof(replacementTokenHash));
        UpdateTimestamp();
    }

    public void Revoke(DateTime nowUtc)
    {
        if (RevokedAt != null)
            return;

        RevokedAt = nowUtc;
        UpdateTimestamp();
    }

    public void EnsureActive(DateTime nowUtc)
    {
        if (RevokedAt != null)
            throw new BusinessRuleViolationException("Invalid refresh token", "The refresh token has already been used or revoked");

        if (ExpiresAt <= nowUtc)
            throw new BusinessRuleViolationException("Expired refresh token", "The refresh token has expired");
    }
}
