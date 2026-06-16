using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.EndUsers.Events;

public interface IUserFacingEventStatusResolver
{
    UserFacingEventStatusKind Resolve(DatingEvent datingEvent, DateTime nowUtc);
}
