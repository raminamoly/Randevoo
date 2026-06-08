using Randevoo.Domain.Entities;

namespace Randevoo.Application.Features.EventPlannerProfiles.Common;

public record EventPlannerProfileDto(
    long Id,
    long UserId,
    string Title,
    string? PictureUrl,
    string Resume,
    string SettlementCurrencyCode,
    bool IsSettlementCurrencyLocked,
    DateTime? SettlementCurrencyLockedAtUtc,
    string? SettlementCurrencyLockReason,
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
            profile.SettlementCurrencyCode,
            profile.IsSettlementCurrencyLocked,
            profile.SettlementCurrencyLockedAtUtc,
            profile.SettlementCurrencyLockReason,
            profile.AverageRating,
            profile.TotalSurveyCount,
            profile.HostedEventCount,
            profile.CancelledEventCount,
            profile.CompletedEventCount);
}
