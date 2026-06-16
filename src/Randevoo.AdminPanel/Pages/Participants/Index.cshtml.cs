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
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Pages.Participants;

[Authorize(Policy = Policies.AdminPlannerOrSupport)]
public class IndexModel : PageModel
{
    private readonly RandevooDbContext _db;
    private readonly IEventsApiClient _eventsApi;
    private readonly ILocationsApiClient _locationsApi;
    private readonly IOperationPermissionService _permissions;
    private readonly CurrentSessionState _session;

    public IndexModel(
        RandevooDbContext db,
        IEventsApiClient eventsApi,
        ILocationsApiClient locationsApi,
        IOperationPermissionService permissions,
        CurrentSessionState session)
    {
        _db = db;
        _eventsApi = eventsApi;
        _locationsApi = locationsApi;
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
    public long? CityId { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? ZodiacSignId { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? IsActive { get; set; }

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
    public string Sort { get; set; } = string.Empty;

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
    public SelectList CityOptions { get; private set; } = new(Array.Empty<SelectListItem>());
    public SelectList GenderOptions { get; private set; } = new(Array.Empty<SelectListItem>());
    public SelectList ZodiacSignOptions { get; private set; } = new(Array.Empty<SelectListItem>());
    public string? EventTitle { get; private set; }
    public bool IsRtl => _session.IsRtl;
    public bool IsAdmin => _session.CurrentUser?.Role == AdminRole.Admin;
    public bool IsEventScoped => EventId.HasValue;
    public bool IsAdminDirectory => IsAdmin && !EventId.HasValue && !BuyerUserId.HasValue && !TicketOrderId.HasValue;
    public string DefaultSort => IsAdminDirectory ? "last-activity" : "purchase-desc";
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
        || CityId.HasValue
        || ZodiacSignId.HasValue
        || IsActive.HasValue
        || FilterEventId.HasValue
        || BuyerUserId.HasValue
        || TicketOrderId.HasValue
        || FromDate.HasValue
        || ToDate.HasValue
        || !string.Equals(Sort, DefaultSort, StringComparison.OrdinalIgnoreCase);

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
        if (item.IsDirectoryRow)
            return item.IsActive ? "status-approved" : "status-closed";

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
        if (string.IsNullOrWhiteSpace(Sort))
            Sort = DefaultSort;

        if (IsAdminDirectory && string.Equals(Sort, "purchase-desc", StringComparison.OrdinalIgnoreCase))
            Sort = DefaultSort;

        View = IsAdminDirectory
            ? "table"
            : string.Equals(View, "table", StringComparison.OrdinalIgnoreCase) ? "table" : "cards";

        return null;
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        await LoadEventOptionsAsync(cancellationToken);
        await LoadProfileFilterOptionsAsync(cancellationToken);
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

    private async Task LoadProfileFilterOptionsAsync(CancellationToken cancellationToken)
    {
        if (!IsAdminDirectory)
            return;

        var cities = await _locationsApi.GetCitiesAsync(cancellationToken: cancellationToken);
        var genders = await _locationsApi.GetGendersAsync(cancellationToken);
        var zodiacSigns = await _locationsApi.GetZodiacSignsAsync(cancellationToken);

        CityOptions = new SelectList(cities, nameof(CityOption.Id), nameof(CityOption.Name), CityId);
        GenderOptions = new SelectList(genders, nameof(GenderOption.Id), nameof(GenderOption.Title), MapGenderFilterId(Gender));
        ZodiacSignOptions = new SelectList(zodiacSigns, nameof(ZodiacSignOption.Id), nameof(ZodiacSignOption.Title), ZodiacSignId);
    }

    private Task<ParticipantListResult> QueryParticipantsAsync(CancellationToken cancellationToken)
        => IsAdminDirectory
            ? QueryAdminParticipantDirectoryAsync(cancellationToken)
            : QueryTicketParticipantsAsync(cancellationToken);

    private async Task<ParticipantListResult> QueryAdminParticipantDirectoryAsync(CancellationToken cancellationToken)
    {
        var query = _db.Users
            .AsNoTracking()
            .Where(item => item.Role == UserRole.EndUser)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var search = Search.Trim();
            var like = $"%{search}%";
            query = query.Where(item =>
                EF.Functions.Like(item.MobileNumber, like)
                || (item.Email != null && EF.Functions.Like(item.Email, like))
                || (item.Profile != null && EF.Functions.Like(item.Profile.DisplayName, like)));
        }

        if (FilterEventId is long filterEventId)
            query = query.Where(item => _db.EventTickets.Any(ticket => ticket.UserId == item.Id && ticket.DatingEventId == filterEventId));

        if (BuyerUserId is long buyerUserId)
            query = query.Where(item => _db.EventTickets.Any(ticket => ticket.UserId == item.Id && ticket.TicketOrder.BuyerUserId == buyerUserId));

        if (TicketOrderId is long ticketOrderId)
            query = query.Where(item => _db.EventTickets.Any(ticket => ticket.UserId == item.Id && ticket.TicketOrderId == ticketOrderId));

        if (CityId is long cityId)
            query = query.Where(item => item.Profile != null && item.Profile.CityId == cityId);

        if (ZodiacSignId is long zodiacSignId)
            query = query.Where(item => item.Profile != null && item.Profile.ZodiacSignId == zodiacSignId);

        if (IsActive.HasValue)
            query = query.Where(item => item.IsActive == IsActive.Value);

        if (!string.IsNullOrWhiteSpace(ProfileStatus))
        {
            query = ProfileStatus switch
            {
                "completed" => query.Where(IsUserProfileCompleteExpression()),
                "pending" => query.Where(IsUserProfileIncompleteExpression()),
                _ => query
            };
        }

        if (!string.IsNullOrWhiteSpace(TicketStatus))
        {
            query = TicketStatus switch
            {
                "active" => query.Where(item => _db.EventTickets.Any(ticket => ticket.UserId == item.Id && !ticket.IsRefunded && !ticket.IsRemoved)),
                "refunded" => query.Where(item => _db.EventTickets.Any(ticket => ticket.UserId == item.Id && ticket.IsRefunded && !ticket.IsRemoved)),
                "removed" => query.Where(item => _db.EventTickets.Any(ticket => ticket.UserId == item.Id && ticket.IsRemoved)),
                "none" => query.Where(item => !_db.EventTickets.Any(ticket => ticket.UserId == item.Id)),
                _ => query
            };
        }

        if (!string.IsNullOrWhiteSpace(Gender))
        {
            query = Gender switch
            {
                "male" => query.Where(item => item.Profile != null && item.Profile.Gender == Randevoo.Domain.Enums.Gender.Male),
                "female" => query.Where(item => item.Profile != null && item.Profile.Gender == Randevoo.Domain.Enums.Gender.Female),
                _ => query
            };
        }

        if (FromDate is DateTime fromDate)
            query = query.Where(item => item.CreatedAt >= fromDate.Date);

        if (ToDate is DateTime toDate)
            query = query.Where(item => item.CreatedAt < toDate.Date.AddDays(1));

        var userIds = query.Select(item => item.Id);
        var totalCount = await query.CountAsync(cancellationToken);
        var summary = new ParticipantListSummary
        {
            TotalParticipants = totalCount,
            CompletedProfiles = await query.CountAsync(IsUserProfileCompleteExpression(), cancellationToken),
            PendingProfiles = await query.CountAsync(IsUserProfileIncompleteExpression(), cancellationToken),
            TotalOrders = await _db.EventTickets
                .AsNoTracking()
                .Where(item => userIds.Contains(item.UserId))
                .Select(item => item.TicketOrderId)
                .Distinct()
                .CountAsync(cancellationToken),
            TotalBuyers = await _db.EventTickets
                .AsNoTracking()
                .Where(item => userIds.Contains(item.UserId))
                .Select(item => item.UserId)
                .Distinct()
                .CountAsync(cancellationToken),
            ActiveTickets = await _db.EventTickets
                .AsNoTracking()
                .CountAsync(item => userIds.Contains(item.UserId) && !item.IsRefunded && !item.IsRemoved, cancellationToken),
            CancelledTickets = await _db.EventTickets
                .AsNoTracking()
                .CountAsync(item => userIds.Contains(item.UserId) && (item.IsRefunded || item.IsRemoved), cancellationToken),
            AvailableCapacity = FilterEventId.HasValue ? await CalculateAvailableCapacityAsync(cancellationToken) : null
        };

        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
        PageNumber = Math.Min(PageNumber, totalPages);

        var lastActivityQuery = _db.AuditLogs
            .AsNoTracking()
            .Where(log => log.ActorUserId != null)
            .GroupBy(log => log.ActorUserId!.Value)
            .Select(group => new
            {
                UserId = group.Key,
                LastActivityAtUtc = group.Max(item => item.CreatedAt)
            });

        var projectedQuery =
            from user in query
            join lastActivity in lastActivityQuery on user.Id equals lastActivity.UserId into lastActivityGroup
            from lastActivity in lastActivityGroup.DefaultIfEmpty()
            select new
            {
                User = user,
                LastActivityAtUtc = lastActivity != null ? lastActivity.LastActivityAtUtc : (DateTime?)null
            };

        projectedQuery = Sort switch
        {
            "name" => projectedQuery.OrderBy(item => item.User.Profile == null ? item.User.MobileNumber : item.User.Profile.DisplayName),
            "registration-asc" => projectedQuery.OrderBy(item => item.User.CreatedAt).ThenBy(item => item.User.Id),
            "tickets-desc" => projectedQuery.OrderByDescending(item => _db.EventTickets.Count(ticket => ticket.UserId == item.User.Id)).ThenByDescending(item => item.User.CreatedAt),
            "events-desc" => projectedQuery.OrderByDescending(item => _db.EventTickets.Where(ticket => ticket.UserId == item.User.Id).Select(ticket => ticket.DatingEventId).Distinct().Count()).ThenByDescending(item => item.User.CreatedAt),
            "profile-status" => projectedQuery.OrderBy(item => item.User.Profile == null
                || item.User.Profile.EducationLevel == EducationLevel.NotSpecified
                || item.User.Profile.Images.Count == 0
                || item.User.Profile.Interests.Count == 0
                || item.User.Profile.CityId == null
                || item.User.Profile.CountryId == null).ThenByDescending(item => item.User.CreatedAt),
            "activity-asc" => projectedQuery.OrderBy(item => item.LastActivityAtUtc).ThenBy(item => item.User.CreatedAt),
            "registration-desc" => projectedQuery.OrderByDescending(item => item.User.CreatedAt).ThenByDescending(item => item.User.Id),
            _ => projectedQuery.OrderByDescending(item => item.LastActivityAtUtc ?? item.User.CreatedAt).ThenByDescending(item => item.User.CreatedAt)
        };

        var rows = await projectedQuery
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .Select(item => new
            {
                UserId = item.User.Id,
                ParticipantName = item.User.Profile != null ? item.User.Profile.DisplayName : item.User.MobileNumber,
                ParticipantMobile = CanSeeContactInfo ? item.User.MobileNumber : null,
                ProfileImageUrl = item.User.Profile == null
                    ? null
                    : item.User.Profile.Images
                        .OrderByDescending(image => image.IsPrimary)
                        .ThenBy(image => image.DisplayOrder)
                        .Select(image => image.ImageUrl)
                        .FirstOrDefault(),
                GenderLookupTitle = item.User.Profile != null && item.User.Profile.GenderLookup != null ? item.User.Profile.GenderLookup.Title : null,
                Gender = item.User.Profile != null ? item.User.Profile.Gender : Randevoo.Domain.Enums.Gender.Unknown,
                BirthDate = item.User.Profile != null ? item.User.Profile.DateOfBirth : (DateOnly?)null,
                EducationLevelLookupTitle = item.User.Profile != null && item.User.Profile.EducationLevelLookup != null ? item.User.Profile.EducationLevelLookup.Title : null,
                EducationLevel = item.User.Profile != null ? item.User.Profile.EducationLevel : EducationLevel.NotSpecified,
                CityName = item.User.Profile != null && item.User.Profile.City != null ? item.User.Profile.City.Name : null,
                LocationCity = item.User.Profile != null ? item.User.Profile.Location.City : null,
                HasProfile = item.User.Profile != null,
                IsProfileComplete = item.User.Profile != null
                    && item.User.Profile.EducationLevel != EducationLevel.NotSpecified
                    && item.User.Profile.Images.Count > 0
                    && item.User.Profile.Interests.Count > 0
                    && item.User.Profile.CityId != null
                    && item.User.Profile.CountryId != null,
                item.User.IsActive,
                CreatedAtUtc = item.User.CreatedAt,
                item.LastActivityAtUtc,
                TicketCount = _db.EventTickets.Count(ticket => ticket.UserId == item.User.Id),
                ActiveTicketCount = _db.EventTickets.Count(ticket => ticket.UserId == item.User.Id && !ticket.IsRefunded && !ticket.IsRemoved),
                CancelledTicketCount = _db.EventTickets.Count(ticket => ticket.UserId == item.User.Id && (ticket.IsRefunded || ticket.IsRemoved)),
                EventCount = _db.EventTickets.Where(ticket => ticket.UserId == item.User.Id).Select(ticket => ticket.DatingEventId).Distinct().Count(),
                SupportTicketCount = _db.SupportTickets.Count(ticket => ticket.SubmitterUserId == item.User.Id),
                LastTicketAtUtc = _db.EventTickets
                    .Where(ticket => ticket.UserId == item.User.Id)
                    .OrderByDescending(ticket => ticket.CreatedAt)
                    .Select(ticket => (DateTime?)ticket.CreatedAt)
                    .FirstOrDefault(),
                LastTicketId = _db.EventTickets
                    .Where(ticket => ticket.UserId == item.User.Id)
                    .OrderByDescending(ticket => ticket.CreatedAt)
                    .Select(ticket => (long?)ticket.Id)
                    .FirstOrDefault(),
                LastTicketOrderId = _db.EventTickets
                    .Where(ticket => ticket.UserId == item.User.Id)
                    .OrderByDescending(ticket => ticket.CreatedAt)
                    .Select(ticket => ticket.TicketOrderId)
                    .FirstOrDefault(),
                LastEventId = _db.EventTickets
                    .Where(ticket => ticket.UserId == item.User.Id)
                    .OrderByDescending(ticket => ticket.CreatedAt)
                    .Select(ticket => (long?)ticket.DatingEventId)
                    .FirstOrDefault(),
                LastEventTitle = _db.EventTickets
                    .Where(ticket => ticket.UserId == item.User.Id)
                    .OrderByDescending(ticket => ticket.CreatedAt)
                    .Select(ticket => $"#{ticket.DatingEvent.EventCode} - {ticket.DatingEvent.Title}")
                    .FirstOrDefault(),
                LastPlannerUserId = _db.EventTickets
                    .Where(ticket => ticket.UserId == item.User.Id)
                    .OrderByDescending(ticket => ticket.CreatedAt)
                    .Select(ticket => (long?)ticket.DatingEvent.EventPlannerUserId)
                    .FirstOrDefault(),
                LastPlannerName = _db.EventTickets
                    .Where(ticket => ticket.UserId == item.User.Id)
                    .OrderByDescending(ticket => ticket.CreatedAt)
                    .Select(ticket => ticket.DatingEvent.EventPlannerUser.Profile != null
                        ? ticket.DatingEvent.EventPlannerUser.Profile.DisplayName
                        : ticket.DatingEvent.EventPlannerUser.MobileNumber)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        return new ParticipantListResult
        {
            TotalCount = totalCount,
            Summary = summary,
            Items = rows.Select(item => new ParticipantListItem
            {
                IsDirectoryRow = true,
                TicketId = item.LastTicketId ?? 0,
                TicketOrderId = item.LastTicketOrderId,
                EventId = item.LastEventId,
                EventTitle = item.LastEventTitle ?? "بدون رویداد",
                EventPlannerUserId = item.LastPlannerUserId,
                EventPlannerName = item.LastPlannerName ?? "ثبت نشده",
                ParticipantUserId = item.UserId,
                ParticipantName = item.ParticipantName,
                ParticipantMobile = item.ParticipantMobile,
                ProfileImageUrl = item.ProfileImageUrl,
                GenderTitle = item.GenderLookupTitle ?? DisplayFormatter.Gender(item.Gender),
                Age = item.BirthDate.HasValue ? CalculateAge(item.BirthDate.Value) : 0,
                EducationLevelTitle = item.EducationLevelLookupTitle ?? item.EducationLevel.ToString(),
                CityTitle = item.CityName ?? (string.IsNullOrWhiteSpace(item.LocationCity) ? "ثبت نشده" : item.LocationCity),
                HasProfile = item.HasProfile,
                IsProfileComplete = item.IsProfileComplete,
                IsActive = item.IsActive,
                TicketCount = item.TicketCount,
                ActiveTicketCount = item.ActiveTicketCount,
                CancelledTicketCount = item.CancelledTicketCount,
                EventCount = item.EventCount,
                SupportTicketCount = item.SupportTicketCount,
                CreatedAtUtc = DateTime.SpecifyKind(item.CreatedAtUtc, DateTimeKind.Utc),
                LastActivityAtUtc = item.LastActivityAtUtc.HasValue ? DateTime.SpecifyKind(item.LastActivityAtUtc.Value, DateTimeKind.Utc) : null,
                LastTicketAtUtc = item.LastTicketAtUtc.HasValue ? DateTime.SpecifyKind(item.LastTicketAtUtc.Value, DateTimeKind.Utc) : null,
                TicketStatus = item.IsActive ? "فعال" : "غیرفعال",
                PurchasedAtUtc = item.LastTicketAtUtc.HasValue ? DateTime.SpecifyKind(item.LastTicketAtUtc.Value, DateTimeKind.Utc) : DateTime.SpecifyKind(item.CreatedAtUtc, DateTimeKind.Utc)
            }).ToList()
        };
    }

    private async Task<ParticipantListResult> QueryTicketParticipantsAsync(CancellationToken cancellationToken)
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
                || EF.Functions.Like(item.User.Profile!.DisplayName, like)
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
                TotalBuyers = group.Select(item => item.TicketOrder.BuyerUserId).Distinct().Count(),
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

        summary.AvailableCapacity = await CalculateAvailableCapacityAsync(cancellationToken);

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
                BirthDate = item.User.Profile != null ? item.User.Profile.DateOfBirth : (DateOnly?)null,
                EducationLevelLookupTitle = item.User.Profile != null && item.User.Profile.EducationLevelLookup != null ? item.User.Profile.EducationLevelLookup.Title : null,
                EducationLevel = item.User.Profile != null ? item.User.Profile.EducationLevel : EducationLevel.NotSpecified,
                CityName = item.User.Profile != null && item.User.Profile.City != null ? item.User.Profile.City.Name : null,
                HasProfile = item.User.Profile != null,
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
                Age = item.BirthDate.HasValue ? CalculateAge(item.BirthDate.Value) : 0,
                EducationLevelTitle = item.EducationLevelLookupTitle ?? item.EducationLevel.ToString(),
                CityTitle = item.CityName ?? "ثبت نشده",
                HasProfile = item.HasProfile,
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
                CreatedAtUtc = DateTime.SpecifyKind(item.CreatedAt, DateTimeKind.Utc),
                RemovalReason = item.RemovalReason,
                BuyerUserId = item.BuyerUserId,
                BuyerName = item.BuyerName,
                BuyerMobile = item.BuyerMobile
            }).ToList()
        };
    }

