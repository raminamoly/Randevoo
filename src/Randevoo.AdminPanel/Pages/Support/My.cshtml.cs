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

    public async Task OnGetAsync(SupportTicketStatus? status, SupportTicketCategory? category, CancellationToken cancellationToken)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        Tickets = await _supportApi.GetTicketsAsync(current, status, category, null, null, cancellationToken: cancellationToken);
    }
}
