using System.Globalization;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Models.Users;

namespace Randevoo.AdminPanel.Services.MockData;

public sealed class AdminPanelStore
{
    private readonly object _gate = new();
    private readonly List<MockUser> _users = new();
    private readonly List<DatingEvent> _events = new();

    public AdminPanelStore()
    {
        Seed();
    }

    public IReadOnlyList<MockUser> GetUsers()
    {
        lock (_gate)
        {
            return _users
                .OrderBy(user => user.Role)
                .ThenBy(user => user.FullName)
                .ToList();
        }
    }

    public MockUser? FindUserByMobile(string mobile)
    {
        lock (_gate)
        {
            return _users.FirstOrDefault(user => string.Equals(user.Mobile, mobile, StringComparison.OrdinalIgnoreCase));
        }
    }

    public MockUser? FindUserById(Guid id)
    {
        lock (_gate)
        {
            return _users.FirstOrDefault(user => user.Id == id);
        }
    }

    public MockUser UpsertUser(UserUpsertInput input, Guid? existingUserId = null)
    {
        lock (_gate)
        {
            var user = existingUserId is Guid id
                ? _users.FirstOrDefault(item => item.Id == id)
                : _users.FirstOrDefault(item => string.Equals(item.Mobile, input.Mobile, StringComparison.OrdinalIgnoreCase));

            if (user is null)
            {
                user = new MockUser();
                _users.Add(user);
            }

            user.FullName = input.FullName.Trim();
            user.Mobile = input.Mobile.Trim();
            user.Role = input.Role;
            user.IsActive = input.IsActive;
            return CloneUser(user);
        }
    }

    public IReadOnlyList<DatingEvent> GetEvents()
    {
        lock (_gate)
        {
            return _events
                .OrderByDescending(@event => @event.UpdatedAtUtc)
                .ThenBy(@event => @event.DisplayTitle)
                .Select(CloneEvent)
                .ToList();
        }
    }

    public DatingEvent? FindEvent(Guid id)
    {
        lock (_gate)
        {
            var @event = _events.FirstOrDefault(item => item.Id == id);
            return @event is null ? null : CloneEvent(@event);
        }
    }

    public DatingEvent UpsertEvent(EventDraftInput input, MockUser actor, Guid? existingEventId = null)
    {
        lock (_gate)
        {
            var @event = existingEventId is Guid id
                ? _events.FirstOrDefault(item => item.Id == id)
                : null;

            if (@event is null)
            {
                @event = new DatingEvent
                {
                    Id = Guid.NewGuid(),
                    PlannerId = actor.Id.ToString(),
                    PlannerName = actor.FullName,
                    CreatedAtUtc = DateTimeOffset.UtcNow
                };
                _events.Add(@event);
            }

            var newDraft = CloneDraft(input);
            if (actor.Role == AdminRole.Admin || actor.Role == AdminRole.SupportTeam)
            {
                @event.Live = newDraft;
                @event.Pending = null;
                @event.Status = input.IsOpenForSell ? EventApprovalState.Approved : EventApprovalState.Draft;
                @event.IsVisibleToEndUsers = input.IsOpenForSell;
                @event.AdminReviewNote = null;
                @event.ReviewedAtUtc = DateTimeOffset.UtcNow;
                @event.ReviewedByName = actor.FullName;
            }
            else
            {
                @event.Pending = new EventDraftState
                {
                    Draft = newDraft,
                    SubmittedByName = actor.FullName,
                    SubmittedAtUtc = DateTimeOffset.UtcNow,
                    ReviewNote = "Waiting for admin approval."
                };
                @event.Status = EventApprovalState.PendingAdminReview;
                @event.IsVisibleToEndUsers = false;
                if (existingEventId is null)
                {
                    @event.Live = new EventDraftInput();
                }
            }

            @event.PlannerId = actor.Id.ToString();
            @event.PlannerName = actor.FullName;
            @event.UpdatedAtUtc = DateTimeOffset.UtcNow;
            if (existingEventId is null && actor.Role != AdminRole.Admin && actor.Role != AdminRole.SupportTeam)
            {
                @event.Status = EventApprovalState.PendingAdminReview;
            }

            return CloneEvent(@event);
        }
    }

