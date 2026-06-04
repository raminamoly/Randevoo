using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.Domain.Interfaces;
using Randevoo.Infrastructure.Data;
using DomainEventType = Randevoo.Domain.Entities.EventType;

namespace Randevoo.AdminPanel.Services.ApiClients;

public sealed class DatabaseEventTypesApiClient : IEventTypesApiClient
{
    private readonly RandevooDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    public DatabaseEventTypesApiClient(RandevooDbContext db, IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<EventTypeAdminItem>> GetEventTypesAsync(CancellationToken cancellationToken = default)
    {
        return await _db.EventTypes
            .IgnoreQueryFilters()
            .Where(item => !item.IsDeleted)
            .OrderBy(item => item.Name)
            .Select(item => new EventTypeAdminItem
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                IsActive = item.IsActive,
                EventUsageCount = _db.DatingEvents.Count(evt => evt.EventTypeId == item.Id)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<EventTypeAdminItem?> GetEventTypeAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _db.EventTypes
            .IgnoreQueryFilters()
            .Where(item => !item.IsDeleted && item.Id == id)
            .Select(item => new EventTypeAdminItem
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                IsActive = item.IsActive,
                EventUsageCount = _db.DatingEvents.Count(evt => evt.EventTypeId == item.Id)
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<EventTypeAdminItem> UpsertEventTypeAsync(EventTypeEditorInput input, long? existingEventTypeId = null, CancellationToken cancellationToken = default)
    {
        var normalizedName = (input.Name ?? string.Empty).Trim();
        var normalizedDescription = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim();

        var duplicateExists = await _db.EventTypes
            .IgnoreQueryFilters()
            .AnyAsync(item =>
                !item.IsDeleted
                && item.Id != (existingEventTypeId ?? 0)
                && item.Name == normalizedName, cancellationToken);
        if (duplicateExists)
            throw new InvalidOperationException("نوع رویداد با این نام قبلاً ثبت شده است.");

        DomainEventType eventType;
        if (existingEventTypeId is long id)
        {
            eventType = await _db.EventTypes
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(item => !item.IsDeleted && item.Id == id, cancellationToken)
                ?? throw new InvalidOperationException("نوع رویداد مورد نظر پیدا نشد.");

            eventType.Update(normalizedName, normalizedDescription, input.IsActive);
            _db.EventTypes.Update(eventType);
        }
        else
        {
            eventType = new DomainEventType(normalizedName, normalizedDescription);
            eventType.Update(normalizedName, normalizedDescription, input.IsActive);
            _db.EventTypes.Add(eventType);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return (await GetEventTypeAsync(eventType.Id, cancellationToken))!;
    }

    public async Task DeleteEventTypeAsync(long id, CancellationToken cancellationToken = default)
    {
        var eventType = await _db.EventTypes
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => !item.IsDeleted && item.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("نوع رویداد مورد نظر پیدا نشد.");

        var isInUse = await _db.DatingEvents.AnyAsync(item => item.EventTypeId == id, cancellationToken);
        if (isInUse)
            throw new InvalidOperationException("این نوع رویداد در رویدادهای ثبت شده استفاده شده است و قابل حذف نیست.");

        eventType.SoftDelete();
        _db.EventTypes.Update(eventType);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
