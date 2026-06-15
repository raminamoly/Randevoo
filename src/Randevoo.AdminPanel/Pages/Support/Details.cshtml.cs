using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Support;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Application.Features.SupportTickets.Common;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Pages.Support;

public class DetailsModel : PageModel
{
    private readonly ISupportTicketsApiClient _supportApi;
    private readonly CurrentSessionState _session;
    private readonly IWebHostEnvironment _environment;

    public DetailsModel(ISupportTicketsApiClient supportApi, CurrentSessionState session, IWebHostEnvironment environment)
    {
        _supportApi = supportApi;
        _session = session;
        _environment = environment;
    }

    public SupportTicketDetailDto Ticket { get; private set; } = null!;
    public IReadOnlyList<SupportTicketListItemDto> PreviousTickets { get; private set; } = Array.Empty<SupportTicketListItemDto>();
    public SupportSubmitterFinanceContext? FinanceContext { get; private set; }
    public IReadOnlyList<SupportSubmitterEventBookingItem> EventBookings { get; private set; } = Array.Empty<SupportSubmitterEventBookingItem>();
    public IReadOnlyList<(long Id, string DisplayName)> SupportUsers { get; private set; } = Array.Empty<(long Id, string DisplayName)>();
    public IReadOnlyList<SupportTicketLookupOption> TicketStatuses { get; private set; } = Array.Empty<SupportTicketLookupOption>();
    public MockUser CurrentUser => _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
    public bool CanManage => CurrentUser.Role is AdminRole.Admin or AdminRole.SupportTeam || Ticket.RecipientPlannerUserId == CurrentUser.Id;
    public bool CanViewSubmitterContext => CurrentUser.Role is AdminRole.Admin or AdminRole.SupportTeam;
    public bool CanUseSystemTicketsPage => CurrentUser.Role is AdminRole.Admin or AdminRole.SupportTeam;
    public bool IsAdmin => CurrentUser.Role == AdminRole.Admin;
    public IReadOnlyList<SelectListItem> StatusOptions => TicketStatuses
        .Select(status => new SelectListItem(status.TitleFa, status.Id.ToString(), Ticket.TicketStatusId == status.Id))
        .ToList();

    [BindProperty]
    public SupportTicketReplyInput ReplyInput { get; set; } = new();

    [BindProperty]
    public List<IFormFile> Attachments { get; set; } = new();

    [BindProperty]
    public SupportTicketStatusInput StatusInput { get; set; } = new();

    [BindProperty]
    public SupportTicketReassignInput ReassignInput { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync(long id, CancellationToken cancellationToken)
    {
        await LoadAsync(id, cancellationToken);
    }

    public async Task<IActionResult> OnPostReplyAsync(long id, CancellationToken cancellationToken)
    {
        try
        {
            var attachments = await CreateModel.SaveAttachmentsAsync(Attachments, _environment, cancellationToken);
            await _supportApi.ReplyAsync(CurrentUser, id, ReplyInput.Body, attachments, IsAdmin ? ReplyInput.RepresentedUserId : null, cancellationToken);
            StatusMessage = "پاسخ ثبت شد.";
        }
        catch (Exception ex) when (ex is InvalidOperationException or Randevoo.Domain.Exceptions.DomainException)
        {
            StatusMessage = ex.Message;
        }

        return RedirectToPage("/Support/Details", new { id });
    }

    public async Task<IActionResult> OnPostStatusAsync(long id, CancellationToken cancellationToken)
    {
        try
        {
            await _supportApi.ChangeStatusAsync(CurrentUser, id, StatusInput.TicketStatusId, StatusInput.Note, cancellationToken);
            StatusMessage = "وضعیت تیکت تغییر کرد.";
        }
        catch (Exception ex) when (ex is InvalidOperationException or Randevoo.Domain.Exceptions.DomainException)
        {
            StatusMessage = ex.Message;
        }

        return RedirectToPage("/Support/Details", new { id });
    }

    public async Task<IActionResult> OnPostReassignAsync(long id, CancellationToken cancellationToken)
    {
        try
        {
            await _supportApi.ReassignAsync(CurrentUser, id, ReassignInput.AssigneeUserId, ReassignInput.Note, cancellationToken);
            StatusMessage = "مسئول تیکت تغییر کرد.";
        }
        catch (Exception ex) when (ex is InvalidOperationException or Randevoo.Domain.Exceptions.DomainException)
        {
            StatusMessage = ex.Message;
        }

        return RedirectToPage("/Support/Details", new { id });
    }

    private async Task LoadAsync(long id, CancellationToken cancellationToken)
    {
        Ticket = await _supportApi.GetTicketAsync(CurrentUser, id, cancellationToken);
        TicketStatuses = await _supportApi.GetTicketStatusesAsync(cancellationToken);
        StatusInput.TicketStatusId = Ticket.TicketStatusId;
        if (CanViewSubmitterContext)
        {
            FinanceContext = await _supportApi.GetSubmitterFinanceAsync(CurrentUser, id, cancellationToken);
            EventBookings = await _supportApi.GetSubmitterEventsAsync(CurrentUser, id, cancellationToken);
            PreviousTickets = await _supportApi.GetSubmitterPreviousTicketsAsync(CurrentUser, id, cancellationToken);
        }
        else
        {
            PreviousTickets = (await _supportApi.GetTicketsAsync(CurrentUser, null, null, null, null, null, cancellationToken: cancellationToken))
                .Where(item => item.SubmitterUserId == Ticket.Submitter.UserId && item.Id != Ticket.Id)
                .Take(10)
                .ToList();
        }

        SupportUsers = await _supportApi.GetSupportUsersAsync(CurrentUser, cancellationToken);
    }
}
