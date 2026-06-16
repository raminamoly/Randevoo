using Randevoo.Domain.Entities;

namespace Randevoo.Application.EndUsers.Events;

public interface IEndUserEventEligibilityService
{
    EndUserEventEligibilityResult Evaluate(UserProfile? profile, DatingEvent datingEvent, DateTime nowUtc);
}
