using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Dashboard;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Dashboard;

[Authorize(Policy = Policies.AdminOnly)]
public class SalesModel : PageModel
{
    private readonly IAdminAnalyticsApiClient _analyticsApi;
    private readonly CurrentSessionState _session;

    public SalesModel(IAdminAnalyticsApiClient analyticsApi, CurrentSessionState session)
    {
        _analyticsApi = analyticsApi;
        _session = session;
    }

    [BindProperty(SupportsGet = true)]
    public string RangeKey { get; set; } = DashboardDateRange.Default;

    public DashboardDateRangeValue Range { get; private set; } = DashboardDateRange.Resolve(DashboardDateRange.Default);

    public SalesDashboardReport Report { get; private set; } = new();

    public DashboardFilterViewModel FilterModel => new()
    {
        Title = "داشبورد فروش",
        Description = "بررسی خرید بلیت، نوع فروش و رویدادهای پرفروش.",
        CurrentRangeKey = Range.Key,
        PagePath = "/Dashboard/Sales"
    };

    public async Task OnGetAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        Range = DashboardDateRange.Resolve(RangeKey);
        Report = await _analyticsApi.GetSalesDashboardAsync(current, Range);
    }
}
