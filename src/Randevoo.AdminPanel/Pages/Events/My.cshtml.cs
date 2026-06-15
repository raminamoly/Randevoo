using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;
using DomainEventApprovalStatus = Randevoo.Domain.Enums.EventApprovalStatus;

namespace Randevoo.AdminPanel.Pages.Events;

[Authorize(Policy = Policies.AdminOrPlanner)]
public class MyModel : PageModel
{
    private readonly IEventsApiClient _eventsApi;
    private readonly CurrentSessionState _session;

    public MyModel(IEventsApiClient eventsApi, CurrentSessionState session)
    {
        _eventsApi = eventsApi;
        _session = session;
    }

    public IReadOnlyList<DatingEvent> Events { get; private set; } = Array.Empty<DatingEvent>();

    public bool IsRtl => _session.IsRtl;

    [TempData]
    public string? StatusMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        Events = await _eventsApi.GetEventsAsync(current);
    }

    public async Task<IActionResult> OnGetCancellationPreviewAsync(long id)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        var preview = await _eventsApi.PreviewCancellationAsync(id, current);
        return new JsonResult(preview);
    }

    public async Task<IActionResult> OnPostChangeStatusAsync(long id, EventStatusTransitionAction action, string? note, string? publicMessage, bool cancellationConfirmed, string? returnUrl)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        try
        {
            if (action == EventStatusTransitionAction.CancelEvent)
            {
                await _eventsApi.CancelEventWithChecklistAsync(
                    id,
                    current,
                    note ?? string.Empty,
                    publicMessage ?? string.Empty,
                    cancellationConfirmed);
            }
            else
            {
                await _eventsApi.ApplyStatusTransitionAsync(id, current, action, note);
            }

            StatusMessage = "وضعیت رویداد با موفقیت تغییر کرد.";
        }
        catch (InvalidOperationException ex)
        {
            ErrorMessage = ex.Message;
        }

        return RedirectToSafeReturnUrl(returnUrl);
    }

    public string GetOperationalStatusClass(EventOperationalStatus status) => DisplayFormatter.OperationalStatusClass(status);

    public string GetProfileStatusClass(DomainEventApprovalStatus status) => DisplayFormatter.ApprovalStatusClass(status);

    public EventStatusTransitionModalViewModel CreateStatusTransitionModal(DatingEvent datingEvent)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        return new EventStatusTransitionModalViewModel
        {
            Event = datingEvent,
            Options = EventStatusTransitionCatalog.GetOptions(datingEvent, current.Role),
            EmptyMessage = EventStatusTransitionCatalog.GetEmptyMessage(datingEvent),
            ReturnUrl = Request.Path + Request.QueryString,
            CancellationPreviewUrl = Url.Page(null, "CancellationPreview", new { id = datingEvent.Id }) ?? string.Empty
        };
    }

    private IActionResult RedirectToSafeReturnUrl(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToPage();
    }
}
