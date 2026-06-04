using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Events;

namespace Randevoo.AdminPanel.Services.ApiClients;

public interface IEventsApiClient
{
    Task<IReadOnlyList<EventTypeOption>> GetEventTypesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DatingEvent>> GetEventsAsync(MockUser currentUser, CancellationToken cancellationToken = default);

    Task<DatingEvent?> GetEventAsync(long id, CancellationToken cancellationToken = default);

    Task<DatingEvent> SaveEventAsync(EventDraftInput input, MockUser actor, long? existingEventId = null, long? assignedPlannerId = null, CancellationToken cancellationToken = default);

    Task<DatingEvent> ApproveAsync(long eventId, MockUser admin, decimal? commissionPercent = null, string? note = null, CancellationToken cancellationToken = default);

    Task<DatingEvent> RejectAsync(long eventId, MockUser admin, string note, CancellationToken cancellationToken = default);

    Task<DatingEvent> SetCommissionAsync(long eventId, MockUser admin, decimal commissionPercent, CancellationToken cancellationToken = default);

    Task<DatingEvent> ToggleSaleAsync(long eventId, MockUser admin, bool isOpen, CancellationToken cancellationToken = default);

    Task<DatingEvent> CancelAsync(long eventId, MockUser admin, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EventTicketBuyerItem>> GetEventTicketBuyersAsync(long eventId, MockUser actor, CancellationToken cancellationToken = default);

    Task EmergencyRefundTicketAsync(long eventId, long ticketId, MockUser admin, string reason, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EventSmsRequest>> GetSmsRequestsAsync(long eventId, MockUser actor, CancellationToken cancellationToken = default);

    Task RequestSmsAsync(long eventId, MockUser actor, string message, DateTimeOffset? plannedSendAtUtc = null, CancellationToken cancellationToken = default);

    Task ApproveSmsRequestAsync(long eventId, long requestId, MockUser admin, string approvedMessage, DateTimeOffset? plannedSendAtUtc = null, string? note = null, CancellationToken cancellationToken = default);

    Task RejectSmsRequestAsync(long eventId, long requestId, MockUser admin, string note, CancellationToken cancellationToken = default);
}
