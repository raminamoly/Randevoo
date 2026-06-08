using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Support;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Pages.Support;

[Authorize(Policy = Policies.SupportOrAdmin)]
public class IndexModel : PageModel
{
    private readonly ISupportTicketsApiClient _supportApi;
    private readonly CurrentSessionState _session;

    public IndexModel(ISupportTicketsApiClient supportApi, CurrentSessionState session)
    {
        _supportApi = supportApi;
        _session = session;
    }

    public IReadOnlyList<(long Id, string DisplayName)> SupportUsers { get; private set; } = Array.Empty<(long Id, string DisplayName)>();
    public SupportTicketDashboardViewModel Dashboard { get; private set; } = new();
    public bool IsAdmin { get; private set; }
    public string CurrentSupportDisplayName { get; private set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public SupportTicketStatus? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public SupportTicketCategory? Category { get; set; }

    [BindProperty(SupportsGet = true)]
    public UserRole? SubmitterRole { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? AssigneeUserId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? CreatedFromJalali { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? CreatedToJalali { get; set; }

    public IReadOnlyList<SelectListItem> StatusOptions => Enum.GetValues<SupportTicketStatus>()
        .Select(status => new SelectListItem(SupportTicketUiFormatter.FormatStatus(status), ((int)status).ToString(), Status == status))
        .ToList();

    public IReadOnlyList<SelectListItem> CategoryOptions => Enum.GetValues<SupportTicketCategory>()
        .Select(category => new SelectListItem(SupportTicketUiFormatter.FormatCategory(category), ((int)category).ToString(), Category == category))
        .ToList();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        IsAdmin = current.Role == AdminRole.Admin;
        if (current.Role == AdminRole.SupportTeam)
        {
            AssigneeUserId = current.Id;
            CurrentSupportDisplayName = string.IsNullOrWhiteSpace(current.FullName) ? current.Mobile : current.FullName;
        }

        var createdFromUtc = SupportTicketUiFormatter.ParseJalaliDate(CreatedFromJalali);
        var createdToUtc = SupportTicketUiFormatter.ParseJalaliDate(CreatedToJalali);
        var filters = new SupportTicketDashboardFilters(Status, Category, SubmitterRole, AssigneeUserId, createdFromUtc, createdToUtc);
        Dashboard = await _supportApi.GetDashboardAsync(current, filters, cancellationToken);
        SupportUsers = await _supportApi.GetSupportUsersAsync(current, cancellationToken);
    }
}
