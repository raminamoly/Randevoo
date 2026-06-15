using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Models.Participants;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.Permissions;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Domain.Enums;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Pages.Participants;

[Authorize(Policy = Policies.AdminPlannerOrSupport)]
public class IndexModel : PageModel
{
    private readonly RandevooDbContext _db;
    private readonly IEventsApiClient _eventsApi;
    private readonly IOperationPermissionService _permissions;
    private readonly CurrentSessionState _session;

    public IndexModel(
        RandevooDbContext db,
        IEventsApiClient eventsApi,
        IOperationPermissionService permissions,
        CurrentSessionState session)
    {
        _db = db;
        _eventsApi = eventsApi;
        _permissions = permissions;
        _session = session;
    }

    [BindProperty(SupportsGet = true)]
    public long? EventId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ProfileStatus { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? TicketStatus { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Gender { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? FilterEventId { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? BuyerUserId { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? TicketOrderId { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Sort { get; set; } = "purchase-desc";

    [BindProperty(SupportsGet = true)]
    public string View { get; set; } = "table";

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 25;

    [BindProperty]
    public EmergencyRefundInput Input { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public ParticipantListResult Result { get; private set; } = new();
    public IReadOnlySet<string> AllowedActions { get; private set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public SelectList EventOptions { get; private set; } = new(Array.Empty<SelectListItem>());
    public string? EventTitle { get; private set; }
    public bool IsRtl => _session.IsRtl;
    public bool IsAdmin => _session.CurrentUser?.Role == AdminRole.Admin;
    public bool IsEventScoped => EventId.HasValue;
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(Result.TotalCount / (double)Math.Clamp(PageSize, 10, 100)));
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public bool IsCardView => string.Equals(View, "cards", StringComparison.OrdinalIgnoreCase);
    public bool CanSeeContactInfo => AllowedActions.Contains("viewContactInfo");
    public bool CanRefund => AllowedActions.Contains("emergencyRefund");
    public bool HasActiveFilters => !string.IsNullOrWhiteSpace(Search)
        || !string.IsNullOrWhiteSpace(ProfileStatus)
        || !string.IsNullOrWhiteSpace(TicketStatus)
        || !string.IsNullOrWhiteSpace(Gender)
        || FilterEventId.HasValue
        || BuyerUserId.HasValue
        || TicketOrderId.HasValue
        || FromDate.HasValue
        || ToDate.HasValue
        || !string.Equals(Sort, "purchase-desc", StringComparison.OrdinalIgnoreCase);

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var access = await PrepareAccessAsync(cancellationToken);
        if (access is not null)
            return access;

        await LoadAsync(cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostRefundAsync(CancellationToken cancellationToken)
    {
        var access = await PrepareAccessAsync(cancellationToken);
        if (access is not null)
            return access;

        if (!CanRefund)
            ModelState.AddModelError(string.Empty, "برای بازگشت اضطراری شرکت‌کننده دسترسی ندارید.");

        if (!ModelState.IsValid)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        var ticketEventId = await _db.EventTickets
            .AsNoTracking()
            .Where(item => item.Id == Input.TicketId)
            .Select(item => item.DatingEventId)
            .FirstOrDefaultAsync(cancellationToken);

        if (ticketEventId == 0)
            throw new InvalidOperationException("بلیت مورد نظر پیدا نشد.");

        await _eventsApi.EmergencyRefundTicketAsync(ticketEventId, Input.TicketId, current, Input.Reason, cancellationToken);
        StatusMessage = "بازگشت اضطراری شرکت‌کننده ثبت شد.";
        return RedirectToPage(BuildRouteValues());
    }

    public string GetTicketStatusClass(ParticipantListItem item)
    {
        if (item.IsRemoved)
            return "status-cancelled";

        return item.IsRefunded ? "status-closed" : "status-approved";
    }

    public string GetProfileStatusClass(ParticipantListItem item)
        => item.IsProfileComplete ? "status-approved" : "status-pending";

    private async Task<IActionResult?> PrepareAccessAsync(CancellationToken cancellationToken)
    {
        var current = _session.CurrentUser;
        if (current is null)
            return Challenge();

        AllowedActions = await _permissions.GetAllowedActionsAsync(current, "participants", cancellationToken);
        if (!AllowedActions.Contains("list"))
            return Forbid();

        if (!EventId.HasValue && current.Role != AdminRole.Admin)
            return Forbid();

        PageNumber = Math.Max(PageNumber, 1);
        PageSize = Math.Clamp(PageSize, 10, 100);
        return null;
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        await LoadEventOptionsAsync(cancellationToken);
        Result = await QueryParticipantsAsync(cancellationToken);
    }

    private async Task LoadEventOptionsAsync(CancellationToken cancellationToken)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        var eventsQuery = _db.DatingEvents
            .AsNoTracking()
            .OrderByDescending(item => item.DateTimeStart)
            .AsQueryable();

        if (current.Role == AdminRole.EventPlanner)
            eventsQuery = eventsQuery.Where(item => item.EventPlannerUserId == current.Id);

        var events = await eventsQuery
            .Select(item => new
            {
                item.Id,
                Title = $"#{item.EventCode} - {item.Title}"
            })
            .Take(250)
            .ToListAsync(cancellationToken);

        EventOptions = new SelectList(events, "Id", "Title", FilterEventId ?? EventId);

        if (EventId is long eventId)
        {
            EventTitle = events.FirstOrDefault(item => item.Id == eventId)?.Title
                ?? await _db.DatingEvents
                    .AsNoTracking()
                    .Where(item => item.Id == eventId)
                    .Select(item => $"#{item.EventCode} - {item.Title}")
                    .FirstOrDefaultAsync(cancellationToken);
        }
    }

    private async Task<ParticipantListResult> QueryParticipantsAsync(CancellationToken cancellationToken)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        var query = _db.EventTickets
            .AsNoTracking()
            .AsQueryable();

        if (EventId is long routeEventId)
            query = query.Where(item => item.DatingEventId == routeEventId);
        else if (FilterEventId is long filterEventId)
            query = query.Where(item => item.DatingEventId == filterEventId);

        if (current.Role == AdminRole.EventPlanner)
            query = query.Where(item => item.DatingEvent.EventPlannerUserId == current.Id);

        if (BuyerUserId is long buyerUserId)
            query = query.Where(item => item.TicketOrder.BuyerUserId == buyerUserId);

        if (TicketOrderId is long ticketOrderId)
            query = query.Where(item => item.TicketOrderId == ticketOrderId);

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var search = Search.Trim();
            var like = $"%{search}%";
            var normalizedCode = search.Trim().TrimStart('#');
            var hasEventCode = int.TryParse(normalizedCode, out var eventCode);
            query = query.Where(item =>
                (hasEventCode && item.DatingEvent.EventCode == eventCode)
                || EF.Functions.Like(item.DatingEvent.EventCode.ToString(), like)
                ||
                EF.Functions.Like(item.User.Profile!.DisplayName, like)
                || EF.Functions.Like(item.DatingEvent.Title, like)
                || EF.Functions.Like(item.TicketOrder.BuyerUser.Profile!.DisplayName, like)
                || (item.User.Email != null && EF.Functions.Like(item.User.Email, like))
                || (item.TicketOrder.BuyerUser.Email != null && EF.Functions.Like(item.TicketOrder.BuyerUser.Email, like))
                || (item.DiscountCode != null && EF.Functions.Like(item.DiscountCode, like))
                || (CanSeeContactInfo && EF.Functions.Like(item.User.MobileNumber, like))
                || (CanSeeContactInfo && EF.Functions.Like(item.TicketOrder.BuyerUser.MobileNumber, like))
                || EF.Functions.Like(item.Id.ToString(), like));
        }

        if (!string.IsNullOrWhiteSpace(ProfileStatus))
        {
            query = ProfileStatus switch
            {
                "completed" => query.Where(item => item.User.Profile != null
                    && item.User.Profile.EducationLevel != EducationLevel.NotSpecified
                    && item.User.Profile.Images.Count > 0
                    && item.User.Profile.Interests.Count > 0
                    && item.User.Profile.CityId != null
                    && item.User.Profile.CountryId != null),
                "pending" => query.Where(item => item.User.Profile == null
                    || item.User.Profile.EducationLevel == EducationLevel.NotSpecified
                    || item.User.Profile.Images.Count == 0
                    || item.User.Profile.Interests.Count == 0
                    || item.User.Profile.CityId == null
                    || item.User.Profile.CountryId == null),
                _ => query
            };
        }

        if (!string.IsNullOrWhiteSpace(TicketStatus))
        {
            query = TicketStatus switch
            {
                "active" => query.Where(item => !item.IsRefunded && !item.IsRemoved),
                "refunded" => query.Where(item => item.IsRefunded && !item.IsRemoved),
                "removed" => query.Where(item => item.IsRemoved),
                _ => query
            };
        }

        if (!string.IsNullOrWhiteSpace(Gender))
        {
            query = Gender switch
            {
                "male" => query.Where(item => item.Gender == Randevoo.Domain.Enums.Gender.Male),
                "female" => query.Where(item => item.Gender == Randevoo.Domain.Enums.Gender.Female),
                _ => query
            };
        }

        if (FromDate is DateTime fromDate)
            query = query.Where(item => item.CreatedAt >= fromDate.Date);

        if (ToDate is DateTime toDate)
            query = query.Where(item => item.CreatedAt < toDate.Date.AddDays(1));

        var summary = await query
            .GroupBy(_ => 1)
            .Select(group => new ParticipantListSummary
            {
                TotalParticipants = group.Count(),
                TotalOrders = group.Select(item => item.TicketOrderId).Distinct().Count(),
                CompletedProfiles = group.Count(item => item.User.Profile != null
                    && item.User.Profile.EducationLevel != EducationLevel.NotSpecified
                    && item.User.Profile.Images.Count > 0
                    && item.User.Profile.Interests.Count > 0
                    && item.User.Profile.CityId != null
                    && item.User.Profile.CountryId != null),
                PendingProfiles = group.Count(item => item.User.Profile == null
                    || item.User.Profile.EducationLevel == EducationLevel.NotSpecified
                    || item.User.Profile.Images.Count == 0
                    || item.User.Profile.Interests.Count == 0
                    || item.User.Profile.CityId == null
                    || item.User.Profile.CountryId == null),
                ActiveTickets = group.Count(item => !item.IsRefunded && !item.IsRemoved),
                CancelledTickets = group.Count(item => item.IsRefunded || item.IsRemoved)
            })
            .FirstOrDefaultAsync(cancellationToken) ?? new ParticipantListSummary();

        var totalCount = summary.TotalParticipants;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
        PageNumber = Math.Min(PageNumber, totalPages);

        query = Sort switch
        {
            "purchase-asc" => query.OrderBy(item => item.CreatedAt).ThenBy(item => item.Id),
            "name" => query.OrderBy(item => item.User.Profile!.DisplayName).ThenBy(item => item.Id),
            "event" => query.OrderBy(item => item.DatingEvent.Title).ThenByDescending(item => item.CreatedAt),
            "profile-status" => query.OrderBy(item => item.User.Profile == null
                || item.User.Profile.EducationLevel == EducationLevel.NotSpecified
                || item.User.Profile.Images.Count == 0
                || item.User.Profile.Interests.Count == 0)
                .ThenByDescending(item => item.CreatedAt),
            "payment-status" => query.OrderBy(item => item.IsRefunded).ThenBy(item => item.IsRemoved).ThenByDescending(item => item.CreatedAt),
            "price-desc" => query.OrderByDescending(item => item.Price).ThenByDescending(item => item.CreatedAt),
            "price-asc" => query.OrderBy(item => item.Price).ThenByDescending(item => item.CreatedAt),
            _ => query.OrderByDescending(item => item.CreatedAt).ThenByDescending(item => item.Id)
        };

        var rows = await query
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .Select(item => new
            {
                TicketId = item.Id,
                item.TicketOrderId,
                item.DatingEventId,
                EventTitle = $"#{item.DatingEvent.EventCode} - {item.DatingEvent.Title}",
                item.DatingEvent.EventPlannerUserId,
                EventPlannerName = item.DatingEvent.EventPlannerUser.Profile != null
                    ? item.DatingEvent.EventPlannerUser.Profile.DisplayName
                    : item.DatingEvent.EventPlannerUser.MobileNumber,
                item.UserId,
                ParticipantName = item.User.Profile != null ? item.User.Profile.DisplayName : item.User.MobileNumber,
                ParticipantMobile = CanSeeContactInfo ? item.User.MobileNumber : null,
                BuyerUserId = item.TicketOrder.BuyerUserId,
                BuyerName = item.TicketOrder.BuyerUser.Profile != null ? item.TicketOrder.BuyerUser.Profile.DisplayName : item.TicketOrder.BuyerUser.MobileNumber,
                BuyerMobile = CanSeeContactInfo ? item.TicketOrder.BuyerUser.MobileNumber : null,
                ProfileImageUrl = item.User.Profile == null
                    ? null
                    : item.User.Profile.Images
                        .OrderByDescending(image => image.IsPrimary)
                        .ThenBy(image => image.DisplayOrder)
                        .Select(image => image.ImageUrl)
                        .FirstOrDefault(),
                GenderLookupTitle = item.User.Profile != null && item.User.Profile.GenderLookup != null ? item.User.Profile.GenderLookup.Title : null,
                item.Gender,
                Age = item.User.Profile != null ? item.User.Profile.Age : 0,
                EducationLevelLookupTitle = item.User.Profile != null && item.User.Profile.EducationLevelLookup != null ? item.User.Profile.EducationLevelLookup.Title : null,
                EducationLevel = item.User.Profile != null ? item.User.Profile.EducationLevel : EducationLevel.NotSpecified,
                CityName = item.User.Profile != null && item.User.Profile.City != null ? item.User.Profile.City.Name : null,
                IsProfileComplete = item.User.Profile != null
                    && item.User.Profile.EducationLevel != EducationLevel.NotSpecified
                    && item.User.Profile.Images.Count > 0
                    && item.User.Profile.Interests.Count > 0
                    && item.User.Profile.CityId != null
                    && item.User.Profile.CountryId != null,
                item.Price,
                item.CurrencyCode,
                item.IsRefunded,
                item.IsRemoved,
                item.CreatedAt,
                item.RemovalReason
            })
            .ToListAsync(cancellationToken);

        return new ParticipantListResult
        {
            TotalCount = totalCount,
            Summary = summary,
            Items = rows.Select(item => new ParticipantListItem
            {
                TicketId = item.TicketId,
                TicketOrderId = item.TicketOrderId,
                EventId = item.DatingEventId,
                EventTitle = item.EventTitle,
                EventPlannerUserId = item.EventPlannerUserId,
                EventPlannerName = item.EventPlannerName,
                ParticipantUserId = item.UserId,
                ParticipantName = item.ParticipantName,
                ParticipantMobile = item.ParticipantMobile,
                ProfileImageUrl = item.ProfileImageUrl,
                GenderTitle = item.GenderLookupTitle ?? DisplayFormatter.Gender(item.Gender),
                Age = item.Age,
                EducationLevelTitle = item.EducationLevelLookupTitle ?? item.EducationLevel.ToString(),
                CityTitle = item.CityName ?? "ثبت نشده",
                IsProfileComplete = item.IsProfileComplete,
                TicketPrice = item.Price,
                TicketCurrencyCode = item.CurrencyCode,
                IsRefunded = item.IsRefunded,
                IsRemoved = item.IsRemoved,
                TicketStatus = item.IsRemoved
                    ? "حذف و بازگشت وجه"
                    : item.IsRefunded
                        ? "بازگشت وجه"
                        : "فعال",
                PurchasedAtUtc = DateTime.SpecifyKind(item.CreatedAt, DateTimeKind.Utc),
                RemovalReason = item.RemovalReason,
                BuyerUserId = item.BuyerUserId,
                BuyerName = item.BuyerName,
                BuyerMobile = item.BuyerMobile
            }).ToList()
        };
    }

    private RouteValueDictionary BuildRouteValues()
    {
        return new RouteValueDictionary
        {
            [nameof(EventId)] = EventId,
            [nameof(Search)] = Search,
            [nameof(ProfileStatus)] = ProfileStatus,
            [nameof(TicketStatus)] = TicketStatus,
            [nameof(Gender)] = Gender,
            [nameof(FilterEventId)] = FilterEventId,
            [nameof(BuyerUserId)] = BuyerUserId,
            [nameof(TicketOrderId)] = TicketOrderId,
            [nameof(FromDate)] = FromDate?.ToString("yyyy-MM-dd"),
            [nameof(ToDate)] = ToDate?.ToString("yyyy-MM-dd"),
            [nameof(Sort)] = Sort,
            [nameof(View)] = View,
            [nameof(PageNumber)] = PageNumber,
            [nameof(PageSize)] = PageSize
        };
    }
}
