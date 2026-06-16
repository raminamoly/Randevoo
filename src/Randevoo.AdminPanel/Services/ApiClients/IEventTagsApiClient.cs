using Randevoo.AdminPanel.Models.Events;

namespace Randevoo.AdminPanel.Services.ApiClients;

public interface IEventTagsApiClient
{
    Task<IReadOnlyList<TagOption>> GetActiveTagsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InterestOption>> GetInterestsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TagAdminItem>> GetTagsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InterestTagMappingListItem>> GetInterestTagMappingsAsync(CancellationToken cancellationToken = default);

    Task<InterestTagMappingListItem?> GetInterestTagMappingAsync(long id, CancellationToken cancellationToken = default);

    Task<InterestTagMappingListItem> UpsertInterestTagMappingAsync(InterestTagMappingInput input, long? existingMappingId = null, CancellationToken cancellationToken = default);

    Task DeleteInterestTagMappingAsync(long id, CancellationToken cancellationToken = default);

    Task<TagAdminItem?> GetTagAsync(long id, CancellationToken cancellationToken = default);

    Task<TagAdminItem> UpsertTagAsync(TagEditorInput input, long? existingTagId = null, CancellationToken cancellationToken = default);

    Task DeleteTagAsync(long id, CancellationToken cancellationToken = default);
}
