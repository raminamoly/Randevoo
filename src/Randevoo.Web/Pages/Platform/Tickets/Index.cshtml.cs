using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.Web.Services;

namespace Randevoo.Web.Pages.Platform.Tickets;

public class IndexModel : PageModel
{
    private readonly EndUserTicketsApiClient _ticketsApiClient;
    private readonly EndUserSessionService _session;

    public IndexModel(EndUserTicketsApiClient ticketsApiClient, EndUserSessionService session)
    {
        _ticketsApiClient = ticketsApiClient;
        _session = session;
    }

    public IReadOnlyList<MyTicketViewModel> Tickets { get; private set; } = Array.Empty<MyTicketViewModel>();
    public string? ErrorMessage { get; private set; }
    public string? SuccessMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync(string? purchase, CancellationToken cancellationToken)
    {
        if (!_session.IsSignedIn)
            return RedirectToPage("/Platform/Account/Login", new { returnUrl = "/platform/tickets" });

        SuccessMessage = purchase switch
        {
            "success" => "پرداخت انجام شد و بلیت صادر شد.",
            "pending" => "سفارش ثبت شد و بعد از تایید رسید، بلیت صادر می‌شود.",
            _ => null
        };

        try
        {
            Tickets = await _ticketsApiClient.ListMineAsync(cancellationToken);
        }
        catch (Exception)
        {
            ErrorMessage = "در حال حاضر دریافت بلیت‌ها ممکن نیست.";
        }

        return Page();
    }

    public string GetOrderStatus(MyTicketViewModel ticket)
    {
        if (ticket.PaymentStatus == 0)
            return "در انتظار پرداخت/تایید";
        if (ticket.PaymentStatus == 2)
            return "پرداخت رد شده";
        if (ticket.IsRemoved)
            return "حذف‌شده";
        if (ticket.IsRefunded)
            return "بازگشت وجه";
        if (!ticket.HasValidTicket)
            return "در انتظار صدور بلیت";
        if (ticket.DateTimeEnd < DateTime.UtcNow)
            return "برگزارشده";

        return "بلیت معتبر";
    }

    public string GetOrderStatusClass(MyTicketViewModel ticket)
    {
        if (ticket.PaymentStatus == 2 || ticket.IsRemoved || ticket.IsRefunded)
            return "rv-ticket-status--danger";
        if (ticket.PaymentStatus == 0 || !ticket.HasValidTicket)
            return "rv-ticket-status--pending";
        if (ticket.DateTimeEnd < DateTime.UtcNow)
            return "rv-ticket-status--muted";

        return string.Empty;
    }

    public static string PaymentMethodLabel(int method) => method switch
    {
        0 => "درگاه آنلاین پلتفرم",
        1 => "واریز دستی به پلتفرم",
        2 => "واریز دستی به برگزارکننده",
        _ => "نامشخص"
    };
}