    public DatingEvent ApproveEvent(Guid eventId, MockUser admin, decimal? commissionPercent = null, string? note = null)
    {
        lock (_gate)
        {
            var @event = RequireEvent(eventId);
            if (@event.Pending is not null)
            {
                @event.Live = CloneDraft(@event.Pending.Draft);
                @event.Pending = null;
            }

            if (commissionPercent is not null)
            {
                @event.Live.OrganizerCommissionPercent = commissionPercent.Value;
            }

            @event.Status = @event.Live.IsOpenForSell ? EventApprovalState.Approved : EventApprovalState.Draft;
            @event.IsVisibleToEndUsers = @event.Live.IsOpenForSell;
            @event.ReviewedByName = admin.FullName;
            @event.ReviewedAtUtc = DateTimeOffset.UtcNow;
            @event.AdminReviewNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
            @event.UpdatedAtUtc = DateTimeOffset.UtcNow;
            return CloneEvent(@event);
        }
    }

    public DatingEvent RejectEvent(Guid eventId, MockUser admin, string note)
    {
        lock (_gate)
        {
            var @event = RequireEvent(eventId);
            @event.Status = EventApprovalState.Rejected;
            @event.IsVisibleToEndUsers = false;
            @event.ReviewedByName = admin.FullName;
            @event.ReviewedAtUtc = DateTimeOffset.UtcNow;
            @event.AdminReviewNote = note.Trim();
            if (@event.Pending is not null)
            {
                @event.Pending.ReviewNote = note.Trim();
            }
            @event.UpdatedAtUtc = DateTimeOffset.UtcNow;
            return CloneEvent(@event);
        }
    }

    public DatingEvent SetCommission(Guid eventId, MockUser admin, decimal commissionPercent)
    {
        lock (_gate)
        {
            var @event = RequireEvent(eventId);
            @event.Live.OrganizerCommissionPercent = commissionPercent;
            @event.Status = @event.Pending is null && @event.Live.IsOpenForSell
                ? EventApprovalState.Approved
                : @event.Status;
            @event.UpdatedAtUtc = DateTimeOffset.UtcNow;
            @event.ReviewedByName = admin.FullName;
            @event.ReviewedAtUtc = DateTimeOffset.UtcNow;
            return CloneEvent(@event);
        }
    }

    public DatingEvent ToggleSale(Guid eventId, MockUser admin, bool isOpen)
    {
        lock (_gate)
        {
            var @event = RequireEvent(eventId);
            @event.Live.IsOpenForSell = isOpen;
            @event.IsVisibleToEndUsers = isOpen && @event.Pending is null && @event.Status != EventApprovalState.Rejected;
            @event.Status = isOpen ? EventApprovalState.Approved : EventApprovalState.Closed;
            @event.ReviewedByName = admin.FullName;
            @event.ReviewedAtUtc = DateTimeOffset.UtcNow;
            @event.UpdatedAtUtc = DateTimeOffset.UtcNow;
            return CloneEvent(@event);
        }
    }

    public DatingEvent Cancel(Guid eventId, MockUser admin)
    {
        lock (_gate)
        {
            var @event = RequireEvent(eventId);
            @event.Status = EventApprovalState.Cancelled;
            @event.Live.IsOpenForSell = false;
            @event.IsVisibleToEndUsers = false;
            @event.ReviewedByName = admin.FullName;
            @event.ReviewedAtUtc = DateTimeOffset.UtcNow;
            @event.UpdatedAtUtc = DateTimeOffset.UtcNow;
            return CloneEvent(@event);
        }
    }

    public DashboardStats GetDashboardStats(MockUser currentUser)
    {
        lock (_gate)
        {
            var visibleEvents = currentUser.Role == AdminRole.EventPlanner
                ? _events.Where(item => item.PlannerId == currentUser.Id.ToString())
                : _events.AsEnumerable();

            return new DashboardStats
            {
                UsersCount = _users.Count,
                PlannerCount = _users.Count(user => user.Role == AdminRole.EventPlanner),
                MyEventsCount = visibleEvents.Count(),
                PendingEventsCount = visibleEvents.Count(item => item.Status == EventApprovalState.PendingAdminReview),
                LiveEventsCount = visibleEvents.Count(item => item.Status == EventApprovalState.Approved && item.Live.IsOpenForSell),
                ClosedEventsCount = visibleEvents.Count(item => item.Status is EventApprovalState.Closed or EventApprovalState.Cancelled),
                TotalTicketSales = visibleEvents.Sum(item => item.Live.TicketPrice * Math.Max(item.Live.CapacityMale + item.Live.CapacityFemale, 1)),
                PendingRevenue = visibleEvents.Where(item => item.Status == EventApprovalState.PendingAdminReview)
                    .Sum(item => item.Live.TicketPrice)
            };
        }
    }

