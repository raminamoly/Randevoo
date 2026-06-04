using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Services.ApiClients;

public sealed class DatabaseEventTagsApiClient : IEventTagsApiClient
{
    private readonly RandevooDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    public DatabaseEventTagsApiClient(RandevooDbContext db, IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<TagOption>> GetActiveTagsAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Tags
            .Where(item => item.IsActive)
            .OrderBy(item => item.Name)
            .Select(item => new TagOption { Id = item.Id, Name = item.Name })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TagAdminItem>> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Tags
            .IgnoreQueryFilters()
            .Where(item => !item.IsDeleted)
            .OrderBy(item => item.Name)
            .Select(item => new TagAdminItem
            {
                Id = item.Id,
                Name = item.Name,
                IsActive = item.IsActive,
                EventUsageCount = _db.EventTags.Count(eventTag => eventTag.TagId == item.Id)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<TagAdminItem?> GetTagAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _db.Tags
            .IgnoreQueryFilters()
            .Where(item => !item.IsDeleted && item.Id == id)
            .Select(item => new TagAdminItem
            {
                Id = item.Id,
                Name = item.Name,
                IsActive = item.IsActive,
                EventUsageCount = _db.EventTags.Count(eventTag => eventTag.TagId == item.Id)
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TagAdminItem> UpsertTagAsync(TagEditorInput input, long? existingTagId = null, CancellationToken cancellationToken = default)
    {
        var normalizedName = (input.Name ?? string.Empty).Trim();
        var duplicateExists = await _db.Tags
            .IgnoreQueryFilters()
            .AnyAsync(item =>
                !item.IsDeleted
                && item.Id != (existingTagId ?? 0)
                && item.Name == normalizedName, cancellationToken);
        if (duplicateExists)
            throw new InvalidOperationException("تگی با این نام قبلاً ثبت شده است.");

        Tag tag;
        if (existingTagId is long id)
        {
            tag = await _db.Tags
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(item => !item.IsDeleted && item.Id == id, cancellationToken)
                ?? throw new InvalidOperationException("تگ مورد نظر پیدا نشد.");

            tag.Update(normalizedName, input.IsActive);
            _db.Tags.Update(tag);
        }
        else
        {
            tag = new Tag(normalizedName);
            tag.Update(normalizedName, input.IsActive);
            _db.Tags.Add(tag);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return (await GetTagAsync(tag.Id, cancellationToken))!;
    }

    public async Task DeleteTagAsync(long id, CancellationToken cancellationToken = default)
    {
        var tag = await _db.Tags
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => !item.IsDeleted && item.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("تگ مورد نظر پیدا نشد.");

        var isInUse = await _db.EventTags.AnyAsync(item => item.TagId == id, cancellationToken);
        if (isInUse)
            throw new InvalidOperationException("این تگ در رویدادها استفاده شده و قابل حذف نیست. می توانید آن را غیرفعال کنید.");

        tag.SoftDelete();
        _db.Tags.Update(tag);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
