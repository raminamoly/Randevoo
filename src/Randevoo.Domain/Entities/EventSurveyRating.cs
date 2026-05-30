using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class EventSurveyRating : BaseEntity
{
    public long EventSurveyResponseId { get; private set; }
    public EventSurveyResponse EventSurveyResponse { get; private set; } = null!;
    public SurveyFactor Factor { get; private set; }
    public int Score { get; private set; }

    private EventSurveyRating() { }

    internal EventSurveyRating(EventSurveyResponse response, SurveyFactor factor, int score)
    {
        EventSurveyResponse = GuardAgainst.Object.Null(response, nameof(response));
        Factor = factor;
        if (score is < 1 or > 5)
            throw new BusinessRuleViolationException("Invalid survey score", "Survey score must be between 1 and 5");

        Score = score;
    }
}
