using Randevoo.AdminPanel.Models.Events;

namespace Randevoo.AdminPanel.Services.ApiClients;

public interface IEventTypesApiClient
{
    Task<IReadOnlyList<EventTypeAdminItem>> GetEventTypesAsync(CancellationToken cancellationToken = default);

    Task<EventTypeAdminItem?> GetEventTypeAsync(long id, CancellationToken cancellationToken = default);

    Task<EventTypeAdminItem> UpsertEventTypeAsync(EventTypeEditorInput input, long? existingEventTypeId = null, CancellationToken cancellationToken = default);

    Task DeleteEventTypeAsync(long id, CancellationToken cancellationToken = default);
}
