using Randevoo.ControlCenter.Models.Auth;
using Randevoo.ControlCenter.Models.EventPlanners;
using Randevoo.ControlCenter.Models.Events;

namespace Randevoo.ControlCenter.Services.ApiClients;

public interface IEventsApiClient
{
    Task<IReadOnlyList<EventSummary>> GetEventsAsync(ControlCenterRole role, CancellationToken cancellationToken = default);

    Task<EventSummary?> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default);

    Task<EventSummary> CreatePlannerEventAsync(EventDraftInput input, CancellationToken cancellationToken = default);

    Task<EventSummary?> UpdatePlannerEventAsync(Guid eventId, EventDraftInput input, CancellationToken cancellationToken = default);

    Task<EventSummary?> ConfirmEventAsync(Guid eventId, decimal commissionPercent, CancellationToken cancellationToken = default);

    Task<EventSummary?> SetCommissionAsync(Guid eventId, decimal commissionPercent, CancellationToken cancellationToken = default);

    Task<EventSummary?> OpenForSellAsync(Guid eventId, CancellationToken cancellationToken = default);

    Task<EventSummary?> CloseForSellAsync(Guid eventId, CancellationToken cancellationToken = default);

    Task<EventSummary?> CancelEventAsync(Guid eventId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EventPlannerSummary>> GetEventPlannersAsync(CancellationToken cancellationToken = default);
}
