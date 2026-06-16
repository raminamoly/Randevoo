using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Common;

namespace Randevoo.AdminPanel.Pages.UserProfiles;

[Authorize(Policy = Policies.AdminOnly)]
public class IndexModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? CityId { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? GenderId { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? ZodiacSignId { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? IsActive { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? IsProfileComplete { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Sort { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public IActionResult OnGet()
    {
        return RedirectToPage("/Participants/Index", new
        {
            Search,
            CityId,
            Gender = GenderId switch
            {
                2 => "male",
                3 => "female",
                _ => null
            },
            ZodiacSignId,
            IsActive,
            ProfileStatus = IsProfileComplete switch
            {
                true => "completed",
                false => "pending",
                _ => null
            },
            Sort = Sort switch
            {
                "newest" => "registration-desc",
                "oldest" => "registration-asc",
                "last-activity" => "last-activity",
                "name" => "name",
                _ => null
            },
            PageNumber
        });
    }
}
