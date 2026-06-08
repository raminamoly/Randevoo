using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Dashboard;

[Authorize(Policy = Policies.AdminOnly)]
public class IndexModel : PageModel
{
    private readonly IDashboardApiClient _dashboardApi;
    private readonly IEventsApiClient _eventsApi;
    private readonly CurrentSessionState _session;

    public IndexModel(IDashboardApiClient dashboardApi, IEventsApiClient eventsApi, CurrentSessionState session)
    {
        _dashboardApi = dashboardApi;
        _eventsApi = eventsApi;
        _session = session;
    }

    public DashboardStats Stats { get; private set; } = new();

    public IReadOnlyList<DatingEvent> Events { get; private set; } = Array.Empty<DatingEvent>();

    public bool IsRtl => _session.IsRtl;

    public async Task OnGetAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        Stats = await _dashboardApi.GetStatsAsync(current);
        Events = (await _eventsApi.GetEventsAsync(current)).Take(6).ToList();
    }

    public string GetOperationalStatusClass(EventOperationalStatus status) => DisplayFormatter.OperationalStatusClass(status);

    public string GetReviewStatusClass(EventReviewStatus status) => DisplayFormatter.ReviewStatusClass(status);

    public string GetMapMarkerStyle(DashboardMapPoint point)
    {
        var left = Clamp(((double)point.Longitude - 43d) / 22d * 100d, 5d, 95d);
        var top = Clamp((40d - (double)point.Latitude) / 16d * 100d, 7d, 93d);
        var size = Clamp(20d + point.EventCount * 6d + point.TicketCount * 1.5d, 26d, 74d);

        return string.Create(CultureInfo.InvariantCulture, $"--x:{left:0.##}%;--y:{top:0.##}%;--s:{size:0.##}px;");
    }

    private static double Clamp(double value, double min, double max) => Math.Min(Math.Max(value, min), max);
}
