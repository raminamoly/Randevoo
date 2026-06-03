using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Events;

[Authorize(Policy = Policies.AdminOrPlanner)]
public class DetailsModel : PageModel
{
    private readonly IEventsApiClient _eventsApi;
    private readonly CurrentSessionState _session;

    public DetailsModel(IEventsApiClient eventsApi, CurrentSessionState session)
    {
        _eventsApi = eventsApi;
        _session = session;
    }

    public DatingEvent Event { get; private set; } = new();

    public IReadOnlyList<string> PendingChanges { get; private set; } = Array.Empty<string>();

    public bool IsAdmin => _session.CurrentUser?.Role is AdminRole.Admin or AdminRole.SupportTeam;

    public bool IsRtl => _session.IsRtl;

    public string StatusClass => EditModel.GetStatusClass(Event.Status);

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var @event = await _eventsApi.GetEventAsync(id);
        if (@event is null)
        {
            return NotFound();
        }

        Event = @event;
        PendingChanges = BuildChanges(@event);
        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(Guid id, decimal commissionPercent, string? note)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("Current user was not resolved.");
        await _eventsApi.ApproveAsync(id, current, commissionPercent, note);
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRejectAsync(Guid id, string note)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("Current user was not resolved.");
        await _eventsApi.RejectAsync(id, current, note);
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostOpenSaleAsync(Guid id)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("Current user was not resolved.");
        await _eventsApi.ToggleSaleAsync(id, current, true);
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostCloseSaleAsync(Guid id)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("Current user was not resolved.");
        await _eventsApi.ToggleSaleAsync(id, current, false);
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostCancelAsync(Guid id)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("Current user was not resolved.");
        await _eventsApi.CancelAsync(id, current);
        return RedirectToPage(new { id });
    }

    private static IReadOnlyList<string> BuildChanges(DatingEvent @event)
    {
        if (@event.Pending is null)
        {
            return Array.Empty<string>();
        }

        var current = @event.Live;
        var next = @event.Pending.Draft;
        var changes = new List<string>();

        AddChange(changes, "Title", current.Title, next.Title);
        AddChange(changes, "Country", current.Country, next.Country);
        AddChange(changes, "City", current.City, next.City);
        AddChange(changes, "Region", current.Region, next.Region);
        AddChange(changes, "Venue", current.VenueName, next.VenueName);
        AddChange(changes, "Address", current.Address, next.Address);
        AddChange(changes, "Event type", current.EventType.ToString(), next.EventType.ToString());
        AddChange(changes, "Male age range", current.AgeRangeForMale, next.AgeRangeForMale);
        AddChange(changes, "Female age range", current.AgeRangeForFemale, next.AgeRangeForFemale);
        AddChange(changes, "Ticket price", current.TicketPrice.ToString("N0"), next.TicketPrice.ToString("N0"));
        AddChange(changes, "Capacity male", current.CapacityMale.ToString(), next.CapacityMale.ToString());
        AddChange(changes, "Capacity female", current.CapacityFemale.ToString(), next.CapacityFemale.ToString());
        AddChange(changes, "Chat limit", current.ChatLimit.ToString(), next.ChatLimit.ToString());
        AddChange(changes, "Open for sale", current.IsOpenForSell.ToString(), next.IsOpenForSell.ToString());
        AddChange(changes, "Start", PersianDateFormatter.Format(current.StartAtUtc, true), PersianDateFormatter.Format(next.StartAtUtc, true));
        AddChange(changes, "End", PersianDateFormatter.Format(current.EndAtUtc, true), PersianDateFormatter.Format(next.EndAtUtc, true));

        if (!string.Equals(current.Image1, next.Image1, StringComparison.Ordinal))
        {
            changes.Add("Image 1 changed.");
        }
        if (!string.Equals(current.Image2, next.Image2, StringComparison.Ordinal))
        {
            changes.Add("Image 2 changed.");
        }
        if (!string.Equals(current.Image3, next.Image3, StringComparison.Ordinal))
        {
            changes.Add("Image 3 changed.");
        }

        return changes.Count == 0 ? new[] { "No visible field changed, but the planner still submitted a review request." } : changes;
    }

    private static void AddChange(List<string> changes, string label, string oldValue, string newValue)
    {
        if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
        {
            changes.Add($"{label}: {oldValue} -> {newValue}");
        }
    }
}

