using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Events;

[Authorize(Policy = Policies.AdminOrPlanner)]
public class BuyersModel : PageModel
{
    private readonly IEventsApiClient _eventsApi;
    private readonly CurrentSessionState _session;

    public BuyersModel(IEventsApiClient eventsApi, CurrentSessionState session)
    {
        _eventsApi = eventsApi;
        _session = session;
    }

    public long EventId { get; private set; }

    public IReadOnlyList<EventTicketBuyerItem> Buyers { get; private set; } = Array.Empty<EventTicketBuyerItem>();

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Gender { get; set; }

    [BindProperty(SupportsGet = true)]
    public string View { get; set; } = "grid";

    [BindProperty]
    public EmergencyRefundInput Input { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public bool IsAdmin => _session.CurrentUser?.Role == AdminRole.Admin;

    public bool IsRtl => _session.IsRtl;

    public bool HasActiveFilters => !string.IsNullOrWhiteSpace(Search)
        || !string.IsNullOrWhiteSpace(Status)
        || !string.IsNullOrWhiteSpace(Gender);

    public async Task OnGetAsync(long eventId)
    {
        EventId = eventId;
        await LoadBuyersAsync(eventId);
    }

    public async Task<IActionResult> OnPostRefundAsync(long eventId)
    {
        EventId = eventId;
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        if (!IsAdmin)
        {
            ModelState.AddModelError(string.Empty, "فقط مدیر می تواند بازگشت وجه اضطراری ثبت کند.");
        }

        if (!ModelState.IsValid)
        {
            await LoadBuyersAsync(eventId);
            return Page();
        }

        await _eventsApi.EmergencyRefundTicketAsync(eventId, Input.TicketId, current, Input.Reason);
        StatusMessage = "بازگشت وجه اضطراری ثبت شد و تراکنش های مالی به روز شدند.";
        return RedirectToPage(new { eventId, Search, Status, Gender });
    }

    public string GetTicketStatusClass(EventTicketBuyerItem item)
    {
        if (item.IsRemoved)
            return "status-cancelled";

        return item.IsRefunded ? "status-closed" : "status-approved";
    }

    private async Task LoadBuyersAsync(long eventId)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        var buyers = await _eventsApi.GetEventTicketBuyersAsync(eventId, current);

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var search = Search.Trim();
            buyers = buyers
                .Where(item =>
                    item.DisplayName.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || item.City.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || item.Country.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || (IsAdmin && item.MobileNumber?.Contains(search, StringComparison.OrdinalIgnoreCase) == true))
                .ToList();
        }

        buyers = Status switch
        {
            "active" => buyers.Where(item => !item.IsRefunded && !item.IsRemoved).ToList(),
            "refunded" => buyers.Where(item => item.IsRefunded).ToList(),
            "removed" => buyers.Where(item => item.IsRemoved).ToList(),
            _ => buyers
        };

        buyers = Gender switch
        {
            "male" => buyers.Where(item => item.Gender == Randevoo.Domain.Enums.Gender.Male).ToList(),
            "female" => buyers.Where(item => item.Gender == Randevoo.Domain.Enums.Gender.Female).ToList(),
            _ => buyers
        };

        Buyers = buyers;
    }
}
