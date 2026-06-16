using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Randevoo.AdminPanel.Models.Support;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Application.Features.SupportTickets.Common;
using Randevoo.Domain.Constants;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Pages.Support;

public class MyModel : PageModel
{
    private readonly ISupportTicketsApiClient _supportApi;
    private readonly CurrentSessionState _session;
    private readonly IWebHostEnvironment _environment;

    public MyModel(ISupportTicketsApiClient supportApi, CurrentSessionState session, IWebHostEnvironment environment)
    {
        _supportApi = supportApi;
        _session = session;
        _environment = environment;
    }

    public IReadOnlyList<SupportTicketListItemDto> Tickets { get; private set; } = Array.Empty<SupportTicketListItemDto>();

    public IReadOnlyList<SupportTicketLookupOption> TicketTypes { get; private set; } = Array.Empty<SupportTicketLookupOption>();

    public IReadOnlyList<SupportTicketEventOption> EventOptions { get; private set; } = Array.Empty<SupportTicketEventOption>();

    public SelectList TicketTypeOptions => new(TicketTypes, "Id", "TitleFa", Input.TicketTypeId);

    public SelectList EventSelectOptions => new(EventOptions.Select(item => new { item.Id, Text = $"{item.Title} - {item.PlannerDisplayName}" }), "Id", "Text", Input.EventId);

    [BindProperty]
    public SupportTicketCreateInput Input { get; set; } = new()
    {
        TicketRecipientTypeId = SupportTicketLookupIds.RecipientPlatformSupport
    };

    [BindProperty]
    public List<IFormFile>? Attachments { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public bool OpenCreateModal { get; private set; }

    public async Task OnGetAsync(long? ticketStatusId, long? ticketTypeId, long? ticketRecipientTypeId, CancellationToken cancellationToken)
    {
        await LoadPageAsync(ticketStatusId, ticketTypeId, ticketRecipientTypeId, cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        Input.TicketRecipientTypeId = SupportTicketLookupIds.RecipientPlatformSupport;
        await LoadPageAsync(null, null, null, cancellationToken);

        if (!ModelState.IsValid)
        {
            OpenCreateModal = true;
            return Page();
        }

        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        try
        {
            var attachments = await CreateModel.SaveAttachmentsAsync(Attachments ?? [], _environment, cancellationToken);
            var ticket = await _supportApi.CreateTicketAsync(
                current,
                Input.Title,
                Input.TicketTypeId,
                SupportTicketLookupIds.RecipientPlatformSupport,
                Input.EventId,
                Input.Body,
                attachments,
                cancellationToken);

            StatusMessage = "تیکت پشتیبانی ثبت شد.";
            return RedirectToPage("/Support/Details", new { id = ticket.Id });
        }
        catch (Exception ex) when (ex is InvalidOperationException or Randevoo.Domain.Exceptions.DomainException)
        {
            ErrorMessage = ex.Message;
            OpenCreateModal = true;
            return Page();
        }
    }

    private async Task LoadPageAsync(long? ticketStatusId, long? ticketTypeId, long? ticketRecipientTypeId, CancellationToken cancellationToken)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        Tickets = await _supportApi.GetTicketsAsync(current, ticketStatusId, ticketTypeId, ticketRecipientTypeId, null, null, cancellationToken: cancellationToken);
        TicketTypes = await _supportApi.GetTicketTypesAsync(cancellationToken);
        EventOptions = await _supportApi.GetTicketEventOptionsAsync(current, cancellationToken);

        Input.TicketRecipientTypeId = SupportTicketLookupIds.RecipientPlatformSupport;
        if (Input.TicketTypeId <= 0 && TicketTypes.Count > 0)
            Input.TicketTypeId = TicketTypes[0].Id;
    }
}
