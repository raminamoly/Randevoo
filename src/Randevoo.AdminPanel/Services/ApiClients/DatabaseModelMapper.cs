using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Services.ApiClients;

internal static class DatabaseModelMapper
{
    public static MockUser ToAdminUser(User user)
    {
        return new MockUser
        {
            Id = user.Id,
            FullName = ResolveUserDisplayName(user),
            Mobile = user.MobileNumber,
            Role = ToAdminRole(user.Role),
            IsActive = user.IsActive
        };
    }

    public static PlannerProfileViewModel ToPlannerProfileViewModel(EventPlannerProfile profile, int hostedEventCount, int cancelledEventCount, int completedEventCount)
    {
        return new PlannerProfileViewModel
        {
            UserId = profile.UserId,
            FullName = ResolveUserDisplayName(profile.User),
            Title = profile.Title,
            PictureUrl = profile.PictureUrl,
            Resume = profile.Resume,
            City = profile.User.Profile?.City?.Name ?? string.Empty,
            HasPendingChanges = profile.HasPendingChanges,
            PendingFullName = profile.PendingFullName,
            PendingCity = profile.PendingCity,
            PendingTitle = profile.PendingTitle,
            PendingPictureUrl = profile.PendingPictureUrl,
            PendingResume = profile.PendingResume,
            PendingSubmittedAtUtc = profile.PendingSubmittedAt is null ? null : DateTime.SpecifyKind(profile.PendingSubmittedAt.Value, DateTimeKind.Utc),
            PendingReviewNote = profile.PendingReviewNote,
            PendingReviewedAtUtc = profile.PendingReviewedAt is null ? null : DateTime.SpecifyKind(profile.PendingReviewedAt.Value, DateTimeKind.Utc),
            AverageRating = profile.AverageRating,
            TotalSurveyCount = profile.TotalSurveyCount,
            HostedEventCount = hostedEventCount,
            CancelledEventCount = cancelledEventCount,
            CompletedEventCount = completedEventCount
        };
    }

    public static EventDraftInput ToEventDraftInput(Randevoo.Domain.Entities.DatingEvent datingEvent)
    {
        var (venueName, address) = SplitStoredAddress(datingEvent.Address);

        return new EventDraftInput
        {
            Title = datingEvent.Title,
            Country = datingEvent.Country?.Name ?? string.Empty,
            City = datingEvent.City?.Name ?? string.Empty,
            Region = datingEvent.Location.Region ?? string.Empty,
            VenueName = venueName,
            Address = address,
            Latitude = datingEvent.Location.Coordinates.Latitude,
            Longitude = datingEvent.Location.Coordinates.Longitude,
            EventTypeId = datingEvent.EventTypeId,
            EventTypeName = datingEvent.EventType.Name,
            AgeRangeForMale = $"{datingEvent.AgeRangeForMale.Min}-{datingEvent.AgeRangeForMale.Max}",
            AgeRangeForFemale = $"{datingEvent.AgeRangeForFemale.Min}-{datingEvent.AgeRangeForFemale.Max}",
            IsOpenForSell = datingEvent.IsOpenForSell,
            TicketPrice = datingEvent.TicketPrice,
            EducationLevelRestriction = datingEvent.EducationLevelRestriction,
            MinimumEducationLevelId = datingEvent.MinimumEducationLevelId,
            OrganizerCommissionPercent = datingEvent.EventPlannerCommissionPercent,
            CapacityMale = datingEvent.MaleCapacity,
            CapacityFemale = datingEvent.FemaleCapacity,
            ChatLimit = datingEvent.NumberOfChatAllowed,
            Tags = datingEvent.Tags.ToList(),
            TagIds = datingEvent.EventTags.Select(item => item.TagId).ToList(),
            DescriptionHtml = datingEvent.EventDescriptionHtml,
            Image1 = datingEvent.EventImage1,
            Image2 = datingEvent.EventImage2,
            Image3 = datingEvent.EventImage3,
            StartAtUtc = DateTime.SpecifyKind(datingEvent.DateTimeStart, DateTimeKind.Utc),
            EndAtUtc = DateTime.SpecifyKind(datingEvent.DateTimeEnd, DateTimeKind.Utc)
        };
    }

    public static Randevoo.AdminPanel.Models.Events.DatingEvent ToAdminDatingEvent(Randevoo.Domain.Entities.DatingEvent datingEvent)
    {
        return new Randevoo.AdminPanel.Models.Events.DatingEvent
        {
            Id = datingEvent.Id,
            PlannerUserId = datingEvent.EventPlannerUserId,
            PlannerName = ResolveUserDisplayName(datingEvent.EventPlannerUser),
            Live = ToEventDraftInput(datingEvent),
            Status = ToEventApprovalState(datingEvent),
            CreatedAtUtc = DateTime.SpecifyKind(datingEvent.CreatedAt, DateTimeKind.Utc),
            UpdatedAtUtc = DateTime.SpecifyKind(datingEvent.UpdatedAt ?? datingEvent.CreatedAt, DateTimeKind.Utc),
            IsVisibleToEndUsers = datingEvent.IsOpenForSell && !datingEvent.IsCancelled
        };
    }

