using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.Web.Services;

namespace Randevoo.Web.Pages.Events;

public class DetailsModel : PageModel
{
    private readonly EndUserEventsApiClient _eventsApiClient;

    public DetailsModel(EndUserEventsApiClient eventsApiClient)
    {
        _eventsApiClient = eventsApiClient;
    }

    public EndUserEventDetailsViewModel? EventDetails { get; private set; }
    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync(long id, CancellationToken cancellationToken)
    {
        try
        {
            EventDetails = await _eventsApiClient.GetDetailsAsync(id, cancellationToken);
            if (EventDetails is null)
                return NotFound();

            ViewData["Title"] = EventDetails.Title;
            return Page();
        }
        catch (Exception)
        {
            ErrorMessage = "دریافت جزئیات رویداد ممکن نیست.";
            return Page();
        }
    }
}
