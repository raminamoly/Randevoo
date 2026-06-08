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

    public IReadOnlyList<SelectListItem> CategoryOptions => Enum.GetValues<SupportTicketCategory>()
        .Select(category => new SelectListItem(SupportTicketUiFormatter.FormatCategory(category), ((int)category).ToString(), Category == category))
        .ToList();

    public IReadOnlyList<SupportTicketStatus> StatusTabs => Enum.GetValues<SupportTicketStatus>();

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
        Tickets = await _supportApi.GetTicketsAsync(current, null, Category, SubmitterRole, AssigneeUserId, createdFromUtc, createdToUtc, cancellationToken);
        SupportUsers = await _supportApi.GetSupportUsersAsync(current, cancellationToken);
    }

    public IReadOnlyList<SupportTicketListItemDto> TicketsFor(SupportTicketStatus status) =>
        Tickets.Where(ticket => ticket.Status == status).ToList();

    public static string ActionLabel(SupportTicketStatus status) => status switch
    {
        SupportTicketStatus.Open => "پاسخ",
        SupportTicketStatus.InProgress => "ادامه رسیدگی",
        SupportTicketStatus.WaitingForUser => "پیگیری",
        SupportTicketStatus.Closed => "مشاهده",
        SupportTicketStatus.Reopened => "پاسخ",
        _ => "مشاهده"
    };

    public static string ActionIcon(SupportTicketStatus status) => status switch
    {
        SupportTicketStatus.Open => "bi-reply-fill",
        SupportTicketStatus.InProgress => "bi-play-circle",
        SupportTicketStatus.WaitingForUser => "bi-hourglass-split",
        SupportTicketStatus.Closed => "bi-eye",
        SupportTicketStatus.Reopened => "bi-arrow-clockwise",
        _ => "bi-box-arrow-in-left"
    };
}
