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

    public async Task<IReadOnlyList<InterestOption>> GetInterestsAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Interests
            .IgnoreQueryFilters()
            .Where(item => !item.IsDeleted)
            .OrderByDescending(item => item.UsageCount)
            .ThenBy(item => item.Name)
            .Select(item => new InterestOption
            {
                Id = item.Id,
                Name = item.Name,
                Category = item.Category,
                UsageCount = item.UsageCount
            })
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

    public async Task<IReadOnlyList<InterestTagMappingListItem>> GetInterestTagMappingsAsync(CancellationToken cancellationToken = default)
    {
        return await _db.InterestTagMappings
            .IgnoreQueryFilters()
            .Where(item => !item.IsDeleted && !item.Interest.IsDeleted && !item.Tag.IsDeleted)
            .OrderBy(item => item.Interest.Name)
            .ThenByDescending(item => item.RelevanceWeight)
            .ThenBy(item => item.Tag.Name)
            .Select(item => new InterestTagMappingListItem
            {
                Id = item.Id,
                InterestId = item.InterestId,
                InterestName = item.Interest.Name,
                InterestCategory = item.Interest.Category,
                InterestUsageCount = item.Interest.UsageCount,
                TagId = item.TagId,
                TagName = item.Tag.Name,
                RelevanceWeight = item.RelevanceWeight,
                IsActive = item.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<InterestTagMappingListItem?> GetInterestTagMappingAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _db.InterestTagMappings
            .IgnoreQueryFilters()
            .Where(item => !item.IsDeleted && item.Id == id && !item.Interest.IsDeleted && !item.Tag.IsDeleted)
            .Select(item => new InterestTagMappingListItem
            {
                Id = item.Id,
                InterestId = item.InterestId,
                InterestName = item.Interest.Name,
                InterestCategory = item.Interest.Category,
                InterestUsageCount = item.Interest.UsageCount,
                TagId = item.TagId,
                TagName = item.Tag.Name,
                RelevanceWeight = item.RelevanceWeight,
                IsActive = item.IsActive
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

    public async Task<InterestTagMappingListItem> UpsertInterestTagMappingAsync(InterestTagMappingInput input, long? existingMappingId = null, CancellationToken cancellationToken = default)
    {
        if (input.InterestId is not long interestId)
            throw new InvalidOperationException("علاقه را انتخاب کنید.");
        if (input.TagId is not long tagId)
            throw new InvalidOperationException("تگ را انتخاب کنید.");

        var interest = await _db.Interests
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => !item.IsDeleted && item.Id == interestId, cancellationToken)
            ?? throw new InvalidOperationException("علاقه مورد نظر پیدا نشد.");

        var tag = await _db.Tags
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => !item.IsDeleted && item.Id == tagId, cancellationToken)
            ?? throw new InvalidOperationException("تگ مورد نظر پیدا نشد.");

        var duplicateExists = await _db.InterestTagMappings
            .IgnoreQueryFilters()
            .AnyAsync(item =>
                !item.IsDeleted
                && item.Id != (existingMappingId ?? 0)
                && item.InterestId == interestId
                && item.TagId == tagId, cancellationToken);
        if (duplicateExists)
            throw new InvalidOperationException("این علاقه قبلاً به همین تگ وصل شده است.");

        InterestTagMapping mapping;
        if (existingMappingId is long id)
        {
            mapping = await _db.InterestTagMappings
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(item => !item.IsDeleted && item.Id == id, cancellationToken)
                ?? throw new InvalidOperationException("نگاشت مورد نظر پیدا نشد.");

            if (mapping.InterestId != interestId || mapping.TagId != tagId)
                throw new InvalidOperationException("برای تغییر علاقه یا تگ، نگاشت قبلی را حذف و نگاشت جدید ثبت کنید.");

            mapping.UpdateWeight(input.RelevanceWeight);
            mapping.SetActive(input.IsActive);
            _db.InterestTagMappings.Update(mapping);
        }
        else
        {
            mapping = new InterestTagMapping(interest, tag, input.RelevanceWeight, input.IsActive);
            _db.InterestTagMappings.Add(mapping);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return (await GetInterestTagMappingAsync(mapping.Id, cancellationToken))!;
    }

    public async Task DeleteInterestTagMappingAsync(long id, CancellationToken cancellationToken = default)
    {
        var mapping = await _db.InterestTagMappings
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => !item.IsDeleted && item.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("نگاشت مورد نظر پیدا نشد.");

        mapping.SoftDelete();
        _db.InterestTagMappings.Update(mapping);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
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
