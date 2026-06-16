using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.EndUsers.Profile;

public interface IEndUserProfileStatusService
{
    UserProfileStatus RefreshStatus(UserProfile profile);
}
