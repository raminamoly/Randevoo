using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class EventSurveyResponse : BaseEntity, IAggregateRoot
{
    private readonly List<EventSurveyRating> _ratings = new();

    public long DatingEventId { get; private set; }
    public DatingEvent DatingEvent { get; private set; } = null!;
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string? Comment { get; private set; }
    public IReadOnlyList<EventSurveyRating> Ratings => _ratings.AsReadOnly();

    private EventSurveyResponse() { }

    public EventSurveyResponse(DatingEvent datingEvent, User user, IEnumerable<EventSurveyRatingInput> ratings, string? comment)
    {
        DatingEvent = GuardAgainst.Object.Null(datingEvent, nameof(datingEvent));
        User = GuardAgainst.Object.Null(user, nameof(user));
        DatingEventId = datingEvent.Id;
        UserId = user.Id;
        UpdateRatings(ratings, comment);
        AddDomainEvent(new EntityCreatedEvent<EventSurveyResponse>(this));
    }

    public void UpdateRatings(IEnumerable<EventSurveyRatingInput> ratings, string? comment)
    {
        var ratingList = ratings.ToList();
        var requiredFactors = Enum.GetValues<SurveyFactor>();
        if (ratingList.Select(r => r.Factor).Distinct().Count() != requiredFactors.Length ||
            requiredFactors.Any(factor => ratingList.All(r => r.Factor != factor)))
        {
            throw new BusinessRuleViolationException("Invalid survey", "Survey must include all 5 rating factors");
        }

        _ratings.Clear();
        foreach (var rating in ratingList)
            _ratings.Add(new EventSurveyRating(this, rating.Factor, rating.Score));

        Comment = string.IsNullOrWhiteSpace(comment)
            ? null
            : GuardAgainst.String.MaxLength(comment, nameof(comment), 2000);
        UpdateTimestamp();
    }
}

public record EventSurveyRatingInput(SurveyFactor Factor, int Score);
