using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Buyers;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Domain.Enums;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Pages.Buyers;

[Authorize(Policy = Policies.AdminPlannerOrSupport)]
public class IndexModel : PageModel
{
    private readonly RandevooDbContext _db;
    private readonly CurrentSessionState _session;

    public IndexModel(RandevooDbContext db, CurrentSessionState session)
    {
        _db = db;
        _session = session;
    }

    [BindProperty(SupportsGet = true)]
    public long? EventId { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? BuyerUserId { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? TicketOrderId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? PaymentStatus { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Sort { get; set; } = "created-desc";

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 25;

    public TicketOrderListResult Result { get; private set; } = new();
    public SelectList EventOptions { get; private set; } = new(Array.Empty<SelectListItem>());
    public bool IsRtl => _session.IsRtl;
    public bool IsAdmin => _session.CurrentUser?.Role == AdminRole.Admin;
    public bool HasActiveFilters => EventId.HasValue
        || BuyerUserId.HasValue
        || TicketOrderId.HasValue
        || !string.IsNullOrWhiteSpace(Search)
        || !string.IsNullOrWhiteSpace(PaymentStatus)
        || !string.Equals(Sort, "created-desc", StringComparison.OrdinalIgnoreCase);
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(Result.TotalCount / (double)Math.Clamp(PageSize, 10, 100)));
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var current = _session.CurrentUser;
        if (current is null)
            return Challenge();

        PageNumber = Math.Max(PageNumber, 1);
        PageSize = Math.Clamp(PageSize, 10, 100);
        await LoadEventOptionsAsync(current, cancellationToken);
        Result = await QueryOrdersAsync(current, cancellationToken);
        return Page();
    }

    public static string PaymentStatusLabel(TicketOrderPaymentStatus status) => status switch
    {
        TicketOrderPaymentStatus.Paid => "پرداخت شده",
        TicketOrderPaymentStatus.Rejected => "رد شده",
        TicketOrderPaymentStatus.Refunded => "بازگشت خورده",
        _ => "در انتظار پرداخت"
    };

    public static string PaymentStatusClass(TicketOrderPaymentStatus status) => status switch
    {
        TicketOrderPaymentStatus.Paid => "status-approved",
        TicketOrderPaymentStatus.Rejected => "status-rejected",
        TicketOrderPaymentStatus.Refunded => "status-closed",
        _ => "status-pending"
    };

    private async Task LoadEventOptionsAsync(MockUser current, CancellationToken cancellationToken)
    {
        var query = _db.DatingEvents
            .AsNoTracking()
            .OrderByDescending(item => item.DateTimeStart)
            .AsQueryable();

        if (current.Role == AdminRole.EventPlanner)
            query = query.Where(item => item.EventPlannerUserId == current.Id);

        var events = await query
            .Select(item => new { item.Id, Title = $"#{item.EventCode} - {item.Title}" })
            .Take(250)
            .ToListAsync(cancellationToken);

        EventOptions = new SelectList(events, "Id", "Title", EventId);
    }

    private async Task<TicketOrderListResult> QueryOrdersAsync(MockUser current, CancellationToken cancellationToken)
    {
        var query = _db.TicketOrders
            .AsNoTracking()
            .AsQueryable();

        if (current.Role == AdminRole.EventPlanner)
            query = query.Where(item => item.DatingEvent.EventPlannerUserId == current.Id);

        if (EventId is long eventId)
            query = query.Where(item => item.DatingEventId == eventId);

        if (BuyerUserId is long buyerUserId)
            query = query.Where(item => item.BuyerUserId == buyerUserId);

        if (TicketOrderId is long ticketOrderId)
            query = query.Where(item => item.Id == ticketOrderId);

        if (!string.IsNullOrWhiteSpace(PaymentStatus)
            && Enum.TryParse<TicketOrderPaymentStatus>(PaymentStatus, true, out var status))
            query = query.Where(item => item.PaymentStatus == status);

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var search = Search.Trim();
            var like = $"%{search}%";
            var normalizedCode = search.Trim().TrimStart('#');
            var hasEventCode = int.TryParse(normalizedCode, out var eventCode);
            query = query.Where(item =>
                (hasEventCode && item.DatingEvent.EventCode == eventCode)
                || EF.Functions.Like(item.DatingEvent.EventCode.ToString(), like)
                ||
                EF.Functions.Like(item.DatingEvent.Title, like)
                || EF.Functions.Like(item.BuyerUser.MobileNumber, like)
                || (item.BuyerUser.Email != null && EF.Functions.Like(item.BuyerUser.Email, like))
                || EF.Functions.Like(item.BuyerUser.Profile!.DisplayName, like)
                || (item.DiscountCode != null && EF.Functions.Like(item.DiscountCode, like))
                || EF.Functions.Like(item.Id.ToString(), like));
        }

        var summary = await query
            .GroupBy(_ => 1)
            .Select(group => new TicketOrderListSummary
            {
                TotalOrders = group.Count(),
                PaidOrders = group.Count(item => item.PaymentStatus == TicketOrderPaymentStatus.Paid),
                PendingOrders = group.Count(item => item.PaymentStatus == TicketOrderPaymentStatus.Pending),
                TicketCount = group.Sum(item => item.Tickets.Count(ticket => !ticket.IsRefunded && !ticket.IsRemoved)),
                NetAmount = group.Sum(item => item.NetAmount),
                ReportingNetAmountIrr = group.Sum(item => item.ReportingNetAmountIrr)
            })
            .FirstOrDefaultAsync(cancellationToken) ?? new TicketOrderListSummary();

        var totalCount = summary.TotalOrders;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
        PageNumber = Math.Min(PageNumber, totalPages);

        query = Sort switch
        {
            "created-asc" => query.OrderBy(item => item.CreatedAt).ThenBy(item => item.Id),
            "amount-desc" => query.OrderByDescending(item => item.NetAmount).ThenByDescending(item => item.CreatedAt),
            "amount-asc" => query.OrderBy(item => item.NetAmount).ThenByDescending(item => item.CreatedAt),
            "event" => query.OrderBy(item => item.DatingEvent.Title).ThenByDescending(item => item.CreatedAt),
            _ => query.OrderByDescending(item => item.CreatedAt).ThenByDescending(item => item.Id)
        };

        var items = await query
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .Select(item => new TicketOrderListItem
            {
                OrderId = item.Id,
                EventId = item.DatingEventId,
                EventTitle = $"#{item.DatingEvent.EventCode} - {item.DatingEvent.Title}",
                EventPlannerUserId = item.DatingEvent.EventPlannerUserId,
                EventPlannerName = item.DatingEvent.EventPlannerUser.Profile != null
                    ? item.DatingEvent.EventPlannerUser.Profile.DisplayName
                    : item.DatingEvent.EventPlannerUser.MobileNumber,
                BuyerUserId = item.BuyerUserId,
                BuyerName = item.BuyerUser.Profile != null ? item.BuyerUser.Profile.DisplayName : item.BuyerUser.MobileNumber,
                BuyerMobile = item.BuyerUser.MobileNumber,
                TicketCount = item.Tickets.Count(ticket => !ticket.IsRefunded && !ticket.IsRemoved),
                GrossAmount = item.GrossAmount,
                DiscountAmount = item.DiscountAmount,
                NetAmount = item.NetAmount,
                CurrencyCode = item.CurrencyCode,
                ReportingNetAmountIrr = item.ReportingNetAmountIrr,
                DiscountCode = item.DiscountCode,
                PaymentCollectionMethod = item.PaymentCollectionMethod,
                PaymentStatus = item.PaymentStatus,
                OrderStatus = item.OrderStatus,
                CreatedAtUtc = item.CreatedAt,
                PaidAtUtc = item.PaidAtUtc
            })
            .ToListAsync(cancellationToken);

        return new TicketOrderListResult
        {
            TotalCount = totalCount,
            Summary = summary,
            Items = items
        };
    }
}
