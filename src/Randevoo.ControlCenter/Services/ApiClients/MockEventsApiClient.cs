using Randevoo.ControlCenter.Models.Auth;
using Randevoo.ControlCenter.Models.EventPlanners;
using Randevoo.ControlCenter.Models.Events;
using Randevoo.ControlCenter.Services.MockData;

namespace Randevoo.ControlCenter.Services.ApiClients;

public sealed class MockEventsApiClient(ControlCenterMockData data) : IEventsApiClient
{
    public Task<IReadOnlyList<EventSummary>> GetEventsAsync(ControlCenterRole role, CancellationToken cancellationToken = default)
    {
        var events = role == ControlCenterRole.Admin
            ? data.Events
            : data.Events.Where(item => item.PlannerName == "Nava Events").ToArray();

        return Task.FromResult<IReadOnlyList<EventSummary>>(events);
    }

    public Task<IReadOnlyList<EventPlannerSummary>> GetEventPlannersAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(data.EventPlanners);
    }
}
