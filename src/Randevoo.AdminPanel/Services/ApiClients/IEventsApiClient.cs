using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Events;

namespace Randevoo.AdminPanel.Services.ApiClients;

public interface IEventsApiClient
{
    Task<IReadOnlyList<DatingEvent>> GetEventsAsync(MockUser currentUser, CancellationToken cancellationToken = default);

    Task<DatingEvent?> GetEventAsync(Guid id, CancellationToken cancellationToken = default);

    Task<DatingEvent> SaveEventAsync(EventDraftInput input, MockUser actor, Guid? existingEventId = null, CancellationToken cancellationToken = default);

    Task<DatingEvent> ApproveAsync(Guid eventId, MockUser admin, decimal? commissionPercent = null, string? note = null, CancellationToken cancellationToken = default);

    Task<DatingEvent> RejectAsync(Guid eventId, MockUser admin, string note, CancellationToken cancellationToken = default);

    Task<DatingEvent> SetCommissionAsync(Guid eventId, MockUser admin, decimal commissionPercent, CancellationToken cancellationToken = default);

    Task<DatingEvent> ToggleSaleAsync(Guid eventId, MockUser admin, bool isOpen, CancellationToken cancellationToken = default);

    Task<DatingEvent> CancelAsync(Guid eventId, MockUser admin, CancellationToken cancellationToken = default);

    Task SendSmsAsync(Guid eventId, MockUser actor, string message, CancellationToken cancellationToken = default);
}
