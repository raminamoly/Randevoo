using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Support;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Application.Features.SupportTickets.Common;
using Randevoo.Domain.Constants;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Pages.Support;

[Authorize(Policy = Policies.SupportOrAdmin)]
public class TicketsModel : PageModel
{
    private readonly ISupportTicketsApiClient _supportApi;
    private readonly CurrentSessionState _session;

    public TicketsModel(ISupportTicketsApiClient supportApi, CurrentSessionState session)
    {
        _supportApi = supportApi;
        _session = session;
    }

    public IReadOnlyList<SupportTicketListItemDto> Tickets { get; private set; } = Array.Empty<SupportTicketListItemDto>();
    public IReadOnlyList<(long Id, string DisplayName)> SupportUsers { get; private set; } = Array.Empty<(long Id, string DisplayName)>();
    public IReadOnlyList<SupportTicketLookupOption> TicketTypes { get; private set; } = Array.Empty<SupportTicketLookupOption>();
    public IReadOnlyList<SupportTicketLookupOption> TicketStatuses { get; private set; } = Array.Empty<SupportTicketLookupOption>();
    public IReadOnlyList<SupportTicketLookupOption> RecipientTypes { get; private set; } = Array.Empty<SupportTicketLookupOption>();
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

    public IReadOnlyList<SelectListItem> TypeOptions => TicketTypes
        .Select(type => new SelectListItem(type.TitleFa, type.Id.ToString(), TicketTypeId == type.Id))
        .ToList();

    public IReadOnlyList<SelectListItem> RecipientOptions => RecipientTypes
        .Select(type => new SelectListItem(type.TitleFa, type.Id.ToString(), TicketRecipientTypeId == type.Id))
        .ToList();

    public IReadOnlyList<SupportTicketLookupOption> StatusTabs => TicketStatuses;

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
        Tickets = await _supportApi.GetTicketsAsync(current, null, TicketTypeId, TicketRecipientTypeId, SubmitterRole, AssigneeUserId, createdFromUtc, createdToUtc, cancellationToken);
        SupportUsers = await _supportApi.GetSupportUsersAsync(current, cancellationToken);
    }

    public IReadOnlyList<SupportTicketListItemDto> TicketsFor(long ticketStatusId) =>
        Tickets.Where(ticket => ticket.TicketStatusId == ticketStatusId).ToList();

    public static string ActionLabel(long ticketStatusId) => ticketStatusId switch
    {
        SupportTicketLookupIds.StatusOpen => "پاسخ",
        SupportTicketLookupIds.StatusInProgress => "ادامه رسیدگی",
        SupportTicketLookupIds.StatusWaitingForUser => "پیگیری",
        SupportTicketLookupIds.StatusClosed => "مشاهده",
        SupportTicketLookupIds.StatusReopened => "پاسخ",
        _ => "مشاهده"
    };

    public static string ActionIcon(long ticketStatusId) => ticketStatusId switch
    {
        SupportTicketLookupIds.StatusOpen => "bi-reply-fill",
        SupportTicketLookupIds.StatusInProgress => "bi-play-circle",
        SupportTicketLookupIds.StatusWaitingForUser => "bi-hourglass-split",
        SupportTicketLookupIds.StatusClosed => "bi-eye",
        SupportTicketLookupIds.StatusReopened => "bi-arrow-clockwise",
        _ => "bi-box-arrow-in-left"
    };
}