    public static EventSmsRequest ToEventSmsRequest(EventParticipantSmsRequest request)
    {
        return new EventSmsRequest
        {
            Id = request.Id,
            Message = request.Message,
            ApprovedMessage = request.ApprovedMessage,
            RequestedByName = ResolveUserDisplayName(request.RequestedByUser),
            RequestedAtUtc = DateTime.SpecifyKind(request.CreatedAt, DateTimeKind.Utc),
            PlannedSendAtUtc = request.PlannedSendAtUtc is null ? null : DateTime.SpecifyKind(request.PlannedSendAtUtc.Value, DateTimeKind.Utc),
            Status = request.Status switch
            {
                EventParticipantSmsRequestStatus.Approved => EventSmsRequestStatus.Approved,
                EventParticipantSmsRequestStatus.Rejected => EventSmsRequestStatus.Rejected,
                _ => EventSmsRequestStatus.Pending
            },
            ReviewNote = request.ReviewNote,
            ReviewedByName = request.ReviewedByAdminUser is null ? null : ResolveUserDisplayName(request.ReviewedByAdminUser),
            ReviewedAtUtc = request.ReviewedAt is null ? null : DateTime.SpecifyKind(request.ReviewedAt.Value, DateTimeKind.Utc),
            QueuedRecipientsCount = request.QueuedRecipientsCount
        };
    }

    public static EventChangeLogEntry ToEventChangeLogEntry(AuditLog log, string actorName)
    {
        return new EventChangeLogEntry
        {
            Id = log.Id,
            Category = ToChangeCategory(log.Action),
            Action = log.Action,
            ActorName = actorName,
            Summary = string.IsNullOrWhiteSpace(log.Reason) ? log.Action : log.Reason!,
            Details = BuildAuditDetails(log),
            CreatedAtUtc = DateTime.SpecifyKind(log.CreatedAt, DateTimeKind.Utc)
        };
    }

    public static AdminRole ToAdminRole(UserRole role) => role switch
    {
        UserRole.Admin => AdminRole.Admin,
        UserRole.EventPlanner => AdminRole.EventPlanner,
        _ => AdminRole.EventPlanner
    };

    public static UserRole ToDomainRole(AdminRole role) => role switch
    {
        AdminRole.Admin => UserRole.Admin,
        AdminRole.SupportTeam => UserRole.Admin,
        _ => UserRole.EventPlanner
    };

    public static EventApprovalState ToEventApprovalState(Randevoo.Domain.Entities.DatingEvent datingEvent)
    {
        if (datingEvent.IsCancelled)
            return EventApprovalState.Cancelled;

        if (datingEvent.IsOpenForSell)
            return EventApprovalState.Approved;

        if (datingEvent.DateTimeEnd <= DateTime.UtcNow)
            return EventApprovalState.Closed;

        return EventApprovalState.Draft;
    }

    public static (int Min, int Max) ParseAgeRange(string ageRange)
    {
        var parts = (ageRange ?? string.Empty)
            .Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2
            || !int.TryParse(parts[0], out var min)
            || !int.TryParse(parts[1], out var max))
        {
            throw new InvalidOperationException("بازه سنی وارد شده معتبر نیست.");
        }

        return (min, max);
    }

    public static string ComposeStoredAddress(string venueName, string address)
    {
        var normalizedVenue = string.IsNullOrWhiteSpace(venueName) ? null : venueName.Trim();
        var normalizedAddress = string.IsNullOrWhiteSpace(address) ? string.Empty : address.Trim();

        return string.IsNullOrWhiteSpace(normalizedVenue)
            ? normalizedAddress
            : $"{normalizedVenue} | {normalizedAddress}";
    }

    public static string ResolveUserDisplayName(User user)
    {
        return user.Profile?.DisplayName
            ?? user.PendingEmail
            ?? user.Email
            ?? user.MobileNumber;
    }

    private static (string VenueName, string Address) SplitStoredAddress(string storedAddress)
    {
        if (string.IsNullOrWhiteSpace(storedAddress))
            return (string.Empty, string.Empty);

        var parts = storedAddress.Split('|', 2, StringSplitOptions.TrimEntries);
        return parts.Length == 2
            ? (parts[0], parts[1])
            : (parts[0], parts[0]);
    }

    private static string ToChangeCategory(string action)
    {
        if (action.Contains("پیام", StringComparison.OrdinalIgnoreCase))
            return "communication";
        if (action.Contains("برگزارکننده", StringComparison.OrdinalIgnoreCase))
            return "assignment";
        if (action.Contains("فروش", StringComparison.OrdinalIgnoreCase))
            return "sale";
        if (action.Contains("کمیسیون", StringComparison.OrdinalIgnoreCase))
            return "pricing";
        if (action.Contains("لغو", StringComparison.OrdinalIgnoreCase))
            return "lifecycle";
        if (action.Contains("تایید", StringComparison.OrdinalIgnoreCase) || action.Contains("رد", StringComparison.OrdinalIgnoreCase))
            return "review";

        return "event";
    }

    private static string? BuildAuditDetails(AuditLog log)
    {
        if (!string.IsNullOrWhiteSpace(log.AfterJson) && !string.IsNullOrWhiteSpace(log.BeforeJson))
            return $"قبل: {log.BeforeJson}\nبعد: {log.AfterJson}";

        return !string.IsNullOrWhiteSpace(log.AfterJson)
            ? log.AfterJson
            : log.BeforeJson;
    }
}
