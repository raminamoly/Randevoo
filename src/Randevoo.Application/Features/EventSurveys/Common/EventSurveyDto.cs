using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.EventSurveys.Common;

public record EventSurveyDto(long Id, long EventId, long UserId, string? Comment, IReadOnlyList<EventSurveyRatingDto> Ratings)
{
    public static EventSurveyDto FromEntity(EventSurveyResponse response) =>
        new(
            response.Id,
            response.DatingEventId,
            response.UserId,
            response.Comment,
            response.Ratings.OrderBy(rating => rating.Factor).Select(EventSurveyRatingDto.FromEntity).ToList());
}

public record EventSurveyRatingDto(SurveyFactor Factor, int Score)
{
    public static EventSurveyRatingDto FromEntity(EventSurveyRating rating) => new(rating.Factor, rating.Score);
}
