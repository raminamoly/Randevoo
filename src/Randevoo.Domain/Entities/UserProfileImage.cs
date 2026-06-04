using Randevoo.Domain.Common;

namespace Randevoo.Domain.Entities;

public class UserProfileImage : BaseEntity
{
    public long UserProfileId { get; private set; }
    public UserProfile UserProfile { get; private set; } = null!;
    public string ImageUrl { get; private set; } = null!;
    public int DisplayOrder { get; private set; }
    public bool IsPrimary { get; private set; }

    private UserProfileImage() { }

    internal UserProfileImage(UserProfile userProfile, string imageUrl, int displayOrder, bool isPrimary)
    {
        UserProfile = GuardAgainst.Object.Null(userProfile, nameof(userProfile));
        ImageUrl = GuardAgainst.String.InvalidLength(imageUrl, nameof(imageUrl), 2, 500);
        DisplayOrder = GuardAgainst.Number.OutOfRange(displayOrder, nameof(displayOrder), 1, 3);
        IsPrimary = isPrimary;
    }

    public void Update(string imageUrl, int displayOrder, bool isPrimary)
    {
        ImageUrl = GuardAgainst.String.InvalidLength(imageUrl, nameof(imageUrl), 2, 500);
        DisplayOrder = GuardAgainst.Number.OutOfRange(displayOrder, nameof(displayOrder), 1, 3);
        IsPrimary = isPrimary;
        UpdateTimestamp();
    }
}
