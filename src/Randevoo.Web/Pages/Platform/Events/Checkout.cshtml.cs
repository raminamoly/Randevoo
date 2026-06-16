using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.Web.Services;

namespace Randevoo.Web.Pages.Platform.Events;

public class CheckoutModel : PageModel
{
    private readonly EndUserEventsApiClient _eventsApiClient;
    private readonly EndUserTicketsApiClient _ticketsApiClient;
    private readonly EndUserSessionService _session;
    private readonly IWebHostEnvironment _environment;

    public CheckoutModel(
        EndUserEventsApiClient eventsApiClient,
        EndUserTicketsApiClient ticketsApiClient,
        EndUserSessionService session,
        IWebHostEnvironment environment)
    {
        _eventsApiClient = eventsApiClient;
        _ticketsApiClient = ticketsApiClient;
        _session = session;
        _environment = environment;
    }

    public EndUserEventDetailsViewModel? EventDetails { get; private set; }
    public TicketCheckoutPreviewViewModel? Preview { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? SuccessMessage { get; private set; }

    [BindProperty]
    public CheckoutInput Input { get; set; } = new();

    [BindProperty]
    public IFormFile? ManualReceiptFile { get; set; }

    public async Task<IActionResult> OnGetAsync(long id, CancellationToken cancellationToken)
    {
        if (!_session.IsSignedIn)
            return RedirectToPage("/Platform/Account/Login", new { returnUrl = $"/platform/events/{id}/checkout" });

        await LoadDetailsAsync(id, cancellationToken);
        await TryLoadPreviewAsync(id, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostPreviewAsync(long id, CancellationToken cancellationToken)
    {
        if (!_session.IsSignedIn)
            return RedirectToPage("/Platform/Account/Login", new { returnUrl = $"/platform/events/{id}/checkout" });

        await LoadDetailsAsync(id, cancellationToken);
        await TryLoadPreviewAsync(id, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync(long id, CancellationToken cancellationToken)
    {
        if (!_session.IsSignedIn)
            return RedirectToPage("/Platform/Account/Login", new { returnUrl = $"/platform/events/{id}/checkout" });

        await LoadDetailsAsync(id, cancellationToken);
        if (EventDetails is null)
            return NotFound();

        try
        {
            string? receiptPath = null;
            if (EventDetails.PaymentCollectionMethod != 0)
                receiptPath = await SaveManualReceiptAsync(id, cancellationToken);

            var result = await _ticketsApiClient.BuyAsync(id, BuildCheckoutRequest(receiptPath), cancellationToken);
            if (result.TicketIds.Count > 0)
                return RedirectToPage("/Platform/Tickets/Index", new { purchase = "success" });

            return RedirectToPage("/Platform/Tickets/Index", new { purchase = "pending" });
        }
        catch (Exception ex)
        {
            ErrorMessage = ToFriendlyPurchaseMessage(ex);
            await TryLoadPreviewAsync(id, cancellationToken);
            return Page();
        }
    }

    private async Task LoadDetailsAsync(long id, CancellationToken cancellationToken)
    {
        EventDetails = await _eventsApiClient.GetDetailsAsync(id, cancellationToken);
        if (EventDetails is not null)
            ViewData["Title"] = $"خرید بلیت {EventDetails.Title}";
    }

    private async Task TryLoadPreviewAsync(long id, CancellationToken cancellationToken)
    {
        try
        {
            Preview = await _ticketsApiClient.PreviewAsync(id, BuildCheckoutRequest(), cancellationToken);
        }
        catch (Exception ex)
        {
            ErrorMessage = ToFriendlyPurchaseMessage(ex);
        }
    }

    private TicketCheckoutRequestViewModel BuildCheckoutRequest(string? receiptPath = null)
    {
        return new TicketCheckoutRequestViewModel(
            Input.DiscountCode,
            null,
            Input.ParticipantMode == CheckoutParticipantMode.Other ? Input.ParticipantMobileNumber : null,
            receiptPath,
            Input.ManualReceiptTrackingNumber,
            Input.ManualReceiptNote);
    }

    private async Task<string> SaveManualReceiptAsync(long eventId, CancellationToken cancellationToken)
    {
        if (ManualReceiptFile is null || ManualReceiptFile.Length == 0)
            throw new InvalidOperationException("برای پرداخت دستی باید تصویر یا فایل رسید را بارگذاری کنی.");

        var extension = Path.GetExtension(ManualReceiptFile.FileName);
        if (string.IsNullOrWhiteSpace(extension))
            extension = ".jpg";

        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };
        if (!allowedExtensions.Contains(extension))
            throw new InvalidOperationException("فرمت رسید باید تصویر یا PDF باشد.");

        var uploadsRoot = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "..", "Randevoo.AdminPanel", "wwwroot", "uploads", "manual-receipts"));
        Directory.CreateDirectory(uploadsRoot);

        var fileName = $"event-{eventId}-{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(uploadsRoot, fileName);
        await using var stream = System.IO.File.Create(fullPath);
        await ManualReceiptFile.CopyToAsync(stream, cancellationToken);

        return $"/uploads/manual-receipts/{fileName}";
    }

    private static string ToFriendlyPurchaseMessage(Exception ex)
    {
        var message = ex.Message;
        if (message.Contains("Participant not found", StringComparison.OrdinalIgnoreCase))
            return "کاربری با این شماره موبایل پیدا نشد. شرکت‌کننده باید قبلاً حساب داشته باشد.";
        if (message.Contains("Profile", StringComparison.OrdinalIgnoreCase))
            return "شرکت‌کننده باید اول پروفایلش را کامل کند.";
        if (message.Contains("education", StringComparison.OrdinalIgnoreCase))
            return "مدرک تحصیلی شرکت‌کننده با شرایط این رویداد هم‌خوانی ندارد.";
        if (message.Contains("age", StringComparison.OrdinalIgnoreCase))
            return "سن شرکت‌کننده با بازه سنی این رویداد هم‌خوانی ندارد.";
        if (message.Contains("capacity", StringComparison.OrdinalIgnoreCase))
            return "ظرفیت این رویداد برای جنسیت شرکت‌کننده تکمیل شده است.";
        if (message.Contains("Discount", StringComparison.OrdinalIgnoreCase))
            return "کد تخفیف واردشده برای این شرکت‌کننده معتبر نیست.";
        if (message.Contains("receipt", StringComparison.OrdinalIgnoreCase))
            return "برای پرداخت دستی، رسید معتبر لازم است.";

        return "ثبت خرید انجام نشد. چند لحظه بعد دوباره تلاش کن.";
    }
}

public sealed class CheckoutInput
{
    public CheckoutParticipantMode ParticipantMode { get; set; } = CheckoutParticipantMode.Self;
    public string? ParticipantMobileNumber { get; set; }
    public string? DiscountCode { get; set; }
    public string? ManualReceiptTrackingNumber { get; set; }
    public string? ManualReceiptNote { get; set; }
}

public enum CheckoutParticipantMode
{
    Self = 0,
    Other = 1
}