    private DatingEvent RequireEvent(Guid eventId)
    {
        var @event = _events.FirstOrDefault(item => item.Id == eventId);
        if (@event is null)
        {
            throw new InvalidOperationException($"Event '{eventId}' was not found.");
        }

        return @event;
    }

    private static MockUser CloneUser(MockUser user) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        Mobile = user.Mobile,
        Role = user.Role,
        IsActive = user.IsActive
    };

    private static EventDraftInput CloneDraft(EventDraftInput draft) => new()
    {
        Title = draft.Title,
        Country = draft.Country,
        City = draft.City,
        Region = draft.Region,
        VenueName = draft.VenueName,
        Address = draft.Address,
        Latitude = draft.Latitude,
        Longitude = draft.Longitude,
        EventType = draft.EventType,
        AgeRangeForMale = draft.AgeRangeForMale,
        AgeRangeForFemale = draft.AgeRangeForFemale,
        IsOpenForSell = draft.IsOpenForSell,
        TicketPrice = draft.TicketPrice,
        OrganizerCommissionPercent = draft.OrganizerCommissionPercent,
        CapacityMale = draft.CapacityMale,
        CapacityFemale = draft.CapacityFemale,
        ChatLimit = draft.ChatLimit,
        DescriptionHtml = draft.DescriptionHtml,
        Image1 = draft.Image1,
        Image2 = draft.Image2,
        Image3 = draft.Image3,
        StartAtUtc = draft.StartAtUtc,
        EndAtUtc = draft.EndAtUtc
    };

    private static DatingEvent CloneEvent(DatingEvent source) => new()
    {
        Id = source.Id,
        PlannerId = source.PlannerId,
        PlannerName = source.PlannerName,
        Live = CloneDraft(source.Live),
        Pending = source.Pending is null ? null : new EventDraftState
        {
            Draft = CloneDraft(source.Pending.Draft),
            SubmittedByName = source.Pending.SubmittedByName,
            SubmittedAtUtc = source.Pending.SubmittedAtUtc,
            ReviewNote = source.Pending.ReviewNote
        },
        Status = source.Status,
        AdminReviewNote = source.AdminReviewNote,
        ReviewedByName = source.ReviewedByName,
        ReviewedAtUtc = source.ReviewedAtUtc,
        CreatedAtUtc = source.CreatedAtUtc,
        UpdatedAtUtc = source.UpdatedAtUtc,
        IsVisibleToEndUsers = source.IsVisibleToEndUsers
    };

    private void Seed()
    {
        _users.AddRange(new[]
        {
            new MockUser
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                FullName = "System Admin",
                Mobile = "09125177721",
                Role = AdminRole.Admin,
                IsActive = true
            },
            new MockUser
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                FullName = "Tehran Planner",
                Mobile = "09125550000",
                Role = AdminRole.EventPlanner,
                IsActive = true
            },
            new MockUser
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                FullName = "Support Team",
                Mobile = "09126660000",
                Role = AdminRole.SupportTeam,
                IsActive = true
            }
        });

        _events.Add(new DatingEvent
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            PlannerId = "22222222-2222-2222-2222-222222222222",
            PlannerName = "Tehran Planner",
            Live = new EventDraftInput
            {
                Title = "Tehran Social Evening",
                Country = "Iran",
                City = "Tehran",
                Region = "Valiasr",
                VenueName = "North Hall",
                Address = "Valiasr St, Tehran",
                Latitude = 35.7219m,
                Longitude = 51.3347m,
                EventType = EventType.SocialEvening,
                AgeRangeForMale = "25-35",
                AgeRangeForFemale = "25-35",
                IsOpenForSell = true,
                TicketPrice = 1800000m,
                OrganizerCommissionPercent = 12m,
                CapacityMale = 40,
                CapacityFemale = 40,
                ChatLimit = 180,
                DescriptionHtml = "<p>A curated evening for selected participants with admin-reviewed access.</p>",
                Image1 = BuildPlaceholderSvg("Tehran Evening", "#1d4ed8", "#ff4d7d"),
                Image2 = BuildPlaceholderSvg("North Hall", "#0f172a", "#1d4ed8"),
                Image3 = BuildPlaceholderSvg("Curated Guest List", "#ff4d7d", "#f59e0b"),
                StartAtUtc = DateTimeOffset.UtcNow.AddDays(8),
                EndAtUtc = DateTimeOffset.UtcNow.AddDays(8).AddHours(3)
            },
            Status = EventApprovalState.Approved,
            IsVisibleToEndUsers = true,
            CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-5),
            UpdatedAtUtc = DateTimeOffset.UtcNow.AddDays(-1)
        });

        _events.Add(new DatingEvent
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            PlannerId = "22222222-2222-2222-2222-222222222222",
            PlannerName = "Tehran Planner",
            Live = new EventDraftInput
            {
                Title = "Rooftop Dinner Preview",
                Country = "Iran",
                City = "Tehran",
                Region = "Pardis",
                VenueName = "Blue Roof",
                Address = "Pardis, Tehran",
                Latitude = 35.72m,
                Longitude = 51.33m,
                EventType = EventType.Dinner,
                AgeRangeForMale = "20-30",
                AgeRangeForFemale = "20-30",
                IsOpenForSell = false,
                TicketPrice = 2400000m,
                OrganizerCommissionPercent = 15m,
                CapacityMale = 30,
                CapacityFemale = 30,
                ChatLimit = 120,
                DescriptionHtml = "<p>Waiting for admin review. This event will open after approval.</p>",
                Image1 = BuildPlaceholderSvg("Rooftop", "#0f172a", "#ff4d7d"),
                Image2 = BuildPlaceholderSvg("Dinner", "#1d4ed8", "#f59e0b"),
                Image3 = BuildPlaceholderSvg("Preview", "#ff4d7d", "#0f172a"),
                StartAtUtc = DateTimeOffset.UtcNow.AddDays(16),
                EndAtUtc = DateTimeOffset.UtcNow.AddDays(16).AddHours(4)
            },
            Pending = new EventDraftState
            {
                Draft = new EventDraftInput
                {
                    Title = "Rooftop Dinner Preview",
                    Country = "Iran",
                    City = "Tehran",
                    Region = "Pardis",
                    VenueName = "Blue Roof",
                    Address = "Pardis, Tehran",
                    Latitude = 35.72m,
                    Longitude = 51.33m,
                    EventType = EventType.Dinner,
                    AgeRangeForMale = "20-30",
                    AgeRangeForFemale = "20-30",
                    IsOpenForSell = false,
                    TicketPrice = 2400000m,
                    OrganizerCommissionPercent = 15m,
                    CapacityMale = 30,
                    CapacityFemale = 30,
                    ChatLimit = 120,
                    DescriptionHtml = "<p>Waiting for admin review. This event will open after approval.</p>",
                    Image1 = BuildPlaceholderSvg("Rooftop", "#0f172a", "#ff4d7d"),
                    Image2 = BuildPlaceholderSvg("Dinner", "#1d4ed8", "#f59e0b"),
                    Image3 = BuildPlaceholderSvg("Preview", "#ff4d7d", "#0f172a"),
                    StartAtUtc = DateTimeOffset.UtcNow.AddDays(16),
                    EndAtUtc = DateTimeOffset.UtcNow.AddDays(16).AddHours(4)
                },
                SubmittedByName = "Tehran Planner",
                SubmittedAtUtc = DateTimeOffset.UtcNow.AddHours(-2),
                ReviewNote = "Waiting for admin approval."
            },
            Status = EventApprovalState.PendingAdminReview,
            IsVisibleToEndUsers = false,
            CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-2),
            UpdatedAtUtc = DateTimeOffset.UtcNow.AddHours(-2)
        });
    }

    private static string BuildPlaceholderSvg(string label, string accentA, string accentB)
    {
        var svg = $"""
        <svg xmlns="http://www.w3.org/2000/svg" width="900" height="600" viewBox="0 0 900 600">
          <defs>
            <linearGradient id="g" x1="0%" y1="0%" x2="100%" y2="100%">
              <stop offset="0%" stop-color="{accentA}" />
              <stop offset="100%" stop-color="{accentB}" />
            </linearGradient>
          </defs>
          <rect width="900" height="600" rx="48" fill="#ffffff"/>
          <rect x="40" y="40" width="820" height="520" rx="36" fill="url(#g)" opacity="0.14"/>
          <circle cx="150" cy="150" r="58" fill="{accentA}" opacity="0.9"/>
          <circle cx="750" cy="150" r="58" fill="{accentB}" opacity="0.9"/>
          <rect x="210" y="220" width="480" height="110" rx="55" fill="{accentA}" opacity="0.9"/>
          <rect x="250" y="340" width="400" height="100" rx="50" fill="{accentB}" opacity="0.9"/>
          <text x="450" y="520" text-anchor="middle" font-family="Arial, sans-serif" font-size="42" font-weight="700" fill="#0f172a">{label}</text>
        </svg>
        """;

        return "data:image/svg+xml;base64," + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(svg));
    }
}
