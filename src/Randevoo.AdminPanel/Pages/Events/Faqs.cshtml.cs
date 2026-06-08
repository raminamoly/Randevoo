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
public class FaqsModel : PageModel
{
    private readonly IEventsApiClient _eventsApi;
    private readonly CurrentSessionState _session;

    public FaqsModel(IEventsApiClient eventsApi, CurrentSessionState session)
    {
        _eventsApi = eventsApi;
        _session = session;
    }

    [BindProperty(SupportsGet = true)]
    public long EventId { get; set; }

    [BindProperty]
    public List<EventFaqInput> Faqs { get; set; } = new();

    public DatingEvent Event { get; private set; } = new();

    public bool IsRtl => _session.IsRtl;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var result = await LoadEventAsync();
        if (result is not null)
            return result;

        Faqs = Event.ActiveDraft.Faqs.Select(item => new EventFaqInput
        {
            Question = item.Question,
            Answer = item.Answer
        }).ToList();
        EnsureRows();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await LoadEventAsync();
        if (result is not null)
            return result;

        NormalizeAndValidateFaqs();
        if (!ModelState.IsValid)
        {
            EnsureRows();
            return Page();
        }

        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        var draft = Event.ActiveDraft;
        draft.Faqs = Faqs
            .Where(item => !string.IsNullOrWhiteSpace(item.Question) && !string.IsNullOrWhiteSpace(item.Answer))
            .Select(item => new EventFaqInput
            {
                Question = item.Question.Trim(),
                Answer = item.Answer.Trim()
            })
            .ToList();

        await _eventsApi.SaveEventAsync(draft, current, Event.Id, current.Role == AdminRole.Admin ? Event.PlannerUserId : null);
        StatusMessage = "سوالات متداول رویداد ذخیره شد.";
        return RedirectToPage(new { eventId = Event.Id });
    }

    private async Task<IActionResult?> LoadEventAsync()
    {
        Event = await _eventsApi.GetEventAsync(EventId) ?? new DatingEvent();
        if (Event.Id == 0)
            return NotFound();

        return null;
    }

    private void NormalizeAndValidateFaqs()
    {
        Faqs = Faqs
            .Where(item => !string.IsNullOrWhiteSpace(item.Question) || !string.IsNullOrWhiteSpace(item.Answer))
            .Take(10)
            .ToList();

        if (Faqs.Count > 10)
            ModelState.AddModelError(nameof(Faqs), "برای هر رویداد حداکثر 10 سوال متداول می توانید ثبت کنید.");

        for (var index = 0; index < Faqs.Count; index++)
        {
            var hasQuestion = !string.IsNullOrWhiteSpace(Faqs[index].Question);
            var hasAnswer = !string.IsNullOrWhiteSpace(Faqs[index].Answer);
            if (hasQuestion != hasAnswer)
                ModelState.AddModelError($"Faqs[{index}].Question", "برای هر سوال متداول، سوال و پاسخ را با هم وارد کنید.");
        }
    }

    private void EnsureRows()
    {
        Faqs = Faqs
            .Where(item => !string.IsNullOrWhiteSpace(item.Question) || !string.IsNullOrWhiteSpace(item.Answer))
            .Take(10)
            .ToList();

        while (Faqs.Count < 5)
        {
            Faqs.Add(new EventFaqInput());
        }
    }
}
