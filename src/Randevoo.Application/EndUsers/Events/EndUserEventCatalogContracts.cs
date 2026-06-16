using Randevoo.Domain.Enums;

namespace Randevoo.Application.EndUsers.Events;

public sealed record EndUserEventCatalogRequest(
    long? UserId,
    int Page,
    int PageSize,
    long? CityId,
    bool IncludeOnline,
    bool IncludeInPerson,
    long? EventTypeId,
    DateTime? DateFromUtc,
    DateTime? DateToUtc,
    decimal? PriceMin,
    decimal? PriceMax,
    int? Age,
    long? EducationLevelId,
    bool OnlyEligibleForMe,
    EndUserEventSort Sort);

public sealed record EndUserEventCatalogPageDto(
    IReadOnlyList<EndUserEventCardDto> Items,
    int Page,
    int PageSize,
    bool HasNextPage);

public sealed record EndUserEventCardDto(
    long Id,
    int EventCode,
    string Title,
    string? ImageUrl,
    string EventType,
    string Country,
    string City,
    bool IsOnline,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    decimal StartingPrice,
    string CurrencyCode,
    EventPaymentCollectionMethod PaymentCollectionMethod,
    IReadOnlyList<string> Tags,
    UserFacingEventStatusKind UserFacingStatus,
    int MaleRemainingCapacity,
    int FemaleRemainingCapacity,
    bool IsEligibleForCurrentUser,
    string EligibilityReasonCode,
    string EligibilityMessage,
    UserProfileStatus? CurrentUserProfileStatus,
    int RecommendationScore,
    string? OrganizerTitle,
    decimal OrganizerAverageRating,
    int OrganizerSurveyCount);

public sealed record EndUserEventDetailsDto(
    long Id,
    int EventCode,
    string Title,
    string EventType,
    string Country,
    string City,
    string? Region,
    string? VenueName,
    string Address,
    bool IsOnline,
    string? OnlinePlatform,
    string? OnlineAccessInstructions,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    IReadOnlyList<string> ImageUrls,
    string DescriptionHtml,
    IReadOnlyList<string> Tags,
    IReadOnlyList<EndUserEventFaqDto> Faqs,
    int MaleMinAge,
    int MaleMaxAge,
    int FemaleMinAge,
    int FemaleMaxAge,
    long? MinimumEducationLevelId,
    EventEducationLevelRestriction EducationLevelRestriction,
    decimal MaleTicketPrice,
    decimal FemaleTicketPrice,
    string CurrencyCode,
    EventPaymentCollectionMethod PaymentCollectionMethod,
    string? OrganizerPaymentInstructions,
    UserFacingEventStatusKind UserFacingStatus,
    int MaleCapacity,
    int FemaleCapacity,
    int MaleRemainingCapacity,
    int FemaleRemainingCapacity,
    bool CanBuyTicket,
    string EligibilityReasonCode,
    string EligibilityMessage,
    UserProfileStatus? CurrentUserProfileStatus,
    OrganizerSummaryDto Organizer);

public sealed record EndUserEventFaqDto(
    string Question,
    string Answer);

public sealed record OrganizerSummaryDto(
    long UserId,
    string? Title,
    string? PictureUrl,
    string? Resume,
    decimal AverageRating,
    int SurveyCount,
    int HostedEventCount,
    int CancelledEventCount,
    int CompletedEventCount);

public interface IEndUserEventCatalogReader
{
    Task<EndUserEventCatalogPageDto> ListAsync(EndUserEventCatalogRequest request, CancellationToken cancellationToken);
    Task<EndUserEventDetailsDto?> GetDetailsAsync(long eventId, long? userId, CancellationToken cancellationToken);
}
