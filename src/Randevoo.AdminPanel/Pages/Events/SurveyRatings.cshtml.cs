using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Domain.Enums;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Pages.Events;

[Authorize(Policy = Policies.AdminOrPlanner)]
public class SurveyRatingsModel : PageModel
{
    private readonly RandevooDbContext _db;
    private readonly CurrentSessionState _session;

    public SurveyRatingsModel(RandevooDbContext db, CurrentSessionState session)
    {
        _db = db;
        _session = session;
    }

    public long EventId { get; private set; }
    public string EventTitle { get; private set; } = string.Empty;
    public IReadOnlyList<SurveyRatingSummaryItem> Summaries { get; private set; } = Array.Empty<SurveyRatingSummaryItem>();
    public IReadOnlyList<SurveyResponseItem> Responses { get; private set; } = Array.Empty<SurveyResponseItem>();

    public async Task<IActionResult> OnGetAsync(long eventId)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        var datingEvent = await _db.DatingEvents
            .FirstOrDefaultAsync(item => item.Id == eventId);
        if (datingEvent is null)
            return NotFound();

        if (current.Role == AdminRole.EventPlanner && datingEvent.EventPlannerUserId != current.Id)
            return Forbid();

        EventId = datingEvent.Id;
        EventTitle = datingEvent.Title;

        var responses = await _db.EventSurveyResponses
            .Include(item => item.User)
            .ThenInclude(user => user.Profile)
            .Include(item => item.Ratings)
            .Where(item => item.DatingEventId == eventId)
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync();

        Responses = responses.Select(response => new SurveyResponseItem
        {
            UserName = response.User.Profile?.DisplayName ?? response.User.MobileNumber,
            Comment = response.Comment,
            SubmittedAtUtc = DateTime.SpecifyKind(response.CreatedAt, DateTimeKind.Utc),
            AverageScore = response.Ratings.Count == 0 ? 0 : response.Ratings.Average(item => item.Score),
            Ratings = response.Ratings
                .OrderBy(item => item.Factor)
                .Select(item => new SurveyRatingItem
                {
                    Factor = FactorTitle(item.Factor),
                    Score = item.Score
                })
                .ToList()
        }).ToList();

        Summaries = Enum.GetValues<SurveyFactor>()
            .Select(factor =>
            {
                var ratings = responses.SelectMany(item => item.Ratings).Where(item => item.Factor == factor).ToList();
                return new SurveyRatingSummaryItem
                {
                    Factor = FactorTitle(factor),
                    AverageScore = ratings.Count == 0 ? 0 : ratings.Average(item => item.Score),
                    Count = ratings.Count
                };
            })
            .ToList();

        return Page();
    }

    private static string FactorTitle(SurveyFactor factor) => factor switch
    {
        SurveyFactor.OverallExperience => "تجربه کلی",
        SurveyFactor.EventOrganization => "نظم اجرا",
        SurveyFactor.VenueAndLocation => "فضا و لوکیشن",
        SurveyFactor.ParticipantQuality => "کیفیت شرکت کنندگان",
        SurveyFactor.SafetyAndComfort => "امنیت و راحتی",
        _ => factor.ToString()
    };

    public sealed class SurveyRatingSummaryItem
    {
        public string Factor { get; set; } = string.Empty;
        public double AverageScore { get; set; }
        public int Count { get; set; }
    }

    public sealed class SurveyResponseItem
    {
        public string UserName { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public DateTimeOffset SubmittedAtUtc { get; set; }
        public double AverageScore { get; set; }
        public IReadOnlyList<SurveyRatingItem> Ratings { get; set; } = Array.Empty<SurveyRatingItem>();
    }

    public sealed class SurveyRatingItem
    {
        public string Factor { get; set; } = string.Empty;
        public int Score { get; set; }
    }
}
