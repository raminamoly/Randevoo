using Randevoo.ControlCenter.Models.Auth;
using Randevoo.ControlCenter.Models.EventPlanners;
using Randevoo.ControlCenter.Models.Events;

namespace Randevoo.ControlCenter.Services.ApiClients;

public interface IEventsApiClient
{
    Task<IReadOnlyList<EventSummary>> GetEventsAsync(ControlCenterRole role, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EventPlannerSummary>> GetEventPlannersAsync(CancellationToken cancellationToken = default);
}
