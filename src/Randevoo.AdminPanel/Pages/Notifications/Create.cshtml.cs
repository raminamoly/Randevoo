using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Notifications;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Notifications;

public class CreateModel : PageModel
{
    private readonly INotificationsApiClient _notificationsApi;
    private readonly CurrentSessionState _session;

    public CreateModel(INotificationsApiClient notificationsApi, CurrentSessionState session)
    {
        _notificationsApi = notificationsApi;
        _session = session;
    }

    [BindProperty]
    public NotificationCreateInput Input { get; set; } = new();

    public IReadOnlyList<NotificationMessageTypeOption> MessageTypes { get; private set; } = Array.Empty<NotificationMessageTypeOption>();
    public IReadOnlyList<NotificationPriorityOption> Priorities { get; private set; } = Array.Empty<NotificationPriorityOption>();
    public IReadOnlyList<NotificationTargetOption> Targets { get; private set; } = Array.Empty<NotificationTargetOption>();
    public IReadOnlyList<NotificationEventOption> Events { get; private set; } = Array.Empty<NotificationEventOption>();
    public IReadOnlyList<NotificationUserOption> Users { get; private set; } = Array.Empty<NotificationUserOption>();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadOptionsAsync(cancellationToken);
        var firstType = MessageTypes.FirstOrDefault();
        if (firstType is not null)
        {
            Input.Type = firstType.Type;
            Input.Priority = firstType.DefaultPriority;
            Input.Target = firstType.AllowedTargets.FirstOrDefault() ?? Targets.FirstOrDefault()?.Value ?? "User";
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await LoadOptionsAsync(cancellationToken);
        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _notificationsApi.CreateNotificationAsync(CurrentUser(), Input, cancellationToken);
            StatusMessage = "پیام ثبت شد. اگر نیاز به تایید داشته باشد در صف تایید قرار می‌گیرد.";
            return RedirectToPage("/Notifications/Index");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }

    public async Task<IActionResult> OnGetUsersAsync(long? eventId, string? search, CancellationToken cancellationToken)
    {
        var users = await _notificationsApi.SearchUserOptionsAsync(CurrentUser(), eventId, search, cancellationToken);
        return new JsonResult(users.Select(item => new
        {
            item.Id,
            item.DisplayText
        }));
    }

    public IEnumerable<SelectListItem> MessageTypeItems => MessageTypes.Select(item => new SelectListItem(item.Label, item.Type.ToString()));
    public IEnumerable<SelectListItem> PriorityItems => Priorities.Select(item => new SelectListItem(item.Label, item.Priority.ToString()));
    public IEnumerable<SelectListItem> TargetItems => Targets.Select(item => new SelectListItem(item.Label, item.Value));
    public IEnumerable<SelectListItem> EventItems => Events.Select(item => new SelectListItem(item.DisplayText, item.Id.ToString()));
    public IEnumerable<SelectListItem> UserItems => Users.Select(item => new SelectListItem(item.DisplayText, item.Id.ToString()));

    private async Task LoadOptionsAsync(CancellationToken cancellationToken)
    {
        var currentUser = CurrentUser();
        MessageTypes = await _notificationsApi.GetMessageTypeOptionsAsync(currentUser, cancellationToken);
        Priorities = await _notificationsApi.GetPriorityOptionsAsync(cancellationToken);
        Targets = await _notificationsApi.GetTargetOptionsAsync(currentUser, cancellationToken);
        Events = await _notificationsApi.SearchEventOptionsAsync(currentUser, null, cancellationToken);
        Users = await _notificationsApi.SearchUserOptionsAsync(currentUser, Input.EventId, null, cancellationToken);
    }

    private MockUser CurrentUser() => _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
}
