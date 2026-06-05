using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Auth;
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

    public async Task<IReadOnlyList<Models.Events.DatingEvent>> GetEventsAsync(MockUser currentUser, CancellationToken cancellationToken = default)
    {
        var query = _db.DatingEvents
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

    public async Task<Models.Events.DatingEvent?> GetEventAsync(long id, CancellationToken cancellationToken = default)
    {
        var datingEvent = await _db.DatingEvents
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
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (datingEvent is null)
            return null;

        var model = DatabaseModelMapper.ToAdminDatingEvent(datingEvent);
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
            item.Action is "EventReviewApproved" or "EventReviewRejected" or "EventReviewSubmitted");

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

    public async Task<Models.Events.DatingEvent> SaveEventAsync(EventDraftInput input, MockUser actor, long? existingEventId = null, long? assignedPlannerId = null, CancellationToken cancellationToken = default)
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
                input.DescriptionHtml);

            datingEvent.SetLocationLookup(locationLookup.CountryId, locationLookup.CityId);
            datingEvent.SetMinimumEducationLevel(minimumEducationLevelId);
            datingEvent.SetEventDelivery(eventMode, onlineEventPlatform, input.OnlineJoinUrl, input.OnlineAccessInstructions);
            datingEvent.ReplaceFaqs(normalizedFaqs);
            datingEvent.ReplaceTags(await ResolveEventTagsAsync(input.TagIds, cancellationToken));
            datingEvent.SetCommissionPercent(input.OrganizerCommissionPercent);
            await _auditLogger.LogAsync(new AuditLogEntry(
                actorUser.Id,
                "ویرایش رویداد",
                "DatingEvent",
                datingEvent.Id.ToString(),
                JsonSerializer.Serialize(beforeSnapshot),
                JsonSerializer.Serialize(CreateEventSnapshot(datingEvent)),
                $"رویداد «{datingEvent.Title}» به روز شد."), cancellationToken);

            await _auditLogger.LogAsync(new AuditLogEntry(
                actorUser.Id,
                "EventReviewSubmitted",
                "DatingEvent",
                datingEvent.Id.ToString(),
                JsonSerializer.Serialize(beforeSnapshot),
                JsonSerializer.Serialize(CreateEventSnapshot(datingEvent)),
                "رویداد برای بررسی مدیر ارسال شد."), cancellationToken);
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
                input.OrganizerCommissionPercent);

            datingEvent.SubmitForReview();
            datingEvent.SetLocationLookup(locationLookup.CountryId, locationLookup.CityId);
            datingEvent.SetMinimumEducationLevel(minimumEducationLevelId);
            datingEvent.SetEventDelivery(eventMode, onlineEventPlatform, input.OnlineJoinUrl, input.OnlineAccessInstructions);
            datingEvent.ReplaceFaqs(normalizedFaqs);
            _db.DatingEvents.Add(datingEvent);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            datingEvent.ReplaceTags(await ResolveEventTagsAsync(input.TagIds, cancellationToken));

            await _auditLogger.LogAsync(new AuditLogEntry(
                actorUser.Id,
                "ایجاد رویداد",
                "DatingEvent",
                datingEvent.Id.ToString(),
                null,
                JsonSerializer.Serialize(CreateEventSnapshot(datingEvent)),
                $"رویداد «{datingEvent.Title}» برای {DatabaseModelMapper.ResolveUserDisplayName(plannerUser)} ساخته شد."), cancellationToken);

            await _auditLogger.LogAsync(new AuditLogEntry(
                actorUser.Id,
                "EventReviewSubmitted",
                "DatingEvent",
                datingEvent.Id.ToString(),
                null,
                JsonSerializer.Serialize(CreateEventSnapshot(datingEvent)),
                "رویداد برای بررسی مدیر ارسال شد."), cancellationToken);
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
        datingEvent.ApproveByAdmin();

        await _auditLogger.LogAsync(new AuditLogEntry(
            actor.Id,
            "EventReviewApproved",
            "DatingEvent",
            datingEvent.Id.ToString(),
            JsonSerializer.Serialize(beforeSnapshot),
            JsonSerializer.Serialize(CreateEventSnapshot(datingEvent)),
            string.IsNullOrWhiteSpace(note) ? "بررسی رویداد توسط مدیر تایید شد." : note.Trim()), cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return (await GetEventAsync(datingEvent.Id, cancellationToken))!;
    }

    public async Task<Models.Events.DatingEvent> RejectAsync(long eventId, MockUser admin, string note, CancellationToken cancellationToken = default)
    {
        var actor = await RequireAdminAsync(admin.Id, cancellationToken);
        var datingEvent = await RequireEventAsync(eventId, cancellationToken);
        var beforeSnapshot = CreateEventSnapshot(datingEvent);
        datingEvent.RejectByAdmin();

        await _auditLogger.LogAsync(new AuditLogEntry(
            actor.Id,
            "EventReviewRejected",
            "DatingEvent",
            datingEvent.Id.ToString(),
            JsonSerializer.Serialize(beforeSnapshot),
            JsonSerializer.Serialize(CreateEventSnapshot(datingEvent)),
            string.IsNullOrWhiteSpace(note) ? "بررسی رویداد توسط مدیر رد شد." : note.Trim()), cancellationToken);

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

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return (await GetEventAsync(datingEvent.Id, cancellationToken))!;
    }

    public async Task<Models.Events.DatingEvent> CancelAsync(long eventId, MockUser admin, CancellationToken cancellationToken = default)
    {
        var actor = await RequireUserAsync(admin.Id, cancellationToken);
        var datingEvent = await _db.DatingEvents
            .Include(item => item.Tickets)
            .Include(item => item.EventPlannerUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.EventType)
            .Include(item => item.EventTags)
            .ThenInclude(eventTag => eventTag.Tag)
            .FirstOrDefaultAsync(item => item.Id == eventId, cancellationToken)
            ?? throw new InvalidOperationException("رویداد مورد نظر پیدا نشد.");

        EnsureEventWriteAccess(actor, datingEvent);

        var beforeSnapshot = CreateEventSnapshot(datingEvent);
        var tickets = datingEvent.Cancel();
        var refundCount = 0;
        var refundTotal = 0m;

        foreach (var ticket in tickets.Where(item => item.IsRefunded))
        {
            var balance = await _db.BalanceAccounts.FirstOrDefaultAsync(item => item.UserId == ticket.UserId, cancellationToken);
            if (balance is null)
                continue;

            balance.Credit(ticket.Price, BalanceTransactionType.TicketRefund, $"Refund for {datingEvent.Title}", datingEvent.Id);
            refundCount++;
            refundTotal += ticket.Price;
        }

        await _auditLogger.LogAsync(new AuditLogEntry(
            actor.Id,
            "EventCancelled",
            "DatingEvent",
            datingEvent.Id.ToString(),
            JsonSerializer.Serialize(beforeSnapshot),
            JsonSerializer.Serialize(new { Event = CreateEventSnapshot(datingEvent), refundCount, refundTotal }),
            "رویداد لغو شد و بلیت‌های معتبر برگشت خوردند."), cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return (await GetEventAsync(datingEvent.Id, cancellationToken))!;
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
                DisplayName = profile?.DisplayName ?? $"کاربر {ticket.UserId}",
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
            actor.Id);

        var plannerBalance = await GetOrCreateBalanceAccountAsync(ticket.DatingEvent.EventPlannerUser, cancellationToken);
        var plannerIncome = ticket.Price * (100 - ticket.DatingEvent.EventPlannerCommissionPercent) / 100;
        if (plannerIncome > 0)
        {
            plannerBalance.DebitAllowNegative(
                plannerIncome,
                BalanceTransactionType.EventPlannerIncomeReversal,
                $"برگشت سهم برگزارکننده بابت بازگشت اضطراری بلیت {ticket.DatingEvent.Title}",
                ticket.DatingEventId,
                nameof(EventTicket),
                ticket.Id,
                actor.Id);
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
            $"بلیت کاربر «{DatabaseModelMapper.ResolveUserDisplayName(ticket.User)}» از رویداد «{ticket.DatingEvent.Title}» حذف و وجه آن برگشت داده شد."), cancellationToken);

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

    private async Task<User> RequireUserAsync(long userId, CancellationToken cancellationToken)
    {
        return await _db.Users
            .Include(item => item.Profile)
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException("کاربر جاری پیدا نشد.");
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
            throw new InvalidOperationException("کاربر انتخاب شده برگزارکننده نیست.");

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

        if (isOpen && !datingEvent.IsOpenForSell)
            datingEvent.OpenForSell();
        else if (!isOpen && datingEvent.IsOpenForSell)
            datingEvent.CloseForSell();
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
            datingEvent.MaleCapacity,
            datingEvent.FemaleCapacity,
            datingEvent.NumberOfLikesAllowed,
            datingEvent.MaleTicketPrice,
            datingEvent.FemaleTicketPrice,
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
            OperationalStatus = datingEvent.ResolveOperationalStatus(DateTime.UtcNow)
        };
    }
}
