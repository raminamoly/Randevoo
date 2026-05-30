using MediatR;
using Randevoo.Application.Features.EventSurveys.Common;

namespace Randevoo.Application.Features.EventSurveys.Queries.GetMyEventSurvey;

public record GetMyEventSurveyQuery(long UserId, long EventId) : IRequest<EventSurveyDto?>;
