using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Services.ApiClients;

namespace Randevoo.AdminPanel.Pages.Tags;

[Authorize(Policy = Policies.AdminOnly)]
public class IndexModel : PageModel
{
    private readonly IEventTagsApiClient _tagsApi;

    public IndexModel(IEventTagsApiClient tagsApi)
    {
        _tagsApi = tagsApi;
    }

    [BindProperty]
    public TagEditorInput Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public long? Id { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public IReadOnlyList<TagAdminItem> Tags { get; private set; } = Array.Empty<TagAdminItem>();

    public bool IsEditing => Id is not null;

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadAsync();

        if (Id is long tagId)
        {
            var tag = await _tagsApi.GetTagAsync(tagId);
            if (tag is null)
                return NotFound();

            Input = new TagEditorInput
            {
                Name = tag.Name,
                IsActive = tag.IsActive
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
            var saved = await _tagsApi.UpsertTagAsync(Input, Id);
            StatusMessage = Id is null
                ? $"تگ «{saved.Name}» ایجاد شد."
                : $"تگ «{saved.Name}» به روز شد.";
            return RedirectToPage("/Tags/Index");
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
            await _tagsApi.DeleteTagAsync(id);
            StatusMessage = "تگ حذف شد.";
        }
        catch (InvalidOperationException ex)
        {
            StatusMessage = ex.Message;
        }

        return RedirectToPage("/Tags/Index");
    }

    private async Task LoadAsync()
    {
        Tags = await _tagsApi.GetTagsAsync();
    }

    private void ValidateInput()
    {
        var normalizedName = (Input.Name ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
            ModelState.AddModelError(nameof(Input.Name), "نام تگ را وارد کنید.");
        else if (normalizedName.Length is < 2 or > 50)
            ModelState.AddModelError(nameof(Input.Name), "نام تگ باید بین 2 تا 50 کاراکتر باشد.");
    }
}