    private async Task<int?> CalculateAvailableCapacityAsync(CancellationToken cancellationToken)
    {
        var scopedEventId = EventId ?? FilterEventId;
        if (!scopedEventId.HasValue)
            return null;

        var eventCapacity = await _db.DatingEvents
            .AsNoTracking()
            .Where(item => item.Id == scopedEventId.Value)
            .Select(item => new
            {
                TotalCapacity = item.MaleCapacity + item.FemaleCapacity,
                ActiveTickets = item.Tickets.Count(ticket => !ticket.IsRefunded && !ticket.IsRemoved)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return eventCapacity is null
            ? null
            : Math.Max(0, eventCapacity.TotalCapacity - eventCapacity.ActiveTickets);
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
            [nameof(CityId)] = CityId,
            [nameof(ZodiacSignId)] = ZodiacSignId,
            [nameof(IsActive)] = IsActive,
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

    private static int CalculateAge(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - birthDate.Year;
        if (birthDate > today.AddYears(-age))
            age--;

        return age;
    }

    private static long? MapGenderFilterId(string? gender) => gender switch
    {
        "male" => 2,
        "female" => 3,
        _ => null
    };

    private static System.Linq.Expressions.Expression<Func<User, bool>> IsUserProfileCompleteExpression()
        => item => item.Profile != null
            && item.Profile.EducationLevel != EducationLevel.NotSpecified
            && item.Profile.Images.Count > 0
            && item.Profile.Interests.Count > 0
            && item.Profile.CityId != null
            && item.Profile.CountryId != null;

    private static System.Linq.Expressions.Expression<Func<User, bool>> IsUserProfileIncompleteExpression()
        => item => item.Profile == null
            || item.Profile.EducationLevel == EducationLevel.NotSpecified
            || item.Profile.Images.Count == 0
            || item.Profile.Interests.Count == 0
            || item.Profile.CityId == null
            || item.Profile.CountryId == null;
}
