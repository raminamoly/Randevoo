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

    public Task<EventSummary?> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(data.GetEvent(eventId));
    }

    public Task<EventSummary> CreatePlannerEventAsync(EventDraftInput input, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(data.AddPlannerEvent(input));
    }

    public Task<EventSummary?> UpdatePlannerEventAsync(Guid eventId, EventDraftInput input, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(data.UpdatePlannerEvent(eventId, input));
    }

    public Task<EventSummary?> ConfirmEventAsync(Guid eventId, decimal commissionPercent, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(data.ConfirmEvent(eventId, commissionPercent));
    }

    public Task<EventSummary?> SetCommissionAsync(Guid eventId, decimal commissionPercent, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(data.SetCommission(eventId, commissionPercent));
    }

    public Task<EventSummary?> OpenForSellAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(data.OpenForSell(eventId));
    }

    public Task<EventSummary?> CloseForSellAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(data.CloseForSell(eventId));
    }

    public Task<EventSummary?> CancelEventAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(data.CancelEvent(eventId));
    }

    public Task<IReadOnlyList<EventPlannerSummary>> GetEventPlannersAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(data.EventPlanners);
    }
}
