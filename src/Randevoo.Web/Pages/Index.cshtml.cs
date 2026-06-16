using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.Web.Services;

namespace Randevoo.Web.Pages;

public class IndexModel : PageModel
{
    private readonly EndUserEventsApiClient _eventsApiClient;

    public IndexModel(EndUserEventsApiClient eventsApiClient)
    {
        _eventsApiClient = eventsApiClient;
    }

    public EndUserEventPageViewModel Events { get; private set; } = EndUserEventPageViewModel.Empty(1, 8);
    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            Events = await _eventsApiClient.ListAsync(new EndUserEventListRequest(
                1,
                8,
                null,
                true,
                true,
                null,
                null,
                null,
                "Soonest"), cancellationToken);
        }
        catch (Exception)
        {
            ErrorMessage = "در حال حاضر دریافت رویدادها ممکن نیست.";
            Events = EndUserEventPageViewModel.Empty(1, 8);
        }
    }
}
