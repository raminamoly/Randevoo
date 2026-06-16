using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class UserRestriction : BaseEntity, IAggregateRoot
{
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;
    public UserRestrictionType RestrictionType { get; private set; }
    public string Reason { get; private set; } = null!;
    public long CreatedByUserId { get; private set; }
    public User CreatedByUser { get; private set; } = null!;
    public DateTime? ExpiresAtUtc { get; private set; }
    public bool IsActive { get; private set; }
    public long? RemovedByUserId { get; private set; }
    public User? RemovedByUser { get; private set; }
    public DateTime? RemovedAtUtc { get; private set; }
    public string? RemovalReason { get; private set; }

    private UserRestriction() { }

    public UserRestriction(
        User user,
        UserRestrictionType restrictionType,
        string reason,
        User createdByUser,
        DateTime? expiresAtUtc = null)
    {
        User = GuardAgainst.Object.Null(user, nameof(user));
        UserId = user.Id;
        RestrictionType = GuardAgainst.Number.AgainstInvalidEnum<UserRestrictionType>((int)restrictionType, nameof(restrictionType));
        Reason = NormalizeReason(reason, nameof(reason));
        CreatedByUser = GuardAgainst.Object.Null(createdByUser, nameof(createdByUser));
        CreatedByUserId = createdByUser.Id;
        ExpiresAtUtc = expiresAtUtc;
        IsActive = true;

        AddDomainEvent(new EntityCreatedEvent<UserRestriction>(this));
    }

    public bool IsEffective(DateTime nowUtc)
        => IsActive && (ExpiresAtUtc is null || ExpiresAtUtc > nowUtc);

    public void Remove(User removedByUser, string reason)
    {
        if (!IsActive)
            return;

        RemovedByUser = GuardAgainst.Object.Null(removedByUser, nameof(removedByUser));
        RemovedByUserId = removedByUser.Id;
        RemovedAtUtc = DateTime.UtcNow;
        RemovalReason = NormalizeReason(reason, nameof(reason));
        IsActive = false;
        UpdateTimestamp();

        AddDomainEvent(new EntityUpdatedEvent<UserRestriction>(this, nameof(IsActive), true, false));
    }

    private static string NormalizeReason(string value, string parameterName)
        => GuardAgainst.String.InvalidLength(GuardAgainst.String.NullOrWhiteSpace(value, parameterName).Trim(), parameterName, 5, 1000);
}
