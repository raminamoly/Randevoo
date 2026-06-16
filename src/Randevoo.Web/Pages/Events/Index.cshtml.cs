using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.Web.Services;

namespace Randevoo.Web.Pages.Events;

public class IndexModel : PageModel
{
    private readonly EndUserEventsApiClient _eventsApiClient;

    public IndexModel(EndUserEventsApiClient eventsApiClient)
    {
        _eventsApiClient = eventsApiClient;
    }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public long? CityId { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool IncludeOnline { get; set; } = true;

    [BindProperty(SupportsGet = true)]
    public bool IncludeInPerson { get; set; } = true;

    [BindProperty(SupportsGet = true)]
    public bool UseAge { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? Age { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? EducationLevelId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Sort { get; set; } = "Recommended";

    public EndUserEventPageViewModel Events { get; private set; } = EndUserEventPageViewModel.Empty(1, 12);
    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            Events = await _eventsApiClient.ListAsync(new EndUserEventListRequest(
                Math.Max(1, PageNumber),
                12,
                CityId,
                IncludeOnline,
                IncludeInPerson,
                null,
                UseAge ? Age : null,
                EducationLevelId,
                string.IsNullOrWhiteSpace(Sort) ? "Recommended" : Sort), cancellationToken);
        }
        catch (Exception)
        {
            ErrorMessage = "در حال حاضر دریافت رویدادها ممکن نیست.";
            Events = EndUserEventPageViewModel.Empty(Math.Max(1, PageNumber), 12);
        }
    }
}
