using Randevoo.Domain.Common;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class UserOperationPermissionOverride : BaseEntity, IAggregateRoot
{
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string Entity { get; private set; } = null!;
    public string Action { get; private set; } = null!;
    public bool Allowed { get; private set; }
    public string? Note { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }

    private UserOperationPermissionOverride() { }

    public UserOperationPermissionOverride(long userId, string entity, string action, bool allowed, string? note = null, DateTime? expiresAtUtc = null)
    {
        UserId = userId;
        Entity = PermissionAction.Normalize(entity);
        Action = PermissionAction.Normalize(action);
        Allowed = allowed;
        Note = string.IsNullOrWhiteSpace(note) ? null : GuardAgainst.String.InvalidLength(note.Trim(), nameof(note), 2, 500);
        ExpiresAtUtc = expiresAtUtc;
        AddDomainEvent(new EntityCreatedEvent<UserOperationPermissionOverride>(this));
    }

    public void Update(bool allowed, string? note, DateTime? expiresAtUtc)
    {
        Allowed = allowed;
        Note = string.IsNullOrWhiteSpace(note) ? null : GuardAgainst.String.InvalidLength(note.Trim(), nameof(note), 2, 500);
        ExpiresAtUtc = expiresAtUtc;
        UpdateTimestamp();
    }
}
