using MediatR;
using Randevoo.Application.Features.EventSurveys.Common;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.EventSurveys.Commands.SubmitEventSurvey;

public record SubmitEventSurveyCommand(long UserId, long EventId, IReadOnlyList<SurveyRatingInput> Ratings, string? Comment) : IRequest<EventSurveyDto>;

public record SurveyRatingInput(SurveyFactor Factor, int Score);
