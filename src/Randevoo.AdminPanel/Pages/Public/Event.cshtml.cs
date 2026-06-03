using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.AdminPanel.Services.ApiClients;

namespace Randevoo.AdminPanel.Pages.Public;

[AllowAnonymous]
public class EventModel : PageModel
{
    private readonly IEventsApiClient _eventsApi;
    private readonly IPlannerProfilesApiClient _plannerProfilesApi;

    public EventModel(IEventsApiClient eventsApi, IPlannerProfilesApiClient plannerProfilesApi)
    {
        _eventsApi = eventsApi;
        _plannerProfilesApi = plannerProfilesApi;
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

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var @event = await _eventsApi.GetEventAsync(id);
        if (@event is null || !@event.IsVisibleToEndUsers)
        {
            return NotFound();
        }

        Event = @event;
        if (Guid.TryParse(@event.PlannerId, out var plannerId))
        {
            PlannerProfile = await _plannerProfilesApi.GetByUserIdAsync(plannerId);
        }

        return Page();
    }
}
