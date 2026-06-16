using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.Web.Services;

namespace Randevoo.Web.Pages.Platform.Events;

public class DetailsModel : PageModel
{
    private readonly EndUserEventsApiClient _eventsApiClient;
    private readonly EndUserTicketsApiClient _ticketsApiClient;
    private readonly EndUserSessionService _session;

    public DetailsModel(
        EndUserEventsApiClient eventsApiClient,
        EndUserTicketsApiClient ticketsApiClient,
        EndUserSessionService session)
    {
        _eventsApiClient = eventsApiClient;
        _ticketsApiClient = ticketsApiClient;
        _session = session;
    }

    public EndUserEventDetailsViewModel? EventDetails { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? PurchaseErrorMessage { get; private set; }
    public bool IsSignedIn => _session.IsSignedIn;

    public async Task<IActionResult> OnGetAsync(long id, CancellationToken cancellationToken)
    {
        if (!_session.IsSignedIn)
            return RedirectToPage("/Platform/Account/Login", new { returnUrl = $"/platform/events/{id}" });

        try
        {
            EventDetails = await _eventsApiClient.GetDetailsAsync(id, cancellationToken);
            if (EventDetails is null)
                return NotFound();

            ViewData["Title"] = EventDetails.Title;
            return Page();
        }
        catch (Exception)
        {
            ErrorMessage = "دریافت جزئیات رویداد ممکن نیست.";
            return Page();
        }
    }

    private async Task LoadDetailsAsync(long id, CancellationToken cancellationToken)
    {
        EventDetails = await _eventsApiClient.GetDetailsAsync(id, cancellationToken);
        if (EventDetails is not null)
            ViewData["Title"] = EventDetails.Title;
    }

    private static string ToFriendlyPurchaseMessage(Exception ex)
    {
        var message = ex.Message;
        if (message.Contains("Profile", StringComparison.OrdinalIgnoreCase))
            return "برای خرید این رویداد اول پروفایل را کامل کن.";
        if (message.Contains("education", StringComparison.OrdinalIgnoreCase))
            return "مدرک تحصیلی پروفایل با شرایط این رویداد هم‌خوانی ندارد.";
        if (message.Contains("age", StringComparison.OrdinalIgnoreCase))
            return "سن ثبت‌شده در پروفایل با بازه سنی این رویداد هم‌خوانی ندارد.";
        if (message.Contains("Balance", StringComparison.OrdinalIgnoreCase))
            return "موجودی یا وضعیت پرداخت برای خرید این بلیت کافی نیست.";
        if (message.Contains("capacity", StringComparison.OrdinalIgnoreCase))
            return "ظرفیت این رویداد برای پروفایل شما تکمیل شده است.";
        if (message.Contains("restricted", StringComparison.OrdinalIgnoreCase))
            return "فعلاً امکان خرید بلیت برای این حساب محدود شده است.";
        if (message.Contains("Discount", StringComparison.OrdinalIgnoreCase))
            return "کد تخفیف واردشده برای این رویداد معتبر نیست.";

        return "خرید بلیت انجام نشد. چند لحظه بعد دوباره تلاش کن.";
    }
}
