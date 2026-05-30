using MediatR;
using Randevoo.Application.Features.EventSurveys.Common;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.EventSurveys.Queries.GetMyEventSurvey;

public class GetMyEventSurveyHandler : IRequestHandler<GetMyEventSurveyQuery, EventSurveyDto?>
{
    private readonly IEventSurveyRepository _surveys;

    public GetMyEventSurveyHandler(IEventSurveyRepository surveys)
    {
        _surveys = surveys;
    }

    public async Task<EventSurveyDto?> Handle(GetMyEventSurveyQuery request, CancellationToken cancellationToken)
    {
        var survey = await _surveys.GetByEventAndUserAsync(request.EventId, request.UserId, cancellationToken);
        return survey is null ? null : EventSurveyDto.FromEntity(survey);
    }
}
