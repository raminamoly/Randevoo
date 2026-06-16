using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Public;

[AllowAnonymous]
public class EventModel : PageModel
{
    private readonly IEventsApiClient _eventsApi;
    private readonly IPlannerProfilesApiClient _plannerProfilesApi;
    private readonly CurrentSessionState _session;

    public EventModel(IEventsApiClient eventsApi, IPlannerProfilesApiClient plannerProfilesApi, CurrentSessionState session)
    {
        _eventsApi = eventsApi;
        _plannerProfilesApi = plannerProfilesApi;
        _session = session;
    }

    public DatingEvent Event { get; private set; } = new();

    public PlannerProfileViewModel? PlannerProfile { get; private set; }

    public IReadOnlyList<string> EventImages => new[] { Event.Live.Image1, Event.Live.Image2, Event.Live.Image3 }
        .Where(image => !string.IsNullOrWhiteSpace(image))
        .Cast<string>()
        .ToList();

    public EventImageCarouselModel EventImageCarousel => new()
    {
        CarouselId = $"public-event-slider-{Event.Id}",
        Images = EventImages
    };

    public bool IsPreviewMode { get; private set; }

    public async Task<IActionResult> OnGetAsync(long id)
    {
        var @event = await _eventsApi.GetEventAsync(id);
        if (@event is null)
        {
            return NotFound();
        }

        IsPreviewMode = CanPreview(@event) && !@event.IsVisibleToEndUsers;
        if (!@event.IsVisibleToEndUsers && !IsPreviewMode)
        {
            return NotFound();
        }

        Event = @event;
        PlannerProfile = await _plannerProfilesApi.GetByUserIdAsync(@event.PlannerUserId);

        return Page();
    }

    private bool CanPreview(DatingEvent datingEvent)
    {
        var currentUser = _session.CurrentUser;
        if (currentUser is null)
            return false;

        return currentUser.Role == AdminRole.Admin
            || (currentUser.Role == AdminRole.EventPlanner && datingEvent.PlannerUserId == currentUser.Id);
    }
}
