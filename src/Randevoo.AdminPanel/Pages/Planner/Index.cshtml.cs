using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Services.ApiClients;

namespace Randevoo.AdminPanel.Pages.Planner;

[Authorize(Policy = Policies.AdminOnly)]
public class IndexModel : PageModel
{
    private readonly IUsersApiClient _usersApi;
    private readonly IPlannerProfilesApiClient _profilesApi;

    public IndexModel(IUsersApiClient usersApi, IPlannerProfilesApiClient profilesApi)
    {
        _usersApi = usersApi;
        _profilesApi = profilesApi;
    }

    public IReadOnlyList<PlannerProfileListItem> Planners { get; private set; } = Array.Empty<PlannerProfileListItem>();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var users = await _usersApi.GetUsersAsync(cancellationToken);
        var plannerUsers = users
            .Where(user => user.Role == AdminRole.EventPlanner)
            .OrderBy(user => user.FullName)
            .ToList();

        var planners = new List<PlannerProfileListItem>(plannerUsers.Count);
        foreach (var planner in plannerUsers)
        {
            var profile = await _profilesApi.GetByUserIdAsync(planner.Id, cancellationToken);
            planners.Add(new PlannerProfileListItem
            {
                Id = planner.Id,
                FullName = profile?.FullName ?? planner.FullName,
                Mobile = profile?.MobileNumber ?? planner.Mobile,
                Title = profile?.Title ?? "برگزارکننده راندوو",
                City = string.IsNullOrWhiteSpace(profile?.City) ? "تهران" : profile.City,
                PictureUrl = string.IsNullOrWhiteSpace(profile?.PictureUrl) ? "/images/logo.png" : profile.PictureUrl,
                IsActive = planner.IsActive,
                HasPendingChanges = profile?.HasPendingChanges ?? false,
                AverageRating = profile?.AverageRating ?? 0,
                HostedEventCount = profile?.HostedEventCount ?? 0,
                CompletedEventCount = profile?.CompletedEventCount ?? 0,
                CancelledEventCount = profile?.CancelledEventCount ?? 0
            });
        }

        Planners = planners;
    }
}

public sealed class PlannerProfileListItem
{
    public long Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Mobile { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string PictureUrl { get; set; } = "/images/logo.png";

    public bool IsActive { get; set; }

    public bool HasPendingChanges { get; set; }

    public decimal AverageRating { get; set; }

    public int HostedEventCount { get; set; }

    public int CompletedEventCount { get; set; }

    public int CancelledEventCount { get; set; }
}
