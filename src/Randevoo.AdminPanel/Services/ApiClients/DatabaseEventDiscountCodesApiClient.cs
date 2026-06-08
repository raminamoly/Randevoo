using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.DiscountCodes;
using Randevoo.Application.Interfaces.Auditing;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Interfaces;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Services.ApiClients;

public sealed class DatabaseEventDiscountCodesApiClient : IEventDiscountCodesApiClient
{
    private readonly RandevooDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;

    public DatabaseEventDiscountCodesApiClient(RandevooDbContext db, IUnitOfWork unitOfWork, IAuditLogger auditLogger)
    {
        _db = db;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
    }

    public async Task<IReadOnlyList<EventDiscountCodeAdminItem>> GetDiscountCodesAsync(CancellationToken cancellationToken = default)
    {
        return await _db.EventDiscountCodes
            .IgnoreQueryFilters()
            .Where(item => !item.IsDeleted)
            .Include(item => item.DatingEvent)
            .OrderByDescending(item => item.CreatedAt)
            .Select(item => ToAdminItem(item, item.DatingEvent == null ? "همه رویدادها" : item.DatingEvent.Title))
            .ToListAsync(cancellationToken);
    }

    public async Task<EventDiscountCodeAdminItem?> GetDiscountCodeAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _db.EventDiscountCodes
            .IgnoreQueryFilters()
            .Where(item => !item.IsDeleted && item.Id == id)
            .Include(item => item.DatingEvent)
            .Select(item => ToAdminItem(item, item.DatingEvent == null ? "همه رویدادها" : item.DatingEvent.Title))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EventDiscountCodeUsageItem>> GetDiscountCodeUsageAsync(long id, CancellationToken cancellationToken = default)
    {
        var discountCode = await _db.EventDiscountCodes
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => !item.IsDeleted && item.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("کد تخفیف مورد نظر پیدا نشد.");

        var tickets = await _db.EventTickets
            .IgnoreQueryFilters()
            .Where(ticket => !ticket.IsDeleted && ticket.EventDiscountCodeId == discountCode.Id)
            .Include(ticket => ticket.User)
            .ThenInclude(user => user.Profile)
            .Include(ticket => ticket.DatingEvent)
            .OrderByDescending(ticket => ticket.CreatedAt)
            .Select(ticket => new EventDiscountCodeUsageItem
            {
                TicketId = ticket.Id,
                BuyerName = ticket.User.Profile != null && ticket.User.Profile.DisplayName != null
                    ? ticket.User.Profile.DisplayName
                    : ticket.User.MobileNumber,
                BuyerMobile = ticket.User.MobileNumber,
                BuyerGender = ticket.Gender,
                EventTitle = ticket.DatingEvent.Title,
                PurchasedAtUtc = ticket.CreatedAt,
                OriginalPrice = ticket.OriginalPrice,
                DiscountAmount = ticket.DiscountAmount,
                FinalPaidAmount = ticket.Price,
                CurrencyCode = ticket.CurrencyCode,
                IsRefunded = ticket.IsRefunded,
                IsRemoved = ticket.IsRemoved
            })
            .ToListAsync(cancellationToken);

        return tickets;
    }

