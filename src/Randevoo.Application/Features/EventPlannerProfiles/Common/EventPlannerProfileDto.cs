using Randevoo.Domain.Entities;

namespace Randevoo.Application.Features.EventPlannerProfiles.Common;

public record EventPlannerProfileDto(
    long Id,
    long UserId,
    string Title,
    string? PictureUrl,
    string Resume,
    decimal AverageRating,
    int TotalSurveyCount,
    int HostedEventCount,
    int CancelledEventCount,
    int CompletedEventCount)
{
    public static EventPlannerProfileDto FromEntity(EventPlannerProfile profile) =>
        new(
            profile.Id,
            profile.UserId,
            profile.Title,
            profile.PictureUrl,
            profile.Resume,
            profile.AverageRating,
            profile.TotalSurveyCount,
            profile.HostedEventCount,
            profile.CancelledEventCount,
            profile.CompletedEventCount);
}
