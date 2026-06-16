using Microsoft.EntityFrameworkCore;
using Randevoo.Application.EndUsers.Events;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Services;

public sealed class EndUserEventCatalogReader : IEndUserEventCatalogReader
{
    private readonly RandevooDbContext _db;
    private readonly IEndUserEventEligibilityService _eligibility;
    private readonly IUserFacingEventStatusResolver _statusResolver;

    public EndUserEventCatalogReader(
        RandevooDbContext db,
        IEndUserEventEligibilityService eligibility,
        IUserFacingEventStatusResolver statusResolver)
    {
        _db = db;
        _eligibility = eligibility;
        _statusResolver = statusResolver;
    }

    public async Task<EndUserEventCatalogPageDto> ListAsync(EndUserEventCatalogRequest request, CancellationToken cancellationToken)
    {
        var nowUtc = DateTime.UtcNow;
        var profile = await LoadProfileAsync(request.UserId, cancellationToken);
        var selectedCityId = request.CityId;
        var recommendationCityId = request.CityId ?? profile?.CityId;
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 30);

        var events = await BuildBaseQuery(request, selectedCityId, nowUtc)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        var organizerProfiles = await LoadOrganizerProfilesAsync(events, cancellationToken);
        var tagWeights = await LoadTagWeightsAsync(profile, cancellationToken);

        var cards = events
            .Select(datingEvent => ToCard(datingEvent, profile, organizerProfiles, tagWeights, nowUtc, recommendationCityId))
            .Where(card => !request.OnlyEligibleForMe || card.IsEligibleForCurrentUser)
            .ToList();

        cards = Sort(cards, request.Sort)
            .Skip((page - 1) * pageSize)
            .Take(pageSize + 1)
            .ToList();

