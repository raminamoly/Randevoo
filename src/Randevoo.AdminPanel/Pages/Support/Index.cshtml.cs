using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Support;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Domain.Constants;
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
    public IReadOnlyList<SupportTicketLookupOption> TicketTypes { get; private set; } = Array.Empty<SupportTicketLookupOption>();
    public IReadOnlyList<SupportTicketLookupOption> TicketStatuses { get; private set; } = Array.Empty<SupportTicketLookupOption>();
    public IReadOnlyList<SupportTicketLookupOption> RecipientTypes { get; private set; } = Array.Empty<SupportTicketLookupOption>();
    public SupportTicketDashboardViewModel Dashboard { get; private set; } = new();
    public bool IsAdmin { get; private set; }
    public string CurrentSupportDisplayName { get; private set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public long? TicketStatusId { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? TicketTypeId { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? TicketRecipientTypeId { get; set; }

    [BindProperty(SupportsGet = true)]
    public UserRole? SubmitterRole { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? AssigneeUserId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? CreatedFromJalali { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? CreatedToJalali { get; set; }

    public IReadOnlyList<SelectListItem> StatusOptions => TicketStatuses
        .Select(status => new SelectListItem(status.TitleFa, status.Id.ToString(), TicketStatusId == status.Id))
        .ToList();

    public IReadOnlyList<SelectListItem> TypeOptions => TicketTypes
        .Select(type => new SelectListItem(type.TitleFa, type.Id.ToString(), TicketTypeId == type.Id))
        .ToList();

    public IReadOnlyList<SelectListItem> RecipientOptions => RecipientTypes
        .Select(type => new SelectListItem(type.TitleFa, type.Id.ToString(), TicketRecipientTypeId == type.Id))
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
        TicketTypes = await _supportApi.GetTicketTypesAsync(cancellationToken);
        TicketStatuses = await _supportApi.GetTicketStatusesAsync(cancellationToken);
        RecipientTypes = await _supportApi.GetTicketRecipientTypesAsync(current, cancellationToken);
        var filters = new SupportTicketDashboardFilters(TicketStatusId, TicketTypeId, TicketRecipientTypeId, SubmitterRole, AssigneeUserId, createdFromUtc, createdToUtc);
        Dashboard = await _supportApi.GetDashboardAsync(current, filters, cancellationToken);
        SupportUsers = await _supportApi.GetSupportUsersAsync(current, cancellationToken);
    }
}
