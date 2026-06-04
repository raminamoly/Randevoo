using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Finance;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Finance;

[Authorize(Policy = Policies.AdminOnly)]
public class TicketTransactionsModel : PageModel
{
    private readonly IFinanceApiClient _financeApi;
    private readonly CurrentSessionState _session;

    public TicketTransactionsModel(IFinanceApiClient financeApi, CurrentSessionState session)
    {
        _financeApi = financeApi;
        _session = session;
    }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? EventId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Sort { get; set; } = "start-desc";

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public bool IsRtl => _session.IsRtl;
    public bool HasActiveFilters => !string.IsNullOrWhiteSpace(Search)
        || EventId is not null
        || !string.IsNullOrWhiteSpace(FromDate)
        || !string.IsNullOrWhiteSpace(ToDate)
        || !string.Equals(Sort, "start-desc", StringComparison.OrdinalIgnoreCase);
    public int PageSize { get; } = 8;
    public int TotalCount { get; private set; }
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public IReadOnlyList<AdminEventTicketTransactionGroup> EventTransactions { get; private set; } = Array.Empty<AdminEventTicketTransactionGroup>();

    public async Task OnGetAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        var groups = (await _financeApi.GetTicketPurchaseTransactionsByEventAsync(current)).AsEnumerable();

        if (EventId is long eventId)
            groups = groups.Where(item => item.EventId == eventId);

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var query = Search.Trim();
            groups = groups.Where(item =>
                item.EventTitle.Contains(query, StringComparison.OrdinalIgnoreCase)
                || item.PlannerName.Contains(query, StringComparison.OrdinalIgnoreCase)
                || item.Transactions.Any(transaction => transaction.BuyerName.Contains(query, StringComparison.OrdinalIgnoreCase)
                    || transaction.BuyerMobile.Contains(query, StringComparison.OrdinalIgnoreCase)));
        }

        if (PersianDateFormatter.TryParseDate(FromDate, IsRtl, out var fromDate))
            groups = groups.Where(item => item.StartAtUtc.Date >= fromDate.UtcDateTime.Date);

        if (PersianDateFormatter.TryParseDate(ToDate, IsRtl, out var toDate))
            groups = groups.Where(item => item.StartAtUtc.Date <= toDate.UtcDateTime.Date);

        groups = Sort switch
        {
            "amount-desc" => groups.OrderByDescending(item => item.TotalTicketAmount),
            "tickets-desc" => groups.OrderByDescending(item => item.TicketCount),
            "start-asc" => groups.OrderBy(item => item.StartAtUtc),
            _ => groups.OrderByDescending(item => item.StartAtUtc)
        };

        var filtered = groups.ToList();
        TotalCount = filtered.Count;
        PageNumber = Math.Clamp(PageNumber, 1, TotalPages);
        EventTransactions = filtered
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToList();
    }
}
