namespace Randevoo.Application.EndUsers.Events;

public sealed record UserFacingEventTimingOptions(
    TimeSpan ParticipantProfilesOpenBeforeStart,
    TimeSpan LikeWindowAfterEnd)
{
    public static UserFacingEventTimingOptions Default { get; } = new(
        ParticipantProfilesOpenBeforeStart: TimeSpan.FromHours(24),
        LikeWindowAfterEnd: TimeSpan.FromHours(24));
}
