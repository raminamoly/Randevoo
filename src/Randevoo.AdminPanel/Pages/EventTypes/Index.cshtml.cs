using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Services.ApiClients;

namespace Randevoo.AdminPanel.Pages.EventTypes;

[Authorize(Policy = Policies.AdminOnly)]
public class IndexModel : PageModel
{
    private readonly IEventTypesApiClient _eventTypesApi;

    public IndexModel(IEventTypesApiClient eventTypesApi)
    {
        _eventTypesApi = eventTypesApi;
    }

    [BindProperty]
    public EventTypeEditorInput Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public long? Id { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public IReadOnlyList<EventTypeAdminItem> EventTypes { get; private set; } = Array.Empty<EventTypeAdminItem>();

    public bool IsEditing => Id is not null;

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadAsync();

        if (Id is long eventTypeId)
        {
            var eventType = await _eventTypesApi.GetEventTypeAsync(eventTypeId);
            if (eventType is null)
                return NotFound();

            Input = new EventTypeEditorInput
            {
                Name = eventType.Name,
                Description = eventType.Description,
                IsActive = eventType.IsActive
            };
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ValidateInput();
        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        try
        {
            var saved = await _eventTypesApi.UpsertEventTypeAsync(Input, Id);
            StatusMessage = Id is null
                ? $"نوع رویداد «{saved.Name}» ایجاد شد."
                : $"نوع رویداد «{saved.Name}» به روز شد.";
            return RedirectToPage("/EventTypes/Index");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadAsync();
            return Page();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(long id)
    {
        try
        {
            await _eventTypesApi.DeleteEventTypeAsync(id);
            StatusMessage = "نوع رویداد حذف شد.";
        }
        catch (InvalidOperationException ex)
        {
            StatusMessage = ex.Message;
        }

        return RedirectToPage("/EventTypes/Index");
    }

    private async Task LoadAsync()
    {
        EventTypes = await _eventTypesApi.GetEventTypesAsync();
    }

    private void ValidateInput()
    {
        var normalizedName = (Input.Name ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
            ModelState.AddModelError(nameof(Input.Name), "نام نوع رویداد را وارد کنید.");
        else if (normalizedName.Length is < 2 or > 100)
            ModelState.AddModelError(nameof(Input.Name), "نام نوع رویداد باید بین 2 تا 100 کاراکتر باشد.");

        if (!string.IsNullOrWhiteSpace(Input.Description) && Input.Description.Trim().Length > 500)
            ModelState.AddModelError(nameof(Input.Description), "توضیحات نوع رویداد حداکثر 500 کاراکتر می تواند باشد.");
    }
}
