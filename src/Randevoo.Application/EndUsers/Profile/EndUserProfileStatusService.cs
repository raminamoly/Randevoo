using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.EndUsers.Profile;

public sealed class EndUserProfileStatusService : IEndUserProfileStatusService
{
    public UserProfileStatus RefreshStatus(UserProfile profile)
    {
        profile.RefreshProfileStatus();
        return profile.ProfileStatus;
    }
}
