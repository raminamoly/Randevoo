using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class RoleOperationPermission : BaseEntity, IAggregateRoot
{
    public UserRole Role { get; private set; }
    public string Entity { get; private set; } = null!;
    public string Action { get; private set; } = null!;
    public bool Allowed { get; private set; }

    private RoleOperationPermission() { }

    public RoleOperationPermission(UserRole role, string entity, string action, bool allowed)
    {
        Role = role;
        Entity = PermissionAction.Normalize(entity);
        Action = PermissionAction.Normalize(action);
        Allowed = allowed;
        AddDomainEvent(new EntityCreatedEvent<RoleOperationPermission>(this));
    }

    public void SetAllowed(bool allowed)
    {
        if (Allowed == allowed)
            return;

        Allowed = allowed;
        UpdateTimestamp();
    }
}
