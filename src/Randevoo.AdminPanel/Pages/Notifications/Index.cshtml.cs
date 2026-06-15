using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Notifications;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Notifications;

public class IndexModel : PageModel
{
    private readonly INotificationsApiClient _notificationsApi;
    private readonly CurrentSessionState _session;

    public IndexModel(INotificationsApiClient notificationsApi, CurrentSessionState session)
    {
        _notificationsApi = notificationsApi;
        _session = session;
    }

    public NotificationListResult Result { get; private set; } = new();
    public IReadOnlyList<NotificationItem> Notifications => Result.Items;
    public IReadOnlyList<NotificationMessageTypeOption> MessageTypes { get; private set; } = Array.Empty<NotificationMessageTypeOption>();
    public IReadOnlyList<NotificationPriorityOption> Priorities { get; private set; } = Array.Empty<NotificationPriorityOption>();
    public IReadOnlyList<NotificationEventOption> Events { get; private set; } = Array.Empty<NotificationEventOption>();

    [BindProperty(SupportsGet = true)]
    public bool UnreadOnly { get; set; }

    [BindProperty(SupportsGet = true)]
    public NotificationListFilter Filter { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadOptionsAsync(cancellationToken);
        if (UnreadOnly)
            Filter.ReadState = "Unread";
        Result = await _notificationsApi.GetMyNotificationsAsync(CurrentUser(), Filter, cancellationToken);
    }

    public async Task<IActionResult> OnPostReadAsync(long id, CancellationToken cancellationToken)
    {
        await _notificationsApi.MarkAsReadAsync(CurrentUser(), id, cancellationToken);
        return RedirectToPage(new { Filter.Page, Filter.PageSize, Filter.ReadState, Filter.Search, Filter.Type, Filter.Priority, Filter.EventId, Filter.SortBy });
    }

    public async Task<IActionResult> OnPostReadAllAsync(CancellationToken cancellationToken)
    {
        await _notificationsApi.MarkAllAsReadAsync(CurrentUser(), cancellationToken);
        StatusMessage = "همه پیام‌ها خوانده شدند.";
        return RedirectToPage();
    }

    public IEnumerable<SelectListItem> MessageTypeItems => MessageTypes.Select(item => new SelectListItem(item.Label, item.Type.ToString()));
    public IEnumerable<SelectListItem> PriorityItems => Priorities.Select(item => new SelectListItem(item.Label, item.Priority.ToString()));
    public IEnumerable<SelectListItem> EventItems => Events.Select(item => new SelectListItem(item.DisplayText, item.Id.ToString()));

    private async Task LoadOptionsAsync(CancellationToken cancellationToken)
    {
        var currentUser = CurrentUser();
        MessageTypes = await _notificationsApi.GetMessageTypeOptionsAsync(currentUser, cancellationToken);
        Priorities = await _notificationsApi.GetPriorityOptionsAsync(cancellationToken);
        Events = await _notificationsApi.SearchEventOptionsAsync(currentUser, null, cancellationToken);
    }

    private MockUser CurrentUser() => _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
}
