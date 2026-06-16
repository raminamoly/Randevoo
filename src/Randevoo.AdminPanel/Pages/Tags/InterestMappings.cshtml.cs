using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Services.ApiClients;

namespace Randevoo.AdminPanel.Pages.Tags;

[Authorize(Policy = Policies.AdminOnly)]
public class InterestMappingsModel : PageModel
{
    private readonly IEventTagsApiClient _tagsApi;

    public InterestMappingsModel(IEventTagsApiClient tagsApi)
    {
        _tagsApi = tagsApi;
    }

    [BindProperty]
    public InterestTagMappingInput Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public long? Id { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public IReadOnlyList<InterestTagMappingListItem> Mappings { get; private set; } = Array.Empty<InterestTagMappingListItem>();
    public IReadOnlyList<InterestOption> Interests { get; private set; } = Array.Empty<InterestOption>();
    public IReadOnlyList<TagOption> Tags { get; private set; } = Array.Empty<TagOption>();

    public bool IsEditing => Id is not null;

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);

        if (Id is long mappingId)
        {
            var mapping = await _tagsApi.GetInterestTagMappingAsync(mappingId, cancellationToken);
            if (mapping is null)
                return NotFound();

            Input = new InterestTagMappingInput
            {
                InterestId = mapping.InterestId,
                TagId = mapping.TagId,
                RelevanceWeight = mapping.RelevanceWeight,
                IsActive = mapping.IsActive
            };
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        try
        {
            var saved = await _tagsApi.UpsertInterestTagMappingAsync(Input, Id, cancellationToken);
            StatusMessage = Id is null
                ? $"نگاشت «{saved.InterestName}» به «{saved.TagName}» ایجاد شد."
                : $"نگاشت «{saved.InterestName}» به روز شد.";

            return RedirectToPage("/Tags/InterestMappings");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadAsync(cancellationToken);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(long id, CancellationToken cancellationToken)
    {
        try
        {
            await _tagsApi.DeleteInterestTagMappingAsync(id, cancellationToken);
            StatusMessage = "نگاشت حذف شد.";
        }
        catch (InvalidOperationException ex)
        {
            StatusMessage = ex.Message;
        }

        return RedirectToPage("/Tags/InterestMappings");
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Mappings = await _tagsApi.GetInterestTagMappingsAsync(cancellationToken);
        Interests = await _tagsApi.GetInterestsAsync(cancellationToken);
        Tags = await _tagsApi.GetActiveTagsAsync(cancellationToken);
    }
}
