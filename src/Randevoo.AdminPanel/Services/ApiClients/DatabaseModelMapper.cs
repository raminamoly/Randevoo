using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using AdminEventOperationalStatus = Randevoo.AdminPanel.Models.Events.EventOperationalStatus;
using AdminEventReviewStatus = Randevoo.AdminPanel.Models.Events.EventReviewStatus;
using DomainEventOperationalStatus = Randevoo.Domain.Enums.EventOperationalStatus;
using DomainEventReviewStatus = Randevoo.Domain.Enums.EventReviewStatus;

namespace Randevoo.AdminPanel.Services.ApiClients;

internal static class DatabaseModelMapper
{
    private static readonly Regex HtmlTagRegex = new("<.*?>", RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly IReadOnlyDictionary<string, string> AuditFieldTitles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["Title"] = "عنوان",
        ["EventPlannerUserId"] = "برگزارکننده",
        ["EventTypeId"] = "نوع رویداد",
        ["EventModeId"] = "نحوه برگزاری",
        ["EventModeName"] = "نحوه برگزاری",
        ["OnlineEventPlatformId"] = "پلتفرم آنلاین",
        ["OnlinePlatformName"] = "پلتفرم آنلاین",
        ["OnlineJoinUrl"] = "لینک ورود آنلاین",
        ["OnlineAccessInstructions"] = "دستورالعمل ورود آنلاین",
        ["CountryName"] = "کشور",
        ["CityName"] = "شهر",
        ["Region"] = "منطقه",
        ["Address"] = "محل و آدرس",
        ["DateTimeStart"] = "زمان شروع",
        ["DateTimeEnd"] = "زمان پایان",
        ["EventPlannerCommissionPercent"] = "درصد کمیسیون",
        ["PaymentCollectionMethod"] = "روش دریافت هزینه",
        ["OrganizerPaymentInstructions"] = "توضیحات پرداخت برگزارکننده",
        ["MaleCapacity"] = "ظرفیت آقایان",
        ["FemaleCapacity"] = "ظرفیت بانوان",
        ["NumberOfLikesAllowed"] = "تعداد لایک مجاز",
        ["MaleTicketPrice"] = "قیمت بلیت آقایان",
        ["MaleTicketCurrencyCode"] = "واحد پول بلیت آقایان",
        ["FemaleTicketPrice"] = "قیمت بلیت بانوان",
        ["FemaleTicketCurrencyCode"] = "واحد پول بلیت بانوان",
        ["CurrencyCode"] = "واحد پول",
        ["EducationLevelRestriction"] = "محدودیت تحصیلات",
        ["Tags"] = "تگ‌ها",
        ["EventDescriptionHtml"] = "توضیحات رویداد",
        ["EventImage1"] = "تصویر اول",
        ["EventImage2"] = "تصویر دوم",
        ["EventImage3"] = "تصویر سوم",
        ["Faqs"] = "سوالات متداول",
        ["IsOpenForSell"] = "وضعیت فروش",
        ["IsCancelled"] = "لغو رویداد",
        ["ReviewStatus"] = "وضعیت بررسی",
        ["OperationalStatus"] = "وضعیت عملیاتی"
    };

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
            MobileNumber = profile.User.MobileNumber,
            Title = profile.Title,
            PictureUrl = profile.PictureUrl,
            Resume = profile.Resume,
            City = profile.User.Profile?.City?.Name ?? string.Empty,
            SettlementCurrencyCode = profile.SettlementCurrencyCode,
            IsSettlementCurrencyLocked = profile.IsSettlementCurrencyLocked,
            SettlementCurrencyLockedAtUtc = profile.SettlementCurrencyLockedAtUtc,
            SettlementCurrencyLockReason = profile.SettlementCurrencyLockReason,
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
            EventModeId = datingEvent.EventModeId,
            EventModeName = datingEvent.EventMode?.Name ?? "حضوری",
            OnlineEventPlatformId = datingEvent.OnlineEventPlatformId,
            OnlinePlatformName = datingEvent.OnlineEventPlatform?.Name,
            OnlineJoinUrl = datingEvent.OnlineJoinUrl,
            OnlineAccessInstructions = datingEvent.OnlineAccessInstructions,
            AgeRangeForMale = $"{datingEvent.AgeRangeForMale.Min}-{datingEvent.AgeRangeForMale.Max}",
            AgeRangeForFemale = $"{datingEvent.AgeRangeForFemale.Min}-{datingEvent.AgeRangeForFemale.Max}",
            IsOpenForSell = datingEvent.IsOpenForSell,
            MaleTicketPrice = datingEvent.MaleTicketPrice,
            MaleTicketCurrencyCode = datingEvent.CurrencyCode,
            FemaleTicketPrice = datingEvent.FemaleTicketPrice,
            FemaleTicketCurrencyCode = datingEvent.CurrencyCode,
            EducationLevelRestriction = datingEvent.EducationLevelRestriction,
            MinimumEducationLevelId = datingEvent.MinimumEducationLevelId,
            OrganizerCommissionPercent = datingEvent.EventPlannerCommissionPercent,
            PaymentCollectionMethod = datingEvent.PaymentCollectionMethod,
            OrganizerPaymentInstructions = datingEvent.OrganizerPaymentInstructions,
            OrganizerPaymentAccountId = datingEvent.OrganizerPaymentAccountId,
            CapacityMale = datingEvent.MaleCapacity,
            CapacityFemale = datingEvent.FemaleCapacity,
            LikeLimit = datingEvent.NumberOfLikesAllowed,
            Tags = datingEvent.Tags.ToList(),
            TagIds = datingEvent.EventTags.Select(item => item.TagId).ToList(),
            DescriptionHtml = datingEvent.EventDescriptionHtml,
            Image1 = datingEvent.EventImage1,
            Image2 = datingEvent.EventImage2,
            Image3 = datingEvent.EventImage3,
            Faqs = datingEvent.Faqs
                .OrderBy(item => item.DisplayOrder)
                .Select(item => new EventFaqInput
                {
                    Question = item.Question,
                    Answer = item.Answer
                })
                .ToList(),
            StartAtUtc = DateTime.SpecifyKind(datingEvent.DateTimeStart, DateTimeKind.Utc),
            EndAtUtc = DateTime.SpecifyKind(datingEvent.DateTimeEnd, DateTimeKind.Utc)
        };
    }

    public static Randevoo.AdminPanel.Models.Events.DatingEvent ToAdminDatingEvent(Randevoo.Domain.Entities.DatingEvent datingEvent)
    {
        return new Randevoo.AdminPanel.Models.Events.DatingEvent
        {
            Id = datingEvent.Id,
            EventCode = datingEvent.EventCode,
            PlannerUserId = datingEvent.EventPlannerUserId,
            PlannerName = ResolveUserDisplayName(datingEvent.EventPlannerUser),
            Live = ToEventDraftInput(datingEvent),
            OperationalStatus = ToEventOperationalStatus(datingEvent),
            ReviewStatus = ToEventReviewStatus(datingEvent.ReviewStatus),
            ApprovalStatus = datingEvent.ApprovalStatus,
            SaleStatus = datingEvent.SaleStatus,
            LifecycleStatus = datingEvent.LifecycleStatus,
            AdminReviewNote = datingEvent.AdminReviewNote,
            ReviewedByName = datingEvent.ApprovedByUser is null ? null : ResolveUserDisplayName(datingEvent.ApprovedByUser),
            ReviewedAtUtc = datingEvent.ApprovedAtUtc is null ? null : DateTime.SpecifyKind(datingEvent.ApprovedAtUtc.Value, DateTimeKind.Utc),
            CreatedAtUtc = DateTime.SpecifyKind(datingEvent.CreatedAt, DateTimeKind.Utc),
            UpdatedAtUtc = DateTime.SpecifyKind(datingEvent.UpdatedAt ?? datingEvent.CreatedAt, DateTimeKind.Utc),
            IsVisibleToEndUsers = datingEvent.ResolveOperationalStatus(DateTime.UtcNow) == DomainEventOperationalStatus.SaleOpen
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
        UserRole.PlatformSupportTeam => AdminRole.SupportTeam,
        UserRole.EventPlanner => AdminRole.EventPlanner,
        _ => AdminRole.EventPlanner
    };

    public static UserRole ToDomainRole(AdminRole role) => role switch
    {
        AdminRole.Admin => UserRole.Admin,
        AdminRole.SupportTeam => UserRole.PlatformSupportTeam,
        _ => UserRole.EventPlanner
    };

    public static AdminEventOperationalStatus ToEventOperationalStatus(Randevoo.Domain.Entities.DatingEvent datingEvent) =>
        datingEvent.ResolveOperationalStatus(DateTime.UtcNow) switch
        {
            DomainEventOperationalStatus.SaleOpen => AdminEventOperationalStatus.SaleOpen,
            DomainEventOperationalStatus.Completed => AdminEventOperationalStatus.Completed,
            DomainEventOperationalStatus.Cancelled => AdminEventOperationalStatus.Cancelled,
            _ => AdminEventOperationalStatus.SaleClosed
        };

    public static AdminEventReviewStatus ToEventReviewStatus(DomainEventReviewStatus status) => status switch
    {
        DomainEventReviewStatus.PendingReview => AdminEventReviewStatus.PendingReview,
        DomainEventReviewStatus.Approved => AdminEventReviewStatus.Approved,
        DomainEventReviewStatus.Rejected => AdminEventReviewStatus.Rejected,
        _ => AdminEventReviewStatus.NotSubmitted
    };

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

        if (string.Equals(normalizedVenue, normalizedAddress, StringComparison.Ordinal))
            return normalizedAddress;

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
        if (parts.Length == 2 && string.Equals(parts[0], parts[1], StringComparison.Ordinal))
            return (string.Empty, parts[1]);

        return parts.Length == 2
            ? (parts[0], parts[1])
            : (string.Empty, parts[0]);
    }

    private static string ToChangeCategory(string action)
    {
        if (action.Contains("پیام", StringComparison.OrdinalIgnoreCase))
            return "communication";
        if (action.StartsWith("EventReview", StringComparison.OrdinalIgnoreCase)
            || action.Equals("EventSubmittedForReview", StringComparison.OrdinalIgnoreCase))
            return "review";
        if (action.StartsWith("EventSale", StringComparison.OrdinalIgnoreCase))
            return "sale";
        if (action.Equals("EventCancelled", StringComparison.OrdinalIgnoreCase))
            return "lifecycle";
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
        if (log.Action.Equals("EventReviewSubmitted", StringComparison.OrdinalIgnoreCase)
            || log.Action.Equals("EventSubmittedForReview", StringComparison.OrdinalIgnoreCase))
            return null;

        if (!string.IsNullOrWhiteSpace(log.AfterJson) && !string.IsNullOrWhiteSpace(log.BeforeJson))
        {
            var diff = BuildReadableAuditDiff(log.BeforeJson, log.AfterJson);
            if (!string.IsNullOrWhiteSpace(diff))
                return diff;

            return null;
        }

        return !string.IsNullOrWhiteSpace(log.AfterJson)
            ? BuildReadableAuditSnapshot(log.AfterJson)
            : BuildReadableAuditSnapshot(log.BeforeJson);
    }

    private static string? BuildReadableAuditDiff(string? beforeJson, string? afterJson)
    {
        if (!TryParseJsonObject(beforeJson, out var before) || !TryParseJsonObject(afterJson, out var after))
            return BuildRawAuditFallback(beforeJson, afterJson);

        var fieldNames = before.RootElement.EnumerateObject().Select(item => item.Name)
            .Union(after.RootElement.EnumerateObject().Select(item => item.Name), StringComparer.OrdinalIgnoreCase)
            .Where(name => AuditFieldTitles.ContainsKey(name))
            .ToList();

        var changes = new List<string>();
        foreach (var fieldName in fieldNames)
        {
            before.RootElement.TryGetProperty(fieldName, out var beforeValue);
            after.RootElement.TryGetProperty(fieldName, out var afterValue);

            if (AreAuditValuesEquivalent(fieldName, beforeValue, afterValue))
                continue;

            changes.Add($"{AuditFieldTitles[fieldName]}: {FormatAuditValue(fieldName, beforeValue)} -> {FormatAuditValue(fieldName, afterValue)}");
        }

        return changes.Count == 0
            ? null
            : string.Join(Environment.NewLine, changes);
    }

    private static string? BuildReadableAuditSnapshot(string? json)
    {
        if (!TryParseJsonObject(json, out var document))
            return string.IsNullOrWhiteSpace(json) ? null : TrimForAudit(json);

        var fields = new[] { "Title", "EventModeName", "CityName", "Region", "Address", "DateTimeStart", "DateTimeEnd", "ReviewStatus", "IsOpenForSell" };
        var lines = fields
            .Where(field => document.RootElement.TryGetProperty(field, out _))
            .Select(field =>
            {
                document.RootElement.TryGetProperty(field, out var value);
                return $"{AuditFieldTitles[field]}: {FormatAuditValue(field, value)}";
            })
            .ToList();

        return lines.Count == 0
            ? null
            : string.Join(Environment.NewLine, lines);
    }

    private static string? BuildRawAuditFallback(string? beforeJson, string? afterJson)
    {
        if (!string.IsNullOrWhiteSpace(beforeJson) && !string.IsNullOrWhiteSpace(afterJson))
            return $"قبل: {TrimForAudit(beforeJson)}{Environment.NewLine}بعد: {TrimForAudit(afterJson)}";

        return TrimForAudit(afterJson ?? beforeJson);
    }

    private static bool TryParseJsonObject(string? json, out JsonDocument document)
    {
        document = null!;
        if (string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            document = JsonDocument.Parse(json);
            return document.RootElement.ValueKind == JsonValueKind.Object;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool AreAuditValuesEquivalent(string fieldName, JsonElement before, JsonElement after)
    {
        if (before.ValueKind == JsonValueKind.Undefined && after.ValueKind == JsonValueKind.Undefined)
            return true;

        if (fieldName.Equals("EventDescriptionHtml", StringComparison.OrdinalIgnoreCase))
            return string.Equals(NormalizeHtmlText(before), NormalizeHtmlText(after), StringComparison.Ordinal);

        if (fieldName.Equals("Address", StringComparison.OrdinalIgnoreCase))
            return string.Equals(NormalizeStoredAddressForAudit(before), NormalizeStoredAddressForAudit(after), StringComparison.Ordinal);

        if (fieldName.Equals("DateTimeStart", StringComparison.OrdinalIgnoreCase)
            || fieldName.Equals("DateTimeEnd", StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(NormalizeDateTimeForAudit(before), NormalizeDateTimeForAudit(after), StringComparison.OrdinalIgnoreCase);
        }

        if (before.ValueKind == JsonValueKind.Number && after.ValueKind == JsonValueKind.Number)
            return string.Equals(before.GetRawText(), after.GetRawText(), StringComparison.OrdinalIgnoreCase);

        return string.Equals(FormatRawAuditValue(before), FormatRawAuditValue(after), StringComparison.Ordinal);
    }

    private static string FormatAuditValue(string fieldName, JsonElement value)
    {
        if (value.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
            return "ندارد";

        if (fieldName.Equals("EventDescriptionHtml", StringComparison.OrdinalIgnoreCase))
            return TrimForAudit(NormalizeHtmlText(value), 140);

        if (fieldName.Equals("Address", StringComparison.OrdinalIgnoreCase))
            return NormalizeStoredAddressForAudit(value);

        if (fieldName.Equals("DateTimeStart", StringComparison.OrdinalIgnoreCase)
            || fieldName.Equals("DateTimeEnd", StringComparison.OrdinalIgnoreCase))
            return NormalizeDateTimeForAudit(value);

        if (fieldName.Equals("ReviewStatus", StringComparison.OrdinalIgnoreCase) && TryGetInt(value, out var reviewStatus))
        {
            return reviewStatus switch
            {
                0 => "ارسال نشده",
                1 => "در انتظار بررسی",
                2 => "تایید شده",
                3 => "رد شده",
                _ => reviewStatus.ToString()
            };
        }

        if (fieldName.Equals("OperationalStatus", StringComparison.OrdinalIgnoreCase) && TryGetInt(value, out var operationalStatus))
        {
            return operationalStatus switch
            {
                0 => "فروش بسته",
                1 => "فروش باز",
                2 => "تمام شده",
                3 => "لغو شده",
                _ => operationalStatus.ToString()
            };
        }

        if (fieldName.Equals("IsOpenForSell", StringComparison.OrdinalIgnoreCase) && value.ValueKind is JsonValueKind.True or JsonValueKind.False)
            return value.GetBoolean() ? "باز" : "بسته";

        if (fieldName.Equals("IsCancelled", StringComparison.OrdinalIgnoreCase) && value.ValueKind is JsonValueKind.True or JsonValueKind.False)
            return value.GetBoolean() ? "بله" : "خیر";

        if (fieldName.Equals("Faqs", StringComparison.OrdinalIgnoreCase) && value.ValueKind == JsonValueKind.Array)
        {
            var count = value.GetArrayLength();
            return count == 0 ? "ندارد" : $"{count} مورد";
        }

        if (value.ValueKind == JsonValueKind.Array)
        {
            var items = value.EnumerateArray()
                .Select(FormatRawAuditValue)
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToList();

            return items.Count == 0 ? "ندارد" : string.Join("، ", items);
        }

        if (value.ValueKind is JsonValueKind.True or JsonValueKind.False)
            return value.GetBoolean() ? "بله" : "خیر";

        return TrimForAudit(FormatRawAuditValue(value), 180);
    }

    private static string FormatRawAuditValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.Undefined or JsonValueKind.Null => string.Empty,
            JsonValueKind.String => value.GetString() ?? string.Empty,
            JsonValueKind.Number => value.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Array => string.Join("، ", value.EnumerateArray().Select(FormatRawAuditValue)),
            _ => value.GetRawText()
        };
    }

    private static string NormalizeHtmlText(JsonElement value)
    {
        if (value.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
            return string.Empty;

        var html = value.ValueKind == JsonValueKind.String ? value.GetString() : value.GetRawText();
        html ??= string.Empty;
        var text = HtmlTagRegex.Replace(html, " ");
        return NormalizeWhitespace(WebUtility.HtmlDecode(text));
    }

    private static string NormalizeStoredAddressForAudit(JsonElement value)
    {
        var storedAddress = value.ValueKind == JsonValueKind.String ? value.GetString() : FormatRawAuditValue(value);
        if (string.IsNullOrWhiteSpace(storedAddress))
            return "ندارد";

        var (venueName, address) = SplitStoredAddress(storedAddress.Trim());
        return string.IsNullOrWhiteSpace(venueName)
            ? address
            : $"{venueName}، {address}";
    }

    private static string NormalizeDateTimeForAudit(JsonElement value)
    {
        var text = value.ValueKind == JsonValueKind.String ? value.GetString() : FormatRawAuditValue(value);
        if (string.IsNullOrWhiteSpace(text))
            return "ندارد";

        text = text.Trim();
        if (DateTime.TryParse(text.TrimEnd('Z'), out var dateTime))
            return dateTime.ToString("yyyy-MM-dd HH:mm");

        return text;
    }

    private static bool TryGetInt(JsonElement value, out int result)
    {
        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out result))
            return true;

        if (value.ValueKind == JsonValueKind.String && int.TryParse(value.GetString(), out result))
            return true;

        result = default;
        return false;
    }

    private static string TrimForAudit(string? value, int maxLength = 1000)
    {
        var normalized = NormalizeWhitespace(value ?? string.Empty);
        if (normalized.Length <= maxLength)
            return normalized;

        return $"{normalized[..maxLength]}...";
    }

    private static string NormalizeWhitespace(string value)
    {
        return Regex.Replace(value, @"\s+", " ").Trim();
    }
}
