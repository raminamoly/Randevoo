using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Application.Features.SupportTickets.Common;
using Randevoo.Domain.Constants;

namespace Randevoo.AdminPanel.Pages.Support;

[Authorize(Policy = Policies.AdminOrPlanner)]
public class ReceivedModel : PageModel
{
    private readonly ISupportTicketsApiClient _supportApi;
    private readonly CurrentSessionState _session;

    public ReceivedModel(ISupportTicketsApiClient supportApi, CurrentSessionState session)
    {
        _supportApi = supportApi;
        _session = session;
    }

    public IReadOnlyList<SupportTicketListItemDto> Tickets { get; private set; } = Array.Empty<SupportTicketListItemDto>();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        Tickets = (await _supportApi.GetTicketsAsync(
                current,
                null,
                null,
                SupportTicketLookupIds.RecipientEventPlanner,
                null,
                null,
                cancellationToken: cancellationToken))
            .Where(item => current.Role == AdminRole.Admin || item.RecipientPlannerUserId == current.Id)
            .ToList();
    }
}
