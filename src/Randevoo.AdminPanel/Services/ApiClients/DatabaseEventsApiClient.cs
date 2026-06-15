using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Application.Interfaces.Auditing;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.ValueObjects;
using Randevoo.Infrastructure.Data;
using DomainDatingEvent = Randevoo.Domain.Entities.DatingEvent;

namespace Randevoo.AdminPanel.Services.ApiClients;

public sealed class DatabaseEventsApiClient : IEventsApiClient
{
    private readonly RandevooDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;

    public DatabaseEventsApiClient(RandevooDbContext db, IUnitOfWork unitOfWork, IAuditLogger auditLogger)
    {
        _db = db;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
    }

    public async Task<IReadOnlyList<EventTypeOption>> GetEventTypesAsync(CancellationToken cancellationToken = default)
    {
        return await _db.EventTypes
            .Where(item => item.IsActive)
            .OrderBy(item => item.Name)
            .Select(item => new EventTypeOption
            {
                Id = item.Id,
                Name = item.Name
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EventModeOption>> GetEventModesAsync(CancellationToken cancellationToken = default)
    {
        return await _db.EventModes
            .Where(item => item.IsActive)
            .OrderBy(item => item.DisplayOrder)
            .Select(item => new EventModeOption
            {
                Id = item.Id,
                Name = item.Name,
                IsOnline = item.IsOnline
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OnlineEventPlatformOption>> GetOnlineEventPlatformsAsync(CancellationToken cancellationToken = default)
    {
        return await _db.OnlineEventPlatforms
            .Where(item => item.IsActive)
            .OrderBy(item => item.DisplayOrder)
            .Select(item => new OnlineEventPlatformOption
            {
                Id = item.Id,
                Name = item.Name
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SystemLookupOption>> GetCurrencyOptionsAsync(CancellationToken cancellationToken = default)
    {
        var currencies = await _db.Currencies
            .Where(item => item.IsActive)
            .OrderBy(item => item.DisplayOrder)
            .ThenBy(item => item.Code)
            .Select(item => new SystemLookupOption
            {
                Id = item.Id,
                Name = item.Code,
                DisplayNameFa = item.DisplayNameFa,
                Symbol = item.Symbol,
                DecimalPlaces = item.DecimalPlaces
            })
            .ToListAsync(cancellationToken);

        var codes = currencies.Select(item => item.Name).ToList();
        var activeRates = await _db.CurrencyExchangeRates
            .Where(item => codes.Contains(item.FromCurrencyCode)
                && item.ToCurrencyCode == "IRR"
                && item.EffectiveToUtc == null)
            .ToListAsync(cancellationToken);

        var rateLookup = activeRates
            .GroupBy(item => item.FromCurrencyCode, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.OrderByDescending(item => item.EffectiveFromUtc).First(), StringComparer.OrdinalIgnoreCase);
        foreach (var currency in currencies)
        {
            if (rateLookup.TryGetValue(currency.Name, out var rate))
            {
                currency.ExchangeRateToIrr = rate.Rate;
                currency.ExchangeRateEffectiveFromUtc = DateTime.SpecifyKind(rate.EffectiveFromUtc, DateTimeKind.Utc);
            }
        }

        return currencies;
    }

    public async Task<IReadOnlyList<Models.Events.DatingEvent>> GetEventsAsync(MockUser currentUser, CancellationToken cancellationToken = default)
    {
        var query = _db.DatingEvents
            .Include(item => item.EventPlannerUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.ApprovedByUser)
            .ThenInclude(user => user!.Profile)
            .Include(item => item.EventType)
            .Include(item => item.EventMode)
            .Include(item => item.OnlineEventPlatform)
            .Include(item => item.Country)
            .Include(item => item.City)
            .Include(item => item.Faqs)
            .Include(item => item.EventTags)
            .ThenInclude(eventTag => eventTag.Tag)
            .AsQueryable();

        if (currentUser.Role == AdminRole.EventPlanner)
        {
            query = query.Where(item => item.EventPlannerUserId == currentUser.Id);
        }

        var events = await query
            .OrderByDescending(item => item.UpdatedAt ?? item.CreatedAt)
            .ToListAsync(cancellationToken);

        return events.Select(DatabaseModelMapper.ToAdminDatingEvent).ToList();
    }

    public async Task<EventListResult> GetEventsPageAsync(MockUser currentUser, EventListFilter filter, CancellationToken cancellationToken = default)
    {
        var nowUtc = DateTime.UtcNow;
        var pageNumber = Math.Max(filter.PageNumber, 1);
        var pageSize = Math.Clamp(filter.PageSize, 10, 100);

        var query = _db.DatingEvents.AsNoTracking().AsQueryable();

        if (currentUser.Role == AdminRole.EventPlanner)
        {
            query = query.Where(item => item.EventPlannerUserId == currentUser.Id);
        }

        query = filter.Scope == EventListScope.Archive
            ? query.Where(item => item.IsCancelled || item.DateTimeEnd <= nowUtc)
            : query.Where(item => !item.IsCancelled && item.DateTimeEnd > nowUtc);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            var like = $"%{search}%";
            var normalizedCode = search.Trim().TrimStart('#');
            var hasEventCode = int.TryParse(normalizedCode, out var eventCode);
            query = query.Where(item =>
                (hasEventCode && item.EventCode == eventCode)
                || EF.Functions.Like(item.EventCode.ToString(), like)
                ||
                EF.Functions.Like(item.Title, like)
                || EF.Functions.Like(item.EventPlannerUser.MobileNumber, like)
                || (item.EventPlannerUser.Email != null && EF.Functions.Like(item.EventPlannerUser.Email, like))
                || (item.EventPlannerUser.PendingEmail != null && EF.Functions.Like(item.EventPlannerUser.PendingEmail, like))
                || (item.EventPlannerUser.Profile != null && item.EventPlannerUser.Profile.DisplayName != null && EF.Functions.Like(item.EventPlannerUser.Profile.DisplayName, like)));
        }

        if (filter.TagId is long tagId)
        {
            query = query.Where(item => item.EventTags.Any(eventTag => eventTag.TagId == tagId));
        }

        if (!string.IsNullOrWhiteSpace(filter.City))
        {
            var city = filter.City.Trim();
            query = query.Where(item => item.City != null && item.City.Name == city);
        }

        if (filter.EventModeId is long eventModeId)
        {
            query = query.Where(item => item.EventModeId == eventModeId);
        }

        if (filter.OperationalStatus is Models.Events.EventOperationalStatus operationalStatus)
        {
            query = operationalStatus switch
            {
                Models.Events.EventOperationalStatus.Cancelled => query.Where(item => item.IsCancelled),
                Models.Events.EventOperationalStatus.Completed => query.Where(item => !item.IsCancelled && item.DateTimeEnd <= nowUtc),
                Models.Events.EventOperationalStatus.SaleOpen => query.Where(item => !item.IsCancelled && item.DateTimeEnd > nowUtc && item.IsOpenForSell),
                Models.Events.EventOperationalStatus.SaleClosed => query.Where(item => !item.IsCancelled && item.DateTimeEnd > nowUtc && !item.IsOpenForSell),
                _ => query.Where(item => !item.IsCancelled && item.DateTimeEnd > nowUtc && !item.IsOpenForSell)
            };
        }

        if (filter.ApprovalStatus is EventApprovalStatus approvalStatus)
            query = query.Where(item => item.ApprovalStatus == approvalStatus);

        if (filter.FromDateUtc is DateTimeOffset fromDate)
        {
            query = query.Where(item => item.DateTimeStart >= fromDate.UtcDateTime);
        }

        if (filter.ToDateUtc is DateTimeOffset toDate)
        {
            var inclusiveEnd = toDate.UtcDateTime.Date.AddDays(1);
            query = query.Where(item => item.DateTimeStart < inclusiveEnd);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        pageNumber = Math.Min(pageNumber, totalPages);

        query = filter.Sort switch
        {
            "start-asc" => query.OrderBy(item => item.DateTimeStart).ThenBy(item => item.Id),
            "start-desc" => query.OrderByDescending(item => item.DateTimeStart).ThenByDescending(item => item.Id),
            "title-asc" => query.OrderBy(item => item.Title).ThenBy(item => item.Id),
            "price-desc" => query.OrderByDescending(item => item.MaleTicketPrice > item.FemaleTicketPrice ? item.MaleTicketPrice : item.FemaleTicketPrice).ThenByDescending(item => item.Id),
            "price-asc" => query.OrderBy(item => item.MaleTicketPrice < item.FemaleTicketPrice ? item.MaleTicketPrice : item.FemaleTicketPrice).ThenBy(item => item.Id),
            _ => query.OrderByDescending(item => item.UpdatedAt ?? item.CreatedAt).ThenByDescending(item => item.Id)
        };

        var events = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(item => item.EventPlannerUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.ApprovedByUser)
            .ThenInclude(user => user!.Profile)
            .Include(item => item.EventType)
            .Include(item => item.EventMode)
            .Include(item => item.OnlineEventPlatform)
            .Include(item => item.Country)
            .Include(item => item.City)
            .Include(item => item.Faqs)
            .Include(item => item.EventTags)
            .ThenInclude(eventTag => eventTag.Tag)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return new EventListResult
        {
            TotalCount = totalCount,
            Items = events.Select(DatabaseModelMapper.ToAdminDatingEvent).ToList()
        };
    }

    public async Task<Models.Events.DatingEvent?> GetEventAsync(long id, CancellationToken cancellationToken = default)
    {
        var datingEvent = await _db.DatingEvents
            .Include(item => item.EventPlannerUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.ApprovedByUser)
            .ThenInclude(user => user!.Profile)
            .Include(item => item.EventType)
            .Include(item => item.EventMode)
            .Include(item => item.OnlineEventPlatform)
            .Include(item => item.Country)
            .Include(item => item.City)
            .Include(item => item.Faqs)
            .Include(item => item.EventTags)
            .ThenInclude(eventTag => eventTag.Tag)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (datingEvent is null)
            return null;

        var model = DatabaseModelMapper.ToAdminDatingEvent(datingEvent);
        model.IsCurrencyLocked = await HasEventFinancialActivityAsync(datingEvent.Id, cancellationToken);
        var smsRequests = await _db.EventParticipantSmsRequests
            .Include(item => item.RequestedByUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.ReviewedByAdminUser)
            .ThenInclude(user => user!.Profile)
            .Where(item => item.DatingEventId == id)
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync(cancellationToken);

        model.SmsRequests = smsRequests
            .Select(DatabaseModelMapper.ToEventSmsRequest)
            .ToList();

        var logs = await _db.AuditLogs
            .Where(item => item.TargetType == "DatingEvent" && item.TargetId == id.ToString())
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync(cancellationToken);

        var actorIds = logs.Where(item => item.ActorUserId.HasValue).Select(item => item.ActorUserId!.Value).Distinct().ToList();
        var actorNames = await _db.Users
            .Include(item => item.Profile)
            .Where(item => actorIds.Contains(item.Id))
            .ToDictionaryAsync(item => item.Id, DatabaseModelMapper.ResolveUserDisplayName, cancellationToken);

        model.ChangeLog = logs
            .Select(item => DatabaseModelMapper.ToEventChangeLogEntry(
                item,
                item.ActorUserId is long actorUserId && actorNames.TryGetValue(actorUserId, out var actorName)
                    ? actorName
                    : "سیستم"))
            .ToList();

        var latestReviewLog = logs.FirstOrDefault(item =>
            item.Action is "EventReviewApproved" or "EventReviewRejected" or "EventReviewSubmitted" or "EventSubmittedForReview");

        if (latestReviewLog is not null)
        {
            model.AdminReviewNote = latestReviewLog.Reason;
            model.ReviewedAtUtc = DateTime.SpecifyKind(latestReviewLog.CreatedAt, DateTimeKind.Utc);
            model.ReviewedByName = latestReviewLog.ActorUserId is long actorUserId
                && actorNames.TryGetValue(actorUserId, out var actorName)
                    ? actorName
                    : "سیستم";
        }

        return model;
    }

    public async Task<Models.Events.DatingEvent> SaveEventAsync(EventDraftInput input, MockUser actor, long? existingEventId = null, long? assignedPlannerId = null, bool submitForReview = false, CancellationToken cancellationToken = default)
    {
        var actorUser = await RequireUserAsync(actor.Id, cancellationToken);
        var plannerUser = await ResolvePlannerAsync(actor, assignedPlannerId, cancellationToken);
        var eventType = await _db.EventTypes.FirstOrDefaultAsync(item => item.Id == input.EventTypeId && item.IsActive, cancellationToken)
            ?? throw new InvalidOperationException("نوع رویداد انتخاب شده معتبر نیست.");
        var eventMode = await ResolveEventModeAsync(input.EventModeId, cancellationToken);
        var onlineEventPlatform = await ResolveOnlineEventPlatformAsync(eventMode, input.OnlineEventPlatformId, cancellationToken);
        var normalizedDelivery = NormalizeDeliveryInput(input, eventMode.IsOnline);
        var locationLookup = await ResolveLocationLookupAsync(normalizedDelivery.Country, normalizedDelivery.City, cancellationToken);
        var minimumEducationLevelId = await ResolveMinimumEducationLevelIdAsync(input.MinimumEducationLevelId, cancellationToken);
        input.MaleTicketCurrencyCode = await ResolveCurrencyCodeAsync(input.MaleTicketCurrencyCode, cancellationToken);
        input.FemaleTicketCurrencyCode = input.MaleTicketCurrencyCode;
        input.OrganizerPaymentAccountId = await ResolveOrganizerPaymentAccountIdAsync(input, plannerUser.Id, cancellationToken);
        var normalizedFaqs = NormalizeFaqs(input.Faqs);

        var maleRange = DatabaseModelMapper.ParseAgeRange(input.AgeRangeForMale);
        var femaleRange = DatabaseModelMapper.ParseAgeRange(input.AgeRangeForFemale);
        var beforeSnapshot = default(object);
        DomainDatingEvent datingEvent;

        if (existingEventId is long eventId)
        {
            datingEvent = await _db.DatingEvents
                .Include(item => item.EventPlannerUser)
                .ThenInclude(user => user.Profile)
                .Include(item => item.EventType)
                .Include(item => item.EventMode)
                .Include(item => item.OnlineEventPlatform)
                .Include(item => item.Faqs)
                .Include(item => item.EventTags)
                .ThenInclude(eventTag => eventTag.Tag)
                .FirstOrDefaultAsync(item => item.Id == eventId, cancellationToken)
                ?? throw new InvalidOperationException("رویداد مورد نظر پیدا نشد.");

            EnsureEventWriteAccess(actorUser, datingEvent);
            beforeSnapshot = CreateEventSnapshot(datingEvent);
            var hasFinancialActivity = await HasEventFinancialActivityAsync(datingEvent.Id, cancellationToken);
            if (hasFinancialActivity
                && !string.Equals(datingEvent.CurrencyCode, input.MaleTicketCurrencyCode, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("واحد پول رویداد بعد از ثبت خرید، رسید یا تراکنش قابل تغییر نیست.");
            }

            if (datingEvent.EventPlannerUserId != plannerUser.Id && actor.Role != AdminRole.EventPlanner)
            {
                datingEvent.ReassignPlanner(plannerUser);
            }

            datingEvent.UpdateDetails(
                input.Title,
                new Location(normalizedDelivery.Country, normalizedDelivery.City, new Coordinates(normalizedDelivery.Latitude, normalizedDelivery.Longitude), normalizedDelivery.Region),
                DatabaseModelMapper.ComposeStoredAddress(normalizedDelivery.VenueName, normalizedDelivery.Address),
                input.StartAtUtc.UtcDateTime,
                input.EndAtUtc.UtcDateTime,
                eventType,
                new AgeRange(maleRange.Min, maleRange.Max),
                new AgeRange(femaleRange.Min, femaleRange.Max),
                input.CapacityMale,
                input.CapacityFemale,
                input.LikeLimit,
                input.MaleTicketPrice,
                input.FemaleTicketPrice,
                input.EducationLevelRestriction,
                input.Tags,
                input.Image1,
                input.Image2,
                input.Image3,
                input.DescriptionHtml,
                input.MaleTicketCurrencyCode,
                input.FemaleTicketCurrencyCode,
                input.PaymentCollectionMethod,
                input.OrganizerPaymentInstructions,
                input.OrganizerPaymentAccountId);

            datingEvent.SetLocationLookup(locationLookup.CountryId, locationLookup.CityId);
            datingEvent.SetMinimumEducationLevel(minimumEducationLevelId);
            datingEvent.SetEventDelivery(eventMode, onlineEventPlatform, input.OnlineJoinUrl, input.OnlineAccessInstructions);
            datingEvent.ReplaceFaqs(normalizedFaqs);
            datingEvent.ReplaceTags(await ResolveEventTagsAsync(input.TagIds, cancellationToken));
            datingEvent.SetCommissionPercent(input.OrganizerCommissionPercent);
            if (submitForReview)
            {
                datingEvent.SubmitForReview();
            }

            await _auditLogger.LogAsync(new AuditLogEntry(
                actorUser.Id,
                submitForReview ? "EventSubmittedForReview" : "EventDraftSaved",
                "DatingEvent",
                datingEvent.Id.ToString(),
                JsonSerializer.Serialize(beforeSnapshot),
                JsonSerializer.Serialize(CreateEventSnapshot(datingEvent)),
                submitForReview ? "رویداد برای بررسی مدیر ارسال شد." : $"پیش‌نویس رویداد «{datingEvent.Title}» ذخیره شد."), cancellationToken);

            AddWorkflowLog(datingEvent, actorUser.Id, submitForReview ? EventWorkflowActionType.SubmittedForReview : EventWorkflowActionType.DraftSaved, beforeSnapshot, CreateEventSnapshot(datingEvent));
        }
        else
        {
            datingEvent = new DomainDatingEvent(
                plannerUser,
                input.Title,
                new Location(normalizedDelivery.Country, normalizedDelivery.City, new Coordinates(normalizedDelivery.Latitude, normalizedDelivery.Longitude), normalizedDelivery.Region),
                DatabaseModelMapper.ComposeStoredAddress(normalizedDelivery.VenueName, normalizedDelivery.Address),
                input.StartAtUtc.UtcDateTime,
                input.EndAtUtc.UtcDateTime,
                eventType,
                new AgeRange(maleRange.Min, maleRange.Max),
                new AgeRange(femaleRange.Min, femaleRange.Max),
                input.CapacityMale,
                input.CapacityFemale,
                input.LikeLimit,
                input.MaleTicketPrice,
                input.FemaleTicketPrice,
                input.EducationLevelRestriction,
                input.Tags,
                input.Image1,
                input.Image2,
                input.Image3,
                input.DescriptionHtml,
                input.OrganizerCommissionPercent,
                input.MaleTicketCurrencyCode,
                input.FemaleTicketCurrencyCode,
                input.PaymentCollectionMethod,
                input.OrganizerPaymentInstructions,
                input.OrganizerPaymentAccountId);

            if (submitForReview)
            {
                datingEvent.SubmitForReview();
            }

            datingEvent.SetLocationLookup(locationLookup.CountryId, locationLookup.CityId);
            datingEvent.SetMinimumEducationLevel(minimumEducationLevelId);
            datingEvent.SetEventDelivery(eventMode, onlineEventPlatform, input.OnlineJoinUrl, input.OnlineAccessInstructions);
            datingEvent.ReplaceFaqs(normalizedFaqs);
            _db.DatingEvents.Add(datingEvent);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            datingEvent.ReplaceTags(await ResolveEventTagsAsync(input.TagIds, cancellationToken));

            await _auditLogger.LogAsync(new AuditLogEntry(
                actorUser.Id,
                submitForReview ? "EventSubmittedForReview" : "EventDraftSaved",
                "DatingEvent",
                datingEvent.Id.ToString(),
                null,
                JsonSerializer.Serialize(CreateEventSnapshot(datingEvent)),
                submitForReview
                    ? "رویداد برای بررسی مدیر ارسال شد."
                    : $"پیش‌نویس رویداد «{datingEvent.Title}» برای {DatabaseModelMapper.ResolveUserDisplayName(plannerUser)} ساخته شد."), cancellationToken);

            AddWorkflowLog(datingEvent, actorUser.Id, submitForReview ? EventWorkflowActionType.SubmittedForReview : EventWorkflowActionType.DraftSaved, null, CreateEventSnapshot(datingEvent));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return (await GetEventAsync(datingEvent.Id, cancellationToken))!;
    }

    public async Task<Models.Events.DatingEvent> ApproveAsync(long eventId, MockUser admin, decimal? commissionPercent = null, string? note = null, CancellationToken cancellationToken = default)
    {
        var actor = await RequireAdminAsync(admin.Id, cancellationToken);
        var datingEvent = await RequireEventAsync(eventId, cancellationToken);

        if (commissionPercent is not null)
        {
            datingEvent.SetCommissionPercent(commissionPercent.Value);
        }

        var beforeSnapshot = CreateEventSnapshot(datingEvent);
        datingEvent.ApproveByAdmin(actor.Id, note);

        await _auditLogger.LogAsync(new AuditLogEntry(
            actor.Id,
            "EventReviewApproved",
            "DatingEvent",
            datingEvent.Id.ToString(),
            JsonSerializer.Serialize(beforeSnapshot),
            JsonSerializer.Serialize(CreateEventSnapshot(datingEvent)),
            string.IsNullOrWhiteSpace(note) ? "بررسی رویداد توسط مدیر تایید شد." : note.Trim()), cancellationToken);

        AddWorkflowLog(datingEvent, actor.Id, EventWorkflowActionType.Approved, beforeSnapshot, CreateEventSnapshot(datingEvent), note);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return (await GetEventAsync(datingEvent.Id, cancellationToken))!;
    }

    public async Task<Models.Events.DatingEvent> RejectAsync(long eventId, MockUser admin, string note, CancellationToken cancellationToken = default)
    {
        var actor = await RequireAdminAsync(admin.Id, cancellationToken);
        var datingEvent = await RequireEventAsync(eventId, cancellationToken);
        var beforeSnapshot = CreateEventSnapshot(datingEvent);
        datingEvent.RejectByAdmin(note);

        await _auditLogger.LogAsync(new AuditLogEntry(
            actor.Id,
            "EventReviewRejected",
            "DatingEvent",
            datingEvent.Id.ToString(),
            JsonSerializer.Serialize(beforeSnapshot),
            JsonSerializer.Serialize(CreateEventSnapshot(datingEvent)),
            string.IsNullOrWhiteSpace(note) ? "بررسی رویداد توسط مدیر رد شد." : note.Trim()), cancellationToken);

        AddWorkflowLog(datingEvent, actor.Id, EventWorkflowActionType.ReturnedToDraft, beforeSnapshot, CreateEventSnapshot(datingEvent), note);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return (await GetEventAsync(datingEvent.Id, cancellationToken))!;
    }

    public async Task<Models.Events.DatingEvent> SetCommissionAsync(long eventId, MockUser admin, decimal commissionPercent, CancellationToken cancellationToken = default)
    {
        var actor = await RequireAdminAsync(admin.Id, cancellationToken);
        var datingEvent = await RequireEventAsync(eventId, cancellationToken);
        var oldCommission = datingEvent.EventPlannerCommissionPercent;
        datingEvent.SetCommissionPercent(commissionPercent);

        await _auditLogger.LogAsync(new AuditLogEntry(
            actor.Id,
            "به روزرسانی کمیسیون رویداد",
            "DatingEvent",
            datingEvent.Id.ToString(),
            JsonSerializer.Serialize(new { CommissionPercent = oldCommission }),
            JsonSerializer.Serialize(new { CommissionPercent = datingEvent.EventPlannerCommissionPercent }),
            $"کمیسیون رویداد «{datingEvent.Title}» به {commissionPercent:0.##}% تغییر کرد."), cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return (await GetEventAsync(datingEvent.Id, cancellationToken))!;
    }

    public async Task<Models.Events.DatingEvent> ToggleSaleAsync(long eventId, MockUser admin, bool isOpen, CancellationToken cancellationToken = default)
    {
        var actor = await RequireUserAsync(admin.Id, cancellationToken);
        var datingEvent = await RequireEventAsync(eventId, cancellationToken);
        EnsureEventWriteAccess(actor, datingEvent);
        var beforeSnapshot = CreateEventSnapshot(datingEvent);
        ApplySaleStatus(datingEvent, isOpen);

        await _auditLogger.LogAsync(new AuditLogEntry(
            actor.Id,
            isOpen ? "EventSaleOpened" : "EventSaleClosed",
            "DatingEvent",
            datingEvent.Id.ToString(),
            JsonSerializer.Serialize(beforeSnapshot),
            JsonSerializer.Serialize(CreateEventSnapshot(datingEvent)),
            isOpen ? "فروش رویداد باز شد." : "فروش رویداد بسته شد."), cancellationToken);

        AddWorkflowLog(datingEvent, actor.Id, isOpen ? EventWorkflowActionType.SaleOpened : EventWorkflowActionType.SaleClosed, beforeSnapshot, CreateEventSnapshot(datingEvent));

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return (await GetEventAsync(datingEvent.Id, cancellationToken))!;
    }

    public async Task<Models.Events.DatingEvent> ApplyStatusTransitionAsync(long eventId, MockUser actor, EventStatusTransitionAction action, string? note = null, CancellationToken cancellationToken = default)
    {
        var currentEvent = await GetEventAsync(eventId, cancellationToken)
            ?? throw new InvalidOperationException("رویداد مورد نظر پیدا نشد.");

        var allowedActions = EventStatusTransitionCatalog
            .GetOptions(currentEvent, actor.Role)
            .Select(option => option.Action)
            .ToHashSet();

        if (!allowedActions.Contains(action))
            throw new InvalidOperationException("این تغییر وضعیت برای وضعیت فعلی رویداد یا نقش شما مجاز نیست.");

        return action switch
        {
            EventStatusTransitionAction.OpenSale => await ToggleSaleAsync(eventId, actor, true, cancellationToken),
            EventStatusTransitionAction.CloseSale => await ToggleSaleAsync(eventId, actor, false, cancellationToken),
            EventStatusTransitionAction.CancelEvent => (await CancelEventWithChecklistAsync(
                eventId,
                actor,
                RequireNote(note, "برای لغو رویداد دلیل یا توضیح لازم است."),
                "رویداد لغو شد. پیگیری‌های مالی و اطلاع‌رسانی مطابق قوانین پلتفرم انجام می‌شود.",
                confirmed: true,
                cancellationToken)).Event,
            _ => throw new InvalidOperationException("عملیات تغییر وضعیت پشتیبانی نمی‌شود.")
        };
    }

    public async Task<Models.Events.DatingEvent> CancelAsync(long eventId, MockUser admin, string? note = null, CancellationToken cancellationToken = default)
        => (await CancelEventWithChecklistAsync(
            eventId,
            admin,
            RequireNote(note, "برای لغو رویداد دلیل یا توضیح لازم است."),
            "رویداد لغو شد. پیگیری‌های مالی و اطلاع‌رسانی مطابق قوانین پلتفرم انجام می‌شود.",
            confirmed: true,
            cancellationToken)).Event;

    public async Task<EventCancellationPreview> PreviewCancellationAsync(long eventId, MockUser actor, CancellationToken cancellationToken = default)
    {
        var actorUser = await RequireUserAsync(actor.Id, cancellationToken);
        var datingEvent = await RequireEventAsync(eventId, cancellationToken);
        EnsureEventWriteAccess(actorUser, datingEvent);

        return await BuildCancellationPreviewAsync(datingEvent, cancellationToken);
    }

    public async Task<EventCancellationResult> CancelEventWithChecklistAsync(long eventId, MockUser actor, string reason, string publicMessage, bool confirmed, CancellationToken cancellationToken = default)
    {
        if (!confirmed)
            throw new InvalidOperationException("برای لغو رویداد باید چک‌لیست اثرات لغو را تایید کنید.");

        var actorUser = await RequireUserAsync(actor.Id, cancellationToken);
        var cancellationReason = RequireNote(reason, "برای لغو رویداد دلیل یا توضیح لازم است.");
        var cancellationPublicMessage = RequireNote(publicMessage, "برای لغو رویداد پیام اطلاع‌رسانی لازم است.");
        if (cancellationReason.Length > 1000)
            throw new InvalidOperationException("دلیل لغو حداکثر می‌تواند ۱۰۰۰ کاراکتر باشد.");

        if (cancellationPublicMessage.Length > 480)
            throw new InvalidOperationException("پیام اطلاع‌رسانی لغو حداکثر می‌تواند ۴۸۰ کاراکتر باشد.");

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        var datingEvent = await _db.DatingEvents
            .Include(item => item.Tickets)
            .ThenInclude(ticket => ticket.User)
            .Include(item => item.EventPlannerUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.EventType)
            .Include(item => item.EventMode)
            .Include(item => item.OnlineEventPlatform)
            .Include(item => item.Country)
            .Include(item => item.City)
            .Include(item => item.EventTags)
            .ThenInclude(eventTag => eventTag.Tag)
            .FirstOrDefaultAsync(item => item.Id == eventId, cancellationToken)
            ?? throw new InvalidOperationException("رویداد مورد نظر پیدا نشد.");

        EnsureEventWriteAccess(actorUser, datingEvent);

        var beforeSnapshot = CreateEventSnapshot(datingEvent);
        var preview = await BuildCancellationPreviewAsync(datingEvent, cancellationToken);
        if (!preview.CanCancel)
            throw new InvalidOperationException(preview.BlockingReasons.FirstOrDefault() ?? "لغو این رویداد در وضعیت فعلی مجاز نیست.");

        var activeTickets = datingEvent.Tickets
            .Where(ticket => !ticket.IsRefunded && !ticket.IsRemoved)
            .ToList();
        var paidOrders = await _db.TicketOrders
            .Include(order => order.BuyerUser)
            .ThenInclude(user => user.Profile)
            .Where(order => order.DatingEventId == eventId && order.PaymentStatus == TicketOrderPaymentStatus.Paid)
            .ToListAsync(cancellationToken);
        var pendingManualReceiptCount = await _db.ManualPaymentReceipts
            .CountAsync(receipt => receipt.DatingEventId == eventId && receipt.Status == ManualPaymentReceiptStatus.Submitted, cancellationToken);

        var smsRecipients = activeTickets
            .Select(ticket => ticket.User)
            .Concat(paidOrders.Select(order => order.BuyerUser))
            .Where(user => !string.IsNullOrWhiteSpace(user.MobileNumber))
            .GroupBy(user => user.Id)
            .Select(group => group.First())
            .ToList();

        datingEvent.Cancel(actorUser.Id, cancellationReason);

        var walletCreditCount = 0;
        var walletCreditTotal = 0m;
        var walletCreditTotalIrr = 0m;
        var organizerManualRefundCount = 0;
        var organizerManualRefundTotalIrr = 0m;

        foreach (var order in paidOrders)
        {
            order.MarkRefunded();

            var buyerBalance = await GetOrCreateBalanceAccountAsync(order.BuyerUser, cancellationToken);
            buyerBalance.Credit(
                order.NetAmount,
                BalanceTransactionType.TicketRefund,
                $"بازگشت وجه سفارش رویداد {datingEvent.Title}",
                datingEvent.Id,
                nameof(TicketOrder),
                order.Id,
                actorUser.Id,
                order.CurrencyCode,
                order.ReportingNetAmountIrr,
                order.ExchangeRateToIrr,
                order.ExchangeRateCapturedAtUtc,
                order.ExchangeRateId,
                order);
            walletCreditCount++;
            walletCreditTotal += order.NetAmount;
            walletCreditTotalIrr += order.ReportingNetAmountIrr;

            if (order.PaymentCollectionMethod == EventPaymentCollectionMethod.OrganizerManualTransfer)
            {
                var plannerBalance = await GetOrCreateBalanceAccountAsync(datingEvent.EventPlannerUser, cancellationToken);
                plannerBalance.DebitAllowNegative(
                    order.NetAmount,
                    BalanceTransactionType.OrganizerManualReceiptLiability,
                    $"بدهی برگزارکننده بابت اعتبار کیف پول سفارش رویداد لغوشده {datingEvent.Title}",
                    datingEvent.Id,
                    nameof(TicketOrder),
                    order.Id,
                    actorUser.Id,
                    order.CurrencyCode,
                    order.ReportingNetAmountIrr,
                    order.ExchangeRateToIrr,
                    order.ExchangeRateCapturedAtUtc,
                    order.ExchangeRateId,
                    order);
                organizerManualRefundCount++;
                organizerManualRefundTotalIrr += order.ReportingNetAmountIrr;
            }
        }

        var request = new EventCancellationRequest(datingEvent, actorUser, cancellationReason);
        var previewJson = JsonSerializer.Serialize(preview);
        request.Approve(actorUser, cancellationReason, cancellationPublicMessage, previewJson);
        _db.EventCancellationRequests.Add(request);

        foreach (var recipient in smsRecipients)
        {
            _db.SmsQueueItems.Add(new SmsQueueItem(recipient, datingEvent, cancellationPublicMessage));
        }

        if (smsRecipients.Count > 0)
        {
            var cancellationNotification = new Notification(
                actorUser,
                NotificationType.EventUpdate,
                "لغو رویداد",
                cancellationPublicMessage,
                NotificationPriority.Critical,
                requiresApproval: false,
                datingEvent,
                nameof(EventCancellationRequest),
                null);
            foreach (var recipient in smsRecipients)
                cancellationNotification.AddRecipient(recipient, NotificationDeliveryChannel.InApp);
            _db.Notifications.Add(cancellationNotification);
        }

        var afterPayload = new
        {
            Event = CreateEventSnapshot(datingEvent),
            activeTicketRefundCount = activeTickets.Count,
            paidOrderRefundCount = paidOrders.Count,
            walletCreditCount,
            walletCreditTotal,
            walletCreditTotalIrr,
            organizerManualRefundCount,
            organizerManualRefundTotalIrr,
            pendingManualReceiptReviewCount = pendingManualReceiptCount,
            smsRecipientCount = smsRecipients.Count
        };

        await _auditLogger.LogAsync(new AuditLogEntry(
            actorUser.Id,
            "EventCancelled",
            "DatingEvent",
            datingEvent.Id.ToString(),
            JsonSerializer.Serialize(beforeSnapshot),
            JsonSerializer.Serialize(afterPayload),
            cancellationReason), cancellationToken);

        AddWorkflowLog(
            datingEvent,
            actorUser.Id,
            EventWorkflowActionType.Cancelled,
            beforeSnapshot,
            afterPayload,
            cancellationReason,
            metadataJson: Truncate(previewJson, 3900));

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new EventCancellationResult
        {
            Event = (await GetEventAsync(datingEvent.Id, cancellationToken))!,
            Preview = preview
        };
    }

    private async Task<EventCancellationPreview> BuildCancellationPreviewAsync(DomainDatingEvent datingEvent, CancellationToken cancellationToken)
    {
        var operationalStatus = DatabaseModelMapper.ToEventOperationalStatus(datingEvent);
        var paidOrderQuery = _db.TicketOrders
            .AsNoTracking()
            .Where(order => order.DatingEventId == datingEvent.Id && order.PaymentStatus == TicketOrderPaymentStatus.Paid);
        var activeTicketQuery = _db.EventTickets
            .AsNoTracking()
            .Where(ticket => ticket.DatingEventId == datingEvent.Id && !ticket.IsRefunded && !ticket.IsRemoved);
        var manualReceiptQuery = _db.ManualPaymentReceipts
            .AsNoTracking()
            .Where(receipt => receipt.DatingEventId == datingEvent.Id);
        var settlementQuery = _db.EventSettlementRequests
            .AsNoTracking()
            .Where(request => request.DatingEventId == datingEvent.Id);

        var paidOrderCount = await paidOrderQuery.CountAsync(cancellationToken);
        var buyerCount = await paidOrderQuery.Select(order => order.BuyerUserId).Distinct().CountAsync(cancellationToken);
        var activeTicketCount = await activeTicketQuery.CountAsync(cancellationToken);
        var participantCount = await activeTicketQuery.Select(ticket => ticket.UserId).Distinct().CountAsync(cancellationToken);
        var pendingManualReceiptCount = await manualReceiptQuery.CountAsync(receipt => receipt.Status == ManualPaymentReceiptStatus.Submitted, cancellationToken);
        var approvedManualReceiptCount = await manualReceiptQuery.CountAsync(receipt => receipt.Status == ManualPaymentReceiptStatus.Approved, cancellationToken);
        var pendingSettlementRequestCount = await settlementQuery.CountAsync(request => request.Status == EventSettlementRequestStatus.Pending, cancellationToken);
        var approvedSettlementRequestCount = await settlementQuery.CountAsync(request => request.Status == EventSettlementRequestStatus.Approved, cancellationToken);
        var platformRefundAmountIrr = await paidOrderQuery.SumAsync(order => order.ReportingNetAmountIrr, cancellationToken);
        var organizerManualRefundAmountIrr = await paidOrderQuery
            .Where(order => order.PaymentCollectionMethod == EventPaymentCollectionMethod.OrganizerManualTransfer)
            .SumAsync(order => order.ReportingNetAmountIrr, cancellationToken);

        var blockingReasons = new List<string>();
        if (datingEvent.ApprovalStatus != EventApprovalStatus.Approved)
            blockingReasons.Add("پروفایل رویداد هنوز تایید نشده است؛ تغییر وضعیت عملیاتی فقط بعد از تایید پروفایل مجاز است.");
        if (operationalStatus == Models.Events.EventOperationalStatus.Cancelled)
            blockingReasons.Add("این رویداد قبلا لغو شده است.");
        if (operationalStatus == Models.Events.EventOperationalStatus.Completed)
            blockingReasons.Add("این رویداد تمام شده است و برای تغییر مالی باید از مسیر تسویه یا اصلاح مالی اقدام شود.");
        if (pendingSettlementRequestCount > 0)
            blockingReasons.Add("برای این رویداد درخواست تسویه در انتظار بررسی وجود دارد؛ قبل از لغو باید تکلیف تسویه مشخص شود.");
        if (approvedSettlementRequestCount > 0)
            blockingReasons.Add("برای این رویداد تسویه تایید شده وجود دارد؛ لغو بعد از تسویه نیازمند فرایند برگشت مالی جداگانه است.");

        var consequences = new List<string>
        {
            "فروش رویداد بسته می‌شود و وضعیت عملیاتی رویداد به «لغو شده» تغییر می‌کند.",
            "همه بلیت‌های فعال این رویداد از حالت معتبر خارج و به عنوان برگشت‌خورده ثبت می‌شوند.",
            "سفارش‌های پرداخت‌شده به وضعیت «بازگشت وجه» منتقل می‌شوند.",
            "درخواست لغو، لاگ تغییر وضعیت و audit مالی/عملیاتی در دیتابیس ثبت می‌شود."
        };

        if (platformRefundAmountIrr > 0)
            consequences.Add("برای همه سفارش‌های پرداخت‌شده، مبلغ به عنوان اعتبار کیف پول در حساب خریدار سفارش ثبت می‌شود تا بتواند در خریدهای بعدی استفاده کند.");
        if (organizerManualRefundAmountIrr > 0)
            consequences.Add("برای پرداخت‌های مستقیم به برگزارکننده، هم کیف پول خریدار شارژ می‌شود و هم بدهی برگزارکننده به همان مبلغ در حساب مالی برگزارکننده ثبت می‌شود.");
        if (pendingManualReceiptCount > 0)
            consequences.Add("رسیدهای دستیِ در انتظار بررسی رد نمی‌شوند؛ اگر بعداً تایید شوند، مبلغشان به کیف پول کاربر اضافه می‌شود نه اینکه برای رویداد لغوشده بلیت صادر شود.");
        if (buyerCount + participantCount > 0)
            consequences.Add("برای خریداران و شرکت‌کنندگان مرتبط، پیام اطلاع‌رسانی لغو در صف پیامک ثبت می‌شود.");

        var warnings = new List<string>();
        if (paidOrderCount == 0 && activeTicketCount == 0)
            warnings.Add("برای این رویداد هنوز خرید یا بلیت فعالی ثبت نشده است؛ لغو اثر مالی مستقیم ندارد.");
        if (organizerManualRefundAmountIrr > 0)
            warnings.Add("بخشی از پرداخت‌ها مستقیم به برگزارکننده انجام شده؛ اعتبار کاربر داخل سایت شارژ می‌شود و بدهی برگزارکننده باید در تسویه‌های بعدی کنترل شود.");
        if (approvedManualReceiptCount > 0)
            warnings.Add("برای این رویداد رسید دستی تایید شده وجود دارد؛ سفارش‌های مرتبط refund می‌شوند اما بررسی مالی آن باید در گزارش‌ها پیگیری شود.");

        var summary = paidOrderCount == 0 && activeTicketCount == 0
            ? "لغو این رویداد فروش را می‌بندد و رکورد لغو را ثبت می‌کند؛ اثر مالی مستقیمی پیدا نشد."
            : $"لغو این رویداد روی {paidOrderCount:N0} سفارش پرداخت‌شده، {activeTicketCount:N0} بلیت فعال، {buyerCount:N0} خریدار و {participantCount:N0} شرکت‌کننده اثر می‌گذارد.";
        var suggestedMessage = $"رویداد «{datingEvent.Title}» لغو شد. وضعیت پرداخت و پیگیری‌های لازم از طریق حساب کاربری شما اطلاع‌رسانی می‌شود.";

        return new EventCancellationPreview
        {
            EventId = datingEvent.Id,
            EventTitle = datingEvent.Title,
            CurrentOperationalStatus = DisplayFormatter.OperationalStatus(operationalStatus),
            CanCancel = blockingReasons.Count == 0,
            Summary = summary,
            RequiresManualRefundFollowUp = organizerManualRefundAmountIrr > 0,
            CreatesBuyerRefundCredits = paidOrderCount > 0,
            ActiveTicketCount = activeTicketCount,
            PaidOrderCount = paidOrderCount,
            BuyerCount = buyerCount,
            ParticipantCount = participantCount,
            PendingManualReceiptCount = pendingManualReceiptCount,
            ApprovedManualReceiptCount = approvedManualReceiptCount,
            PendingSettlementRequestCount = pendingSettlementRequestCount,
            ApprovedSettlementRequestCount = approvedSettlementRequestCount,
            PlatformRefundAmountIrr = platformRefundAmountIrr,
            OrganizerManualRefundAmountIrr = organizerManualRefundAmountIrr,
            SuggestedPublicMessage = suggestedMessage,
            Metrics = new[]
            {
                new EventCancellationPreviewMetric { Label = "سفارش پرداخت‌شده", Value = paidOrderCount.ToString("N0") },
                new EventCancellationPreviewMetric { Label = "بلیت فعال", Value = activeTicketCount.ToString("N0") },
                new EventCancellationPreviewMetric { Label = "خریدار", Value = buyerCount.ToString("N0") },
                new EventCancellationPreviewMetric { Label = "شرکت‌کننده", Value = participantCount.ToString("N0") },
                new EventCancellationPreviewMetric { Label = "اعتبار کیف پول خریداران", Value = FormatIrr(platformRefundAmountIrr), Hint = "برای استفاده در خریدهای بعدی." },
                new EventCancellationPreviewMetric { Label = "بدهی برگزارکننده", Value = FormatIrr(organizerManualRefundAmountIrr), Hint = "برای پرداخت مستقیم به برگزارکننده." },
                new EventCancellationPreviewMetric { Label = "رسید دستی در انتظار", Value = pendingManualReceiptCount.ToString("N0") },
                new EventCancellationPreviewMetric { Label = "تسویه درگیر", Value = (pendingSettlementRequestCount + approvedSettlementRequestCount).ToString("N0") }
            },
            Consequences = consequences,
            Warnings = warnings,
            BlockingReasons = blockingReasons
        };
    }

    public async Task<IReadOnlyList<EventTicketBuyerItem>> GetEventTicketBuyersAsync(long eventId, MockUser actor, CancellationToken cancellationToken = default)
    {
        var currentUser = await RequireUserAsync(actor.Id, cancellationToken);
        var datingEvent = await _db.DatingEvents
            .Include(item => item.EventPlannerUser)
            .ThenInclude(user => user.Profile)
            .FirstOrDefaultAsync(item => item.Id == eventId, cancellationToken)
            ?? throw new InvalidOperationException("رویداد مورد نظر پیدا نشد.");

        EnsureEventWriteAccess(currentUser, datingEvent);

        var canSeeMobile = currentUser.Role == UserRole.Admin;
        var tickets = await _db.EventTickets
            .Include(item => item.User)
            .ThenInclude(user => user.Profile)
            .Include(item => item.User.Profile!.GenderLookup)
            .Include(item => item.User.Profile!.EducationLevelLookup)
            .Include(item => item.User.Profile!.Country)
            .Include(item => item.User.Profile!.City)
            .Include(item => item.User.Profile!.Images)
            .Where(item => item.DatingEventId == eventId)
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync(cancellationToken);

        return tickets.Select(ticket =>
        {
            var profile = ticket.User.Profile;
            var country = profile?.Country?.Name ?? "ثبت نشده";
            var city = profile?.City?.Name ?? "ثبت نشده";
            var genderTitle = profile?.GenderLookup?.Title ?? DisplayFormatter.Gender(ticket.Gender);
            var educationTitle = profile?.EducationLevelLookup?.Title
                ?? (profile is null ? "ثبت نشده" : profile.EducationLevel.ToString());

            return new EventTicketBuyerItem
            {
                TicketId = ticket.Id,
                EventId = ticket.DatingEventId,
                UserId = ticket.UserId,
                DisplayName = profile?.DisplayName ?? $"شرکت‌کننده {ticket.UserId}",
                ProfileImageUrl = profile?.Images
                    .OrderByDescending(image => image.IsPrimary)
                    .ThenBy(image => image.DisplayOrder)
                    .Select(image => image.ImageUrl)
                    .FirstOrDefault(),
                MobileNumber = canSeeMobile ? ticket.User.MobileNumber : null,
                Gender = ticket.Gender,
                GenderTitle = genderTitle,
                Age = profile?.Age ?? 0,
                EducationLevelTitle = educationTitle,
                Country = country,
                City = city,
                TicketPrice = ticket.Price,
                TicketCurrencyCode = ticket.CurrencyCode,
                IsRefunded = ticket.IsRefunded,
                IsRemoved = ticket.IsRemoved,
                TicketStatus = ticket.IsRemoved
                    ? "حذف و بازگشت وجه"
                    : ticket.IsRefunded
                        ? "بازگشت وجه"
                        : "فعال",
                PurchasedAtUtc = DateTime.SpecifyKind(ticket.CreatedAt, DateTimeKind.Utc),
                RemovalReason = ticket.RemovalReason
            };
        }).ToList();
    }

    public async Task EmergencyRefundTicketAsync(long eventId, long ticketId, MockUser admin, string reason, CancellationToken cancellationToken = default)
    {
        var actor = await RequireAdminAsync(admin.Id, cancellationToken);
        var ticket = await _db.EventTickets
            .Include(item => item.User)
            .ThenInclude(user => user.Profile)
            .Include(item => item.DatingEvent)
            .ThenInclude(datingEvent => datingEvent.EventPlannerUser)
            .FirstOrDefaultAsync(item => item.Id == ticketId && item.DatingEventId == eventId, cancellationToken)
            ?? throw new InvalidOperationException("بلیت مورد نظر پیدا نشد.");

        if (ticket.IsRefunded || ticket.IsRemoved)
            throw new InvalidOperationException("برای این بلیت قبلا بازگشت وجه ثبت شده است.");

        ticket.RemoveWithRefund(actor.Id, reason.Trim());

        var buyerBalance = await GetOrCreateBalanceAccountAsync(ticket.User, cancellationToken);
        buyerBalance.Credit(
            ticket.Price,
            BalanceTransactionType.EmergencyRemovalRefund,
            $"بازگشت اضطراری بلیت رویداد {ticket.DatingEvent.Title}",
            ticket.DatingEventId,
            nameof(EventTicket),
            ticket.Id,
            actor.Id,
            ticket.CurrencyCode,
            ticket.ReportingPriceIrr,
            ticket.ExchangeRateToIrr,
            ticket.ExchangeRateCapturedAtUtc,
            ticket.ExchangeRateId);

        var plannerIncome = ticket.Price * (100 - ticket.DatingEvent.EventPlannerCommissionPercent) / 100;
        var hasSettlementCredit = await _db.BalanceTransactions.AnyAsync(
            transaction =>
                transaction.UserId == ticket.DatingEvent.EventPlannerUserId
                && transaction.DatingEventId == ticket.DatingEventId
                && transaction.Type == BalanceTransactionType.EventSettlementCredit,
            cancellationToken);

        if (plannerIncome > 0 && hasSettlementCredit)
        {
            var plannerBalance = await GetOrCreateBalanceAccountAsync(ticket.DatingEvent.EventPlannerUser, cancellationToken);
            plannerBalance.DebitAllowNegative(
                plannerIncome,
                BalanceTransactionType.EventSettlementReversal,
                $"برگشت بستانکاری برگزارکننده بابت بازگشت اضطراری بلیت {ticket.DatingEvent.Title}",
                ticket.DatingEventId,
                nameof(EventTicket),
                ticket.Id,
                actor.Id,
                ticket.CurrencyCode,
                Math.Round(ticket.ReportingPriceIrr * (100 - ticket.DatingEvent.EventPlannerCommissionPercent) / 100, 0, MidpointRounding.AwayFromZero),
                ticket.ExchangeRateToIrr,
                ticket.ExchangeRateCapturedAtUtc,
                ticket.ExchangeRateId);
        }

        await _auditLogger.LogAsync(new AuditLogEntry(
            actor.Id,
            "بازگشت اضطراری بلیت",
            "EventTicket",
            ticket.Id.ToString(),
            null,
            JsonSerializer.Serialize(new
            {
                ticket.DatingEventId,
                ticket.UserId,
                ticket.Price,
                PlannerIncomeReversal = plannerIncome,
                ticket.RemovalReason
            }),
            $"بلیت شرکت‌کننده «{DatabaseModelMapper.ResolveUserDisplayName(ticket.User)}» از رویداد «{ticket.DatingEvent.Title}» حذف و وجه آن برگشت داده شد."), cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EventSmsRequest>> GetSmsRequestsAsync(long eventId, MockUser actor, CancellationToken cancellationToken = default)
    {
        var currentUser = await RequireUserAsync(actor.Id, cancellationToken);
        var datingEvent = await _db.DatingEvents
            .Include(item => item.EventPlannerUser)
            .FirstOrDefaultAsync(item => item.Id == eventId, cancellationToken)
            ?? throw new InvalidOperationException("رویداد مورد نظر پیدا نشد.");

        EnsureEventWriteAccess(currentUser, datingEvent);

        var requests = await _db.EventParticipantSmsRequests
            .Include(item => item.RequestedByUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.ReviewedByAdminUser)
            .ThenInclude(user => user!.Profile)
            .Where(item => item.DatingEventId == eventId)
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync(cancellationToken);

        return requests.Select(DatabaseModelMapper.ToEventSmsRequest).ToList();
    }

    public async Task RequestSmsAsync(long eventId, MockUser actor, string message, DateTimeOffset? plannedSendAtUtc = null, CancellationToken cancellationToken = default)
    {
        var requester = await RequireUserAsync(actor.Id, cancellationToken);
        var datingEvent = await _db.DatingEvents
            .Include(item => item.EventPlannerUser)
            .FirstOrDefaultAsync(item => item.Id == eventId, cancellationToken)
            ?? throw new InvalidOperationException("رویداد مورد نظر پیدا نشد.");

        EnsureEventWriteAccess(requester, datingEvent);

        var request = new EventParticipantSmsRequest(requester, datingEvent, message, plannedSendAtUtc?.UtcDateTime);
        _db.EventParticipantSmsRequests.Add(request);

        await _auditLogger.LogAsync(new AuditLogEntry(
            requester.Id,
            "درخواست پیام به شرکت کنندگان",
            "DatingEvent",
            datingEvent.Id.ToString(),
            null,
            JsonSerializer.Serialize(new { request.Message, request.PlannedSendAtUtc }),
            request.PlannedSendAtUtc.HasValue
                ? "درخواست ارسال زمان بندی شده پیامک برای بررسی ثبت شد."
                : "درخواست ارسال پیامک برای بررسی ثبت شد."), cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ApproveSmsRequestAsync(long eventId, long requestId, MockUser admin, string approvedMessage, DateTimeOffset? plannedSendAtUtc = null, string? note = null, CancellationToken cancellationToken = default)
    {
        var reviewer = await RequireAdminAsync(admin.Id, cancellationToken);
        var request = await _db.EventParticipantSmsRequests
            .Include(item => item.DatingEvent)
            .ThenInclude(datingEvent => datingEvent.Tickets)
            .Include(item => item.RequestedByUser)
            .FirstOrDefaultAsync(item => item.Id == requestId && item.DatingEventId == eventId, cancellationToken)
            ?? throw new InvalidOperationException("درخواست پیامک پیدا نشد.");

        var participantUserIds = request.DatingEvent.Tickets
            .Where(item => !item.IsRefunded)
            .Select(item => item.UserId)
            .Distinct()
            .ToList();

        var participants = await _db.Users
            .Where(item => participantUserIds.Contains(item.Id))
            .ToListAsync(cancellationToken);

        foreach (var participant in participants)
        {
            _db.SmsQueueItems.Add(new SmsQueueItem(
                participant,
                request.DatingEvent,
                approvedMessage,
                plannedSendAtUtc?.UtcDateTime,
                request.Id));
        }

        request.Approve(reviewer.Id, participants.Count, approvedMessage, plannedSendAtUtc?.UtcDateTime, note);

        await _auditLogger.LogAsync(new AuditLogEntry(
            reviewer.Id,
            "تایید پیام به شرکت کنندگان",
            "DatingEvent",
            request.DatingEventId.ToString(),
            null,
            JsonSerializer.Serialize(new { request.QueuedRecipientsCount, approvedMessage, request.PlannedSendAtUtc }),
            string.IsNullOrWhiteSpace(note) ? "درخواست پیامک تایید و وارد صف ارسال شد." : note.Trim()), cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectSmsRequestAsync(long eventId, long requestId, MockUser admin, string note, CancellationToken cancellationToken = default)
    {
        var reviewer = await RequireAdminAsync(admin.Id, cancellationToken);
        var request = await _db.EventParticipantSmsRequests
            .FirstOrDefaultAsync(item => item.Id == requestId && item.DatingEventId == eventId, cancellationToken)
            ?? throw new InvalidOperationException("درخواست پیامک پیدا نشد.");

        request.Reject(reviewer.Id, note);

        await _auditLogger.LogAsync(new AuditLogEntry(
            reviewer.Id,
            "رد پیام به شرکت کنندگان",
            "DatingEvent",
            request.DatingEventId.ToString(),
            null,
            JsonSerializer.Serialize(new { request.Status }),
            note.Trim()), cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static string RequireNote(string? note, string message)
    {
        if (string.IsNullOrWhiteSpace(note))
            throw new InvalidOperationException(message);

        return note.Trim();
    }

    private static string FormatIrr(decimal amount) => $"{amount:N0} ریال";

    private static string Truncate(string value, int maxLength)
        => value.Length <= maxLength ? value : value[..maxLength];

    private async Task<User> RequireUserAsync(long userId, CancellationToken cancellationToken)
    {
        return await _db.Users
            .Include(item => item.Profile)
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException("حساب جاری پیدا نشد.");
    }

    private async Task<User> RequireAdminAsync(long userId, CancellationToken cancellationToken)
    {
        var user = await RequireUserAsync(userId, cancellationToken);
        if (user.Role != UserRole.Admin)
            throw new InvalidOperationException("فقط مدیر می تواند این عملیات را انجام دهد.");

        return user;
    }

    private async Task<DomainDatingEvent> RequireEventAsync(long eventId, CancellationToken cancellationToken)
    {
        return await _db.DatingEvents
            .Include(item => item.EventPlannerUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.EventType)
            .Include(item => item.EventMode)
            .Include(item => item.OnlineEventPlatform)
            .Include(item => item.Country)
            .Include(item => item.City)
            .Include(item => item.Faqs)
            .Include(item => item.EventTags)
            .ThenInclude(eventTag => eventTag.Tag)
            .FirstOrDefaultAsync(item => item.Id == eventId, cancellationToken)
            ?? throw new InvalidOperationException("رویداد مورد نظر پیدا نشد.");
    }

    private async Task<User> ResolvePlannerAsync(MockUser actor, long? assignedPlannerId, CancellationToken cancellationToken)
    {
        var plannerUserId = actor.Role == AdminRole.EventPlanner ? actor.Id : assignedPlannerId;
        if (plannerUserId is null or 0)
            throw new InvalidOperationException("انتخاب برگزارکننده الزامی است.");

        var planner = await _db.Users
            .Include(item => item.Profile)
            .FirstOrDefaultAsync(item => item.Id == plannerUserId.Value, cancellationToken)
            ?? throw new InvalidOperationException("برگزارکننده انتخاب شده پیدا نشد.");

        if (planner.Role != UserRole.EventPlanner && planner.Role != UserRole.Admin)
            throw new InvalidOperationException("حساب انتخاب‌شده برگزارکننده نیست.");

        return planner;
    }

    private async Task<BalanceAccount> GetOrCreateBalanceAccountAsync(User user, CancellationToken cancellationToken)
    {
        var account = await _db.BalanceAccounts
            .Include(item => item.Transactions)
            .FirstOrDefaultAsync(item => item.UserId == user.Id, cancellationToken);
        if (account is not null)
            return account;

        account = new BalanceAccount(user);
        _db.BalanceAccounts.Add(account);
        return account;
    }

    private async Task<(long CountryId, long CityId)> ResolveLocationLookupAsync(string countryName, string cityName, CancellationToken cancellationToken)
    {
        var city = await _db.Cities
            .Include(item => item.Country)
            .Where(item => item.IsActive && item.Country.IsActive)
            .FirstOrDefaultAsync(item => item.Country.Name == countryName && item.Name == cityName, cancellationToken)
            ?? throw new InvalidOperationException("شهر انتخاب شده برای این کشور معتبر نیست.");

        return (city.CountryId, city.Id);
    }

    private async Task<long?> ResolveMinimumEducationLevelIdAsync(long? minimumEducationLevelId, CancellationToken cancellationToken)
    {
        if (minimumEducationLevelId is null or 0)
            return null;

        var exists = await _db.EducationLevels.AnyAsync(item => item.Id == minimumEducationLevelId && item.IsActive, cancellationToken);
        if (!exists)
            throw new InvalidOperationException("حداقل سطح تحصیل انتخاب شده معتبر نیست.");

        return minimumEducationLevelId;
    }

    private async Task<string> ResolveCurrencyCodeAsync(string? currencyCode, CancellationToken cancellationToken)
    {
        var normalized = CurrencyLookup.NormalizeCode(string.IsNullOrWhiteSpace(currencyCode) ? "IRR" : currencyCode);
        var exists = await _db.Currencies.AnyAsync(item => item.Code == normalized && item.IsActive, cancellationToken);
        if (!exists)
            throw new InvalidOperationException("واحد پول انتخاب شده معتبر نیست.");

        return normalized;
    }

    private async Task<long?> ResolveOrganizerPaymentAccountIdAsync(EventDraftInput input, long plannerUserId, CancellationToken cancellationToken)
    {
        if (input.PaymentCollectionMethod != EventPaymentCollectionMethod.OrganizerManualTransfer)
        {
            input.OrganizerPaymentInstructions = null;
            return null;
        }

        if (input.OrganizerPaymentAccountId is null or <= 0)
            throw new InvalidOperationException("برای واریز مستقیم، حساب دریافت وجه برگزارکننده را انتخاب کنید.");

        var normalizedCurrency = CurrencyLookup.NormalizeCode(input.MaleTicketCurrencyCode);
        var account = await _db.PlannerBankAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == input.OrganizerPaymentAccountId.Value, cancellationToken)
            ?? throw new InvalidOperationException("حساب دریافت وجه برگزارکننده پیدا نشد.");

        if (account.UserId != plannerUserId)
            throw new InvalidOperationException("حساب دریافت وجه انتخاب‌شده متعلق به برگزارکننده این رویداد نیست.");

        if (!account.IsActive)
            throw new InvalidOperationException("حساب دریافت وجه انتخاب‌شده فعال نیست.");

        if (!string.Equals(account.CurrencyCode, normalizedCurrency, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("ارز حساب دریافت وجه با ارز رویداد هماهنگ نیست.");

        input.OrganizerPaymentInstructions = account.PublicPaymentInstructions;
        return account.Id;
    }

    private async Task<bool> HasEventFinancialActivityAsync(long eventId, CancellationToken cancellationToken)
    {
        return await _db.EventTickets.AnyAsync(item => item.DatingEventId == eventId, cancellationToken)
            || await _db.OnlinePayments.AnyAsync(item => item.DatingEventId == eventId, cancellationToken)
            || await _db.BalanceTransactions.AnyAsync(item => item.DatingEventId == eventId, cancellationToken)
            || await _db.ManualPaymentReceipts.AnyAsync(item => item.DatingEventId == eventId, cancellationToken);
    }

    private async Task<EventModeLookup> ResolveEventModeAsync(long eventModeId, CancellationToken cancellationToken)
    {
        var normalizedEventModeId = eventModeId <= 0 ? 2L : eventModeId;
        return await _db.EventModes.FirstOrDefaultAsync(item => item.Id == normalizedEventModeId && item.IsActive, cancellationToken)
            ?? throw new InvalidOperationException("نحوه برگزاری انتخاب شده معتبر نیست.");
    }

    private async Task<OnlineEventPlatform?> ResolveOnlineEventPlatformAsync(EventModeLookup eventMode, long? platformId, CancellationToken cancellationToken)
    {
        if (!eventMode.IsOnline)
            return null;

        if (platformId is null or 0)
            throw new InvalidOperationException("برای رویداد آنلاین انتخاب پلتفرم الزامی است.");

        return await _db.OnlineEventPlatforms.FirstOrDefaultAsync(item => item.Id == platformId && item.IsActive, cancellationToken)
            ?? throw new InvalidOperationException("پلتفرم آنلاین انتخاب شده معتبر نیست.");
    }

    private static (string Country, string City, string Region, string VenueName, string Address, decimal Latitude, decimal Longitude) NormalizeDeliveryInput(EventDraftInput input, bool isOnline)
    {
        if (!isOnline)
        {
            return (
                input.Country,
                input.City,
                input.Region,
                input.VenueName,
                input.Address,
                input.Latitude,
                input.Longitude);
        }

        return (
            string.IsNullOrWhiteSpace(input.Country) ? "ایران" : input.Country,
            string.IsNullOrWhiteSpace(input.City) ? "تهران" : input.City,
            "آنلاین",
            "رویداد آنلاین",
            "لینک حضور آنلاین پس از خرید بلیت نمایش داده می شود.",
            input.Latitude == 0 ? 35.6892m : input.Latitude,
            input.Longitude == 0 ? 51.3890m : input.Longitude);
    }

    private static IReadOnlyList<(string Question, string Answer)> NormalizeFaqs(IEnumerable<EventFaqInput>? faqs)
    {
        return (faqs ?? Array.Empty<EventFaqInput>())
            .Select(item => (Question: item.Question?.Trim() ?? string.Empty, Answer: item.Answer?.Trim() ?? string.Empty))
            .Where(item => !string.IsNullOrWhiteSpace(item.Question) || !string.IsNullOrWhiteSpace(item.Answer))
            .Take(10)
            .ToList();
    }

    private async Task<IReadOnlyList<Tag>> ResolveEventTagsAsync(IReadOnlyCollection<long>? tagIds, CancellationToken cancellationToken)
    {
        var normalizedTagIds = (tagIds ?? Array.Empty<long>())
            .Where(id => id > 0)
            .Distinct()
            .Take(10)
            .ToList();

        if (normalizedTagIds.Count == 0)
            return Array.Empty<Tag>();

        var tags = await _db.Tags
            .Where(item => normalizedTagIds.Contains(item.Id) && item.IsActive)
            .ToListAsync(cancellationToken);

        if (tags.Count != normalizedTagIds.Count)
            throw new InvalidOperationException("یک یا چند تگ انتخاب شده معتبر نیست.");

        return tags;
    }

    private static void EnsureEventWriteAccess(User actor, DomainDatingEvent datingEvent)
    {
        if (actor.Role != UserRole.Admin && datingEvent.EventPlannerUserId != actor.Id)
            throw new InvalidOperationException("شما به این رویداد دسترسی ویرایش ندارید.");
    }

    private static void ApplySaleStatus(DomainDatingEvent datingEvent, bool isOpen)
    {
        if (datingEvent.IsCancelled)
            return;

        if (isOpen && datingEvent.SaleStatus != EventSaleStatus.Open)
            datingEvent.OpenForSell();
        else if (!isOpen && datingEvent.SaleStatus != EventSaleStatus.Closed)
            datingEvent.CloseForSell();
    }

    private void AddWorkflowLog(DomainDatingEvent datingEvent, long? actorUserId, EventWorkflowActionType actionType, object? beforeSnapshot, object? afterSnapshot, string? reason = null, string? metadataJson = null)
    {
        _db.EventWorkflowLogs.Add(new EventWorkflowLog(
            datingEvent,
            actionType,
            actorUserId,
            toApprovalStatus: datingEvent.ApprovalStatus,
            toSaleStatus: datingEvent.SaleStatus,
            toLifecycleStatus: datingEvent.LifecycleStatus,
            reason: reason,
            beforeJson: beforeSnapshot is null ? null : JsonSerializer.Serialize(beforeSnapshot),
            afterJson: afterSnapshot is null ? null : JsonSerializer.Serialize(afterSnapshot),
            metadataJson: metadataJson));
    }

    private static object CreateEventSnapshot(DomainDatingEvent datingEvent)
    {
        return new
        {
            datingEvent.Title,
            datingEvent.EventPlannerUserId,
            datingEvent.EventTypeId,
            datingEvent.EventModeId,
            EventModeName = datingEvent.EventMode?.Name,
            datingEvent.OnlineEventPlatformId,
            OnlinePlatformName = datingEvent.OnlineEventPlatform?.Name,
            datingEvent.OnlineJoinUrl,
            datingEvent.OnlineAccessInstructions,
            CountryName = datingEvent.Country?.Name,
            CityName = datingEvent.City?.Name,
            datingEvent.Location.Region,
            datingEvent.Address,
            datingEvent.DateTimeStart,
            datingEvent.DateTimeEnd,
            datingEvent.EventPlannerCommissionPercent,
            datingEvent.PaymentCollectionMethod,
            datingEvent.OrganizerPaymentInstructions,
            datingEvent.OrganizerPaymentAccountId,
            datingEvent.MaleCapacity,
            datingEvent.FemaleCapacity,
            datingEvent.NumberOfLikesAllowed,
            datingEvent.MaleTicketPrice,
            datingEvent.MaleTicketCurrencyCode,
            datingEvent.FemaleTicketPrice,
            datingEvent.CurrencyCode,
            datingEvent.FemaleTicketCurrencyCode,
            datingEvent.EducationLevelRestriction,
            Tags = datingEvent.Tags.ToArray(),
            datingEvent.EventDescriptionHtml,
            datingEvent.EventImage1,
            datingEvent.EventImage2,
            datingEvent.EventImage3,
            Faqs = datingEvent.Faqs
                .OrderBy(item => item.DisplayOrder)
                .Select(item => new { item.Question, item.Answer })
                .ToArray(),
            datingEvent.IsOpenForSell,
            datingEvent.IsCancelled,
            datingEvent.ReviewStatus,
            datingEvent.ApprovalStatus,
            datingEvent.SaleStatus,
            datingEvent.LifecycleStatus,
            datingEvent.AdminReviewNote,
            datingEvent.ApprovedAtUtc,
            datingEvent.ApprovedByUserId,
            datingEvent.CancelledAtUtc,
            datingEvent.CancelledByUserId,
            datingEvent.CancellationReason,
            datingEvent.CompletedAtUtc,
            OperationalStatus = datingEvent.ResolveOperationalStatus(DateTime.UtcNow)
        };
    }
}