    public async Task<EventDiscountCodeAdminItem> UpsertDiscountCodeAsync(EventDiscountCodeEditorInput input, MockUser actor, long? existingDiscountCodeId = null, CancellationToken cancellationToken = default)
    {
        var admin = await RequireAdminAsync(actor.Id, cancellationToken);
        var normalizedCode = (input.Code ?? string.Empty).Trim().ToUpperInvariant();
        var inputEventId = input.DatingEventId;
        var duplicateExists = await _db.EventDiscountCodes
            .IgnoreQueryFilters()
            .AnyAsync(item =>
                !item.IsDeleted
                && ((inputEventId == null && item.DatingEventId == null) || item.DatingEventId == inputEventId)
                && item.Id != (existingDiscountCodeId ?? 0)
                && item.Code == normalizedCode, cancellationToken);
        if (duplicateExists)
            throw new InvalidOperationException("این کد تخفیف برای رویداد انتخاب شده قبلا ثبت شده است.");

        var datingEvent = input.DatingEventId is long eventId
            ? await _db.DatingEvents
                .Include(item => item.DiscountCodes)
                .FirstOrDefaultAsync(item => item.Id == eventId, cancellationToken)
                ?? throw new InvalidOperationException("رویداد انتخاب شده پیدا نشد.")
            : null;

        EventDiscountCode discountCode;
        object? beforeSnapshot = null;
        if (existingDiscountCodeId is long id)
        {
            discountCode = await _db.EventDiscountCodes
                .Include(item => item.DatingEvent)
                .FirstOrDefaultAsync(item => item.Id == id && !item.IsDeleted, cancellationToken)
                ?? throw new InvalidOperationException("کد تخفیف مورد نظر پیدا نشد.");

            beforeSnapshot = CreateSnapshot(discountCode, discountCode.DatingEvent?.Title ?? "همه رویدادها");
            discountCode.UpdateDetails(
                normalizedCode,
                input.GenderScope,
                input.DiscountType,
                input.Value,
                input.StartsAtUtc.ToUniversalTime(),
                input.EndsAtUtc.ToUniversalTime(),
                input.MaxUsageCount,
                input.Title,
                input.Description);
            discountCode.SetActive(input.IsActive);

            if (discountCode.DatingEventId != datingEvent?.Id)
                throw new InvalidOperationException("تغییر رویداد برای کد تخفیف موجود پشتیبانی نمی شود.");
        }
        else
        {
            discountCode = datingEvent is not null
                ? datingEvent.AddDiscountCode(
                    normalizedCode,
                    input.GenderScope,
                    input.DiscountType,
                    input.Value,
                    input.StartsAtUtc.ToUniversalTime(),
                    input.EndsAtUtc.ToUniversalTime(),
                    input.MaxUsageCount,
                    input.IsActive,
                    input.Title,
                    input.Description)
                : new EventDiscountCode(
                    null,
                    normalizedCode,
                    input.GenderScope,
                    input.DiscountType,
                    input.Value,
                    input.StartsAtUtc.ToUniversalTime(),
                    input.EndsAtUtc.ToUniversalTime(),
                    input.MaxUsageCount,
                    input.IsActive,
                    input.Title,
                    input.Description);

            _db.EventDiscountCodes.Add(discountCode);
        }

        if (existingDiscountCodeId is null)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogger.LogAsync(new AuditLogEntry(
            admin.Id,
            existingDiscountCodeId is null ? "ایجاد کد تخفیف" : "ویرایش کد تخفیف",
            "EventDiscountCode",
            discountCode.Id.ToString(),
            beforeSnapshot is null ? null : JsonSerializer.Serialize(beforeSnapshot),
            JsonSerializer.Serialize(CreateSnapshot(discountCode, datingEvent?.Title ?? "همه رویدادها")),
            $"کد تخفیف «{discountCode.Code}» برای «{datingEvent?.Title ?? "همه رویدادها"}» ذخیره شد."), cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return (await GetDiscountCodeAsync(discountCode.Id, cancellationToken))!;
    }

    public async Task SetDiscountCodeActiveAsync(long id, MockUser actor, bool isActive, CancellationToken cancellationToken = default)
    {
        var admin = await RequireAdminAsync(actor.Id, cancellationToken);
        var discountCode = await _db.EventDiscountCodes
            .Include(item => item.DatingEvent)
            .FirstOrDefaultAsync(item => item.Id == id && !item.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("کد تخفیف مورد نظر پیدا نشد.");

        discountCode.SetActive(isActive);
        await _auditLogger.LogAsync(new AuditLogEntry(
            admin.Id,
            isActive ? "فعال سازی کد تخفیف" : "غیرفعال سازی کد تخفیف",
            "EventDiscountCode",
            discountCode.Id.ToString(),
            null,
            JsonSerializer.Serialize(new { discountCode.IsActive }),
            $"کد تخفیف «{discountCode.Code}» برای «{discountCode.DatingEvent?.Title ?? "همه رویدادها"}» {(isActive ? "فعال" : "غیرفعال")} شد."), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<User> RequireAdminAsync(long userId, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .Include(item => item.Profile)
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException("حساب جاری پیدا نشد.");

        if (user.Role != UserRole.Admin)
            throw new InvalidOperationException("فقط مدیر می تواند این عملیات را انجام دهد.");

        return user;
    }

    private static EventDiscountCodeAdminItem ToAdminItem(EventDiscountCode item, string eventTitle)
    {
        return new EventDiscountCodeAdminItem
        {
            Id = item.Id,
            DatingEventId = item.DatingEventId,
            EventTitle = eventTitle,
            Code = item.Code,
            Title = item.Title,
            Description = item.Description,
            GenderScope = item.GenderScope,
            DiscountType = item.DiscountType,
            Value = item.Value,
            StartsAtUtc = DateTime.SpecifyKind(item.StartsAtUtc, DateTimeKind.Utc),
            EndsAtUtc = DateTime.SpecifyKind(item.EndsAtUtc, DateTimeKind.Utc),
            MaxUsageCount = item.MaxUsageCount,
            UsedCount = item.UsedCount,
            IsActive = item.IsActive
        };
    }

    private static object CreateSnapshot(EventDiscountCode item, string eventTitle) => new
    {
        item.DatingEventId,
        EventTitle = eventTitle,
        item.Code,
        item.Title,
        item.Description,
        item.GenderScope,
        item.DiscountType,
        item.Value,
        item.StartsAtUtc,
        item.EndsAtUtc,
        item.MaxUsageCount,
        item.UsedCount,
        item.IsActive
    };
}
