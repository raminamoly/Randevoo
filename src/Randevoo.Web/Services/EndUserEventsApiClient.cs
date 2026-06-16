using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace Randevoo.Web.Services;

public sealed class EndUserEventsApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly EndUserSessionService _session;

    public EndUserEventsApiClient(IHttpClientFactory httpClientFactory, EndUserSessionService session)
    {
        _httpClientFactory = httpClientFactory;
        _session = session;
    }

    public async Task<EndUserEventPageViewModel> ListAsync(EndUserEventListRequest request, CancellationToken cancellationToken)
    {
        var client = CreateClient();
        var query = BuildQueryString(new Dictionary<string, string?>
        {
            ["page"] = request.Page.ToString(),
            ["pageSize"] = request.PageSize.ToString(),
            ["cityId"] = request.CityId?.ToString(),
            ["includeOnline"] = request.IncludeOnline.ToString().ToLowerInvariant(),
            ["includeInPerson"] = request.IncludeInPerson.ToString().ToLowerInvariant(),
            ["eventTypeId"] = request.EventTypeId?.ToString(),
            ["age"] = request.Age?.ToString(),
            ["educationLevelId"] = request.EducationLevelId?.ToString(),
            ["sort"] = request.Sort
        });

        var routePrefix = _session.IsSignedIn ? "/api/v1/platform/events" : "/api/v1/website/events";
        return await client.GetFromJsonAsync<EndUserEventPageViewModel>($"{routePrefix}{query}", cancellationToken)
            ?? EndUserEventPageViewModel.Empty(request.Page, request.PageSize);
    }

    public async Task<EndUserEventDetailsViewModel?> GetDetailsAsync(long id, CancellationToken cancellationToken)
    {
        var client = CreateClient();
        var routePrefix = _session.IsSignedIn ? "/api/v1/platform/events" : "/api/v1/website/events";
        return await client.GetFromJsonAsync<EndUserEventDetailsViewModel>($"{routePrefix}/{id}", cancellationToken);
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("RandevooApi");
        var token = _session.GetAccessToken();
        if (!string.IsNullOrWhiteSpace(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    private static string BuildQueryString(IReadOnlyDictionary<string, string?> values)
    {
        var parts = values
            .Where(item => !string.IsNullOrWhiteSpace(item.Value))
            .Select(item => $"{Uri.EscapeDataString(item.Key)}={Uri.EscapeDataString(item.Value!)}")
            .ToList();

        return parts.Count == 0 ? string.Empty : $"?{string.Join("&", parts)}";
    }
}

public sealed record EndUserEventListRequest(
    int Page,
    int PageSize,
    long? CityId,
    bool IncludeOnline,
    bool IncludeInPerson,
    long? EventTypeId,
    int? Age,
    long? EducationLevelId,
    string Sort);

public sealed record EndUserEventPageViewModel(
    IReadOnlyList<EndUserEventCardViewModel> Items,
    int Page,
    int PageSize,
    bool HasNextPage)
{
    public static EndUserEventPageViewModel Empty(int page, int pageSize) => new([], page, pageSize, false);
}

public sealed record EndUserEventCardViewModel(
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
    int PaymentCollectionMethod,
    IReadOnlyList<string> Tags,
    int UserFacingStatus,
    int MaleRemainingCapacity,
    int FemaleRemainingCapacity,
    bool IsEligibleForCurrentUser,
    string EligibilityReasonCode,
    string EligibilityMessage,
    int? CurrentUserProfileStatus,
    int RecommendationScore,
    string? OrganizerTitle,
    decimal OrganizerAverageRating,
    int OrganizerSurveyCount);

public sealed record EndUserEventDetailsViewModel(
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
    IReadOnlyList<EventFaqViewModel> Faqs,
    int MaleMinAge,
    int MaleMaxAge,
    int FemaleMinAge,
    int FemaleMaxAge,
    long? MinimumEducationLevelId,
    int EducationLevelRestriction,
    decimal MaleTicketPrice,
    decimal FemaleTicketPrice,
    string CurrencyCode,
    int PaymentCollectionMethod,
    string? OrganizerPaymentInstructions,
    int UserFacingStatus,
    int MaleCapacity,
    int FemaleCapacity,
    int MaleRemainingCapacity,
    int FemaleRemainingCapacity,
    bool CanBuyTicket,
    string EligibilityReasonCode,
    string EligibilityMessage,
    int? CurrentUserProfileStatus,
    OrganizerSummaryViewModel Organizer);

public sealed record EventFaqViewModel(
    string Question,
    string Answer);

public sealed record OrganizerSummaryViewModel(
    long UserId,
    string? Title,
    string? PictureUrl,
    string? Resume,
    decimal AverageRating,
    int SurveyCount,
    int HostedEventCount,
    int CancelledEventCount,
    int CompletedEventCount);
