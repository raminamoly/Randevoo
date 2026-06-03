using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Services.MockData;

namespace Randevoo.AdminPanel.Services.ApiClients;

public sealed class MockEventsApiClient : IEventsApiClient
{
    private readonly AdminPanelStore _store;

    public MockEventsApiClient(AdminPanelStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyList<DatingEvent>> GetEventsAsync(MockUser currentUser, CancellationToken cancellationToken = default)
    {
        var events = _store.GetEvents();
        if (currentUser.Role == AdminRole.EventPlanner)
        {
            events = events.Where(item => item.PlannerId == currentUser.Id.ToString()).ToList();
        }

        return Task.FromResult(events);
    }

    public Task<DatingEvent?> GetEventAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.FindEvent(id));

    public Task<DatingEvent> SaveEventAsync(EventDraftInput input, MockUser actor, Guid? existingEventId = null, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.UpsertEvent(input, actor, existingEventId));

    public Task<DatingEvent> ApproveAsync(Guid eventId, MockUser admin, decimal? commissionPercent = null, string? note = null, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.ApproveEvent(eventId, admin, commissionPercent, note));

    public Task<DatingEvent> RejectAsync(Guid eventId, MockUser admin, string note, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.RejectEvent(eventId, admin, note));

    public Task<DatingEvent> SetCommissionAsync(Guid eventId, MockUser admin, decimal commissionPercent, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.SetCommission(eventId, admin, commissionPercent));

    public Task<DatingEvent> ToggleSaleAsync(Guid eventId, MockUser admin, bool isOpen, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.ToggleSale(eventId, admin, isOpen));

    public Task<DatingEvent> CancelAsync(Guid eventId, MockUser admin, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.Cancel(eventId, admin));
}

