using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Application.Features.SupportTickets.Common;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Pages.Support;

public class MyModel : PageModel
{
    private readonly ISupportTicketsApiClient _supportApi;
    private readonly CurrentSessionState _session;

    public MyModel(ISupportTicketsApiClient supportApi, CurrentSessionState session)
    {
        _supportApi = supportApi;
        _session = session;
    }

    public IReadOnlyList<SupportTicketListItemDto> Tickets { get; private set; } = Array.Empty<SupportTicketListItemDto>();

    public async Task OnGetAsync(long? ticketStatusId, long? ticketTypeId, long? ticketRecipientTypeId, CancellationToken cancellationToken)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        Tickets = await _supportApi.GetTicketsAsync(current, ticketStatusId, ticketTypeId, ticketRecipientTypeId, null, null, cancellationToken: cancellationToken);
    }
}