        var hasNextPage = cards.Count > pageSize;
        return new EndUserEventCatalogPageDto(
            cards.Take(pageSize).ToList(),
            page,
            pageSize,
            hasNextPage);
    }

    public async Task<EndUserEventDetailsDto?> GetDetailsAsync(long eventId, long? userId, CancellationToken cancellationToken)
    {
        var nowUtc = DateTime.UtcNow;
        var profile = await LoadProfileAsync(userId, cancellationToken);
        var datingEvent = await _db.DatingEvents
            .AsSplitQuery()
            .Include(item => item.EventType)
            .Include(item => item.EventMode)
            .Include(item => item.OnlineEventPlatform)
            .Include(item => item.Country)
            .Include(item => item.City)
            .Include(item => item.EventTags)
            .ThenInclude(eventTag => eventTag.Tag)
            .Include(item => item.Faqs)
            .Include(item => item.Tickets)
            .FirstOrDefaultAsync(item =>
                item.Id == eventId
                && item.ApprovalStatus == EventApprovalStatus.Approved
                && !item.IsCancelled
                && item.LifecycleStatus != EventLifecycleStatus.Cancelled
                && item.DateTimeEnd > nowUtc,
                cancellationToken);

        if (datingEvent is null)
            return null;

        var organizerProfile = await _db.EventPlannerProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.UserId == datingEvent.EventPlannerUserId, cancellationToken);

        var eligibility = _eligibility.Evaluate(profile, datingEvent, nowUtc);
        var status = _statusResolver.Resolve(datingEvent, nowUtc);
        var capacities = RemainingCapacities(datingEvent);

        var (venueName, address) = SplitStoredAddress(datingEvent.Address);

        return new EndUserEventDetailsDto(
            datingEvent.Id,
            datingEvent.EventCode,
            datingEvent.Title,
            datingEvent.EventType.Name,
            datingEvent.Country?.Name ?? datingEvent.Location.Country,
            datingEvent.City?.Name ?? datingEvent.Location.City,
            datingEvent.Location.Region,
            venueName,
            address,
            datingEvent.EventMode.IsOnline,
            datingEvent.OnlineEventPlatform?.Name,
            datingEvent.OnlineAccessInstructions,
            datingEvent.DateTimeStart,
            datingEvent.DateTimeEnd,
            ImageUrls(datingEvent),
            datingEvent.EventDescriptionHtml,
            datingEvent.Tags,
            datingEvent.Faqs
                .OrderBy(item => item.DisplayOrder)
                .Select(item => new EndUserEventFaqDto(item.Question, item.Answer))
                .ToList(),
            datingEvent.AgeRangeForMale.Min,
            datingEvent.AgeRangeForMale.Max,
            datingEvent.AgeRangeForFemale.Min,
            datingEvent.AgeRangeForFemale.Max,
            datingEvent.MinimumEducationLevelId,
            datingEvent.EducationLevelRestriction,
            datingEvent.MaleTicketPrice,
            datingEvent.FemaleTicketPrice,
            datingEvent.CurrencyCode,
            datingEvent.PaymentCollectionMethod,
            datingEvent.OrganizerPaymentInstructions,
            status,
            datingEvent.MaleCapacity,
            datingEvent.FemaleCapacity,
            capacities.Male,
            capacities.Female,
            eligibility.CanBuyTicket,
            eligibility.ReasonCode,
            eligibility.Message,
            profile?.ProfileStatus,
            ToOrganizerSummary(datingEvent.EventPlannerUserId, organizerProfile));
    }

    private IQueryable<DatingEvent> BuildBaseQuery(EndUserEventCatalogRequest request, long? effectiveCityId, DateTime nowUtc)
    {
        var query = _db.DatingEvents
            .AsNoTracking()
            .Include(item => item.EventType)
            .Include(item => item.EventMode)
            .Include(item => item.Country)
            .Include(item => item.City)
            .Include(item => item.EventTags)
            .ThenInclude(eventTag => eventTag.Tag)
            .Include(item => item.Tickets)
            .Where(item =>
                item.ApprovalStatus == EventApprovalStatus.Approved
                && !item.IsCancelled
                && item.LifecycleStatus != EventLifecycleStatus.Cancelled
                && item.DateTimeEnd > nowUtc);

        if (!request.IncludeOnline && !request.IncludeInPerson)
        {
            query = query.Where(item => false);
        }
        else if (effectiveCityId is not null)
        {
            query = (request.IncludeOnline, request.IncludeInPerson) switch
            {
                (true, true) => query.Where(item => item.CityId == effectiveCityId || item.EventMode.IsOnline),
                (true, false) => query.Where(item => item.EventMode.IsOnline),
                (false, true) => query.Where(item => item.CityId == effectiveCityId && !item.EventMode.IsOnline),
                _ => query
            };
        }
        else if (request.IncludeOnline != request.IncludeInPerson)
        {
            query = request.IncludeOnline
                ? query.Where(item => item.EventMode.IsOnline)
                : query.Where(item => !item.EventMode.IsOnline);
        }

        if (request.EventTypeId is not null)
            query = query.Where(item => item.EventTypeId == request.EventTypeId);

        if (request.DateFromUtc is not null)
            query = query.Where(item => item.DateTimeStart >= request.DateFromUtc);

        if (request.DateToUtc is not null)
            query = query.Where(item => item.DateTimeStart <= request.DateToUtc);

        if (request.PriceMin is not null)
            query = query.Where(item => item.MaleTicketPrice >= request.PriceMin || item.FemaleTicketPrice >= request.PriceMin);

        if (request.PriceMax is not null)
            query = query.Where(item => item.MaleTicketPrice <= request.PriceMax || item.FemaleTicketPrice <= request.PriceMax);

        if (request.EventTypeId is not null)
            query = query.Where(item => item.EventTypeId == request.EventTypeId);

        if (request.Age is not null)
        {
            query = query.Where(item =>
                (item.AgeRangeForMale.Min <= request.Age && item.AgeRangeForMale.Max >= request.Age)
                || (item.AgeRangeForFemale.Min <= request.Age && item.AgeRangeForFemale.Max >= request.Age));
        }

        if (request.EducationLevelId is not null)
        {
            var selectedRank = EducationLevelIdRank(request.EducationLevelId.Value);
            query = query.Where(item => item.MinimumEducationLevelId == null || item.MinimumEducationLevelId <= selectedRank + 1);
        }

        return query.OrderBy(item => item.DateTimeStart).ThenBy(item => item.Id);
    }

    private async Task<UserProfile?> LoadProfileAsync(long? userId, CancellationToken cancellationToken)
    {
        if (userId is null)
            return null;

        var profile = await _db.UserProfiles
            .Include(item => item.Interests)
            .FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken);
        profile?.RefreshProfileStatus();
        return profile;
    }

    private async Task<IReadOnlyDictionary<long, EventPlannerProfile>> LoadOrganizerProfilesAsync(
        IReadOnlyCollection<DatingEvent> events,
        CancellationToken cancellationToken)
    {
        var organizerIds = events.Select(item => item.EventPlannerUserId).Distinct().ToList();
        return await _db.EventPlannerProfiles
            .AsNoTracking()
            .Where(item => organizerIds.Contains(item.UserId))
            .ToDictionaryAsync(item => item.UserId, cancellationToken);
    }

    private async Task<IReadOnlyDictionary<long, int>> LoadTagWeightsAsync(UserProfile? profile, CancellationToken cancellationToken)
    {
        if (profile is null || profile.Interests.Count == 0)
            return new Dictionary<long, int>();

        var interestIds = profile.Interests.Select(item => item.Id).ToList();
        return await _db.InterestTagMappings
            .AsNoTracking()
            .Where(item => item.IsActive && interestIds.Contains(item.InterestId))
            .GroupBy(item => item.TagId)
            .Select(group => new { TagId = group.Key, Weight = group.Sum(item => item.RelevanceWeight) })
            .ToDictionaryAsync(item => item.TagId, item => item.Weight, cancellationToken);
    }

    private EndUserEventCardDto ToCard(
        DatingEvent datingEvent,
        UserProfile? profile,
        IReadOnlyDictionary<long, EventPlannerProfile> organizerProfiles,
        IReadOnlyDictionary<long, int> tagWeights,
        DateTime nowUtc,
        long? effectiveCityId)
    {
        var eligibility = _eligibility.Evaluate(profile, datingEvent, nowUtc);
        var status = _statusResolver.Resolve(datingEvent, nowUtc);
        var capacities = RemainingCapacities(datingEvent);
        organizerProfiles.TryGetValue(datingEvent.EventPlannerUserId, out var organizerProfile);
        var score = RecommendationScore(datingEvent, eligibility, tagWeights, effectiveCityId);

        return new EndUserEventCardDto(
            datingEvent.Id,
            datingEvent.EventCode,
            datingEvent.Title,
            FirstImageUrl(datingEvent),
            datingEvent.EventType.Name,
            datingEvent.Country?.Name ?? datingEvent.Location.Country,
            datingEvent.City?.Name ?? datingEvent.Location.City,
            datingEvent.EventMode.IsOnline,
            datingEvent.DateTimeStart,
            datingEvent.DateTimeEnd,
            Math.Min(datingEvent.MaleTicketPrice, datingEvent.FemaleTicketPrice),
            datingEvent.CurrencyCode,
            datingEvent.PaymentCollectionMethod,
            datingEvent.Tags,
            status,
            capacities.Male,
            capacities.Female,
            eligibility.CanBuyTicket,
            eligibility.ReasonCode,
            eligibility.Message,
            profile?.ProfileStatus,
            score,
            organizerProfile?.Title,
            organizerProfile?.AverageRating ?? 0,
            organizerProfile?.TotalSurveyCount ?? 0);
    }

    private static IEnumerable<EndUserEventCardDto> Sort(IReadOnlyCollection<EndUserEventCardDto> cards, EndUserEventSort sort) =>
        sort switch
        {
            EndUserEventSort.Soonest => cards.OrderBy(item => item.StartsAtUtc).ThenBy(item => item.Id),
            EndUserEventSort.Newest => cards.OrderByDescending(item => item.Id),
            EndUserEventSort.PriceLowToHigh => cards.OrderBy(item => item.StartingPrice).ThenBy(item => item.StartsAtUtc),
            _ => cards.OrderByDescending(item => item.RecommendationScore).ThenBy(item => item.StartsAtUtc).ThenBy(item => item.Id)
        };

    private static int RecommendationScore(
        DatingEvent datingEvent,
        EndUserEventEligibilityResult eligibility,
        IReadOnlyDictionary<long, int> tagWeights,
        long? effectiveCityId)
    {
        var score = 0;
        foreach (var eventTag in datingEvent.EventTags)
        {
            if (tagWeights.TryGetValue(eventTag.TagId, out var weight))
                score += weight;
        }

        if (effectiveCityId is not null && datingEvent.CityId == effectiveCityId)
            score += 50;

        if (datingEvent.EventMode.IsOnline)
            score += 20;

        if (eligibility.CanBuyTicket)
            score += 15;

        return score;
    }

    private static (int Male, int Female) RemainingCapacities(DatingEvent datingEvent)
    {
        var activeTickets = datingEvent.Tickets.Where(ticket => !ticket.IsRefunded && !ticket.IsRemoved).ToList();
        var maleSold = activeTickets.Count(ticket => ticket.Gender == Gender.Male);
        var femaleSold = activeTickets.Count(ticket => ticket.Gender == Gender.Female);
        return (
            Math.Max(0, datingEvent.MaleCapacity - maleSold),
            Math.Max(0, datingEvent.FemaleCapacity - femaleSold));
    }

    private static string? FirstImageUrl(DatingEvent datingEvent) =>
        ImageUrls(datingEvent).FirstOrDefault();

    private static IReadOnlyList<string> ImageUrls(DatingEvent datingEvent) =>
        new[] { datingEvent.EventImage1, datingEvent.EventImage2, datingEvent.EventImage3 }
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Select(item => item!)
            .ToList();

    private static OrganizerSummaryDto ToOrganizerSummary(long userId, EventPlannerProfile? profile) =>
        new(
            userId,
            profile?.Title,
            profile?.PictureUrl,
            profile?.Resume,
            profile?.AverageRating ?? 0,
            profile?.TotalSurveyCount ?? 0,
            profile?.HostedEventCount ?? 0,
            profile?.CancelledEventCount ?? 0,
            profile?.CompletedEventCount ?? 0);

    private static (string? VenueName, string Address) SplitStoredAddress(string storedAddress)
    {
        if (string.IsNullOrWhiteSpace(storedAddress))
            return (null, string.Empty);

        var parts = storedAddress.Split('|', 2, StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
            return (null, parts[0]);

        return string.Equals(parts[0], parts[1], StringComparison.Ordinal)
            ? (null, parts[1])
            : (parts[0], parts[1]);
    }

    private static int EducationLevelIdRank(long educationLevelId) => educationLevelId switch
    {
        1 => 0,
        2 => 1,
        3 => 2,
        4 => 3,
        5 => 4,
        _ => 0
    };
}
