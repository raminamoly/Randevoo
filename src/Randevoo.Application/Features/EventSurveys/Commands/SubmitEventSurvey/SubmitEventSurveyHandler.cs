using MediatR;
using Randevoo.Application.Features.EventSurveys.Common;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.EventSurveys.Commands.SubmitEventSurvey;

public class SubmitEventSurveyHandler : IRequestHandler<SubmitEventSurveyCommand, EventSurveyDto>
{
    private readonly IUserRepository _users;
    private readonly IDatingEventRepository _events;
    private readonly IEventTicketRepository _tickets;
    private readonly IEventSurveyRepository _surveys;
    private readonly IEventPlannerProfileRepository _plannerProfiles;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitEventSurveyHandler(
        IUserRepository users,
        IDatingEventRepository events,
        IEventTicketRepository tickets,
        IEventSurveyRepository surveys,
        IEventPlannerProfileRepository plannerProfiles,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _events = events;
        _tickets = tickets;
        _surveys = surveys;
        _plannerProfiles = plannerProfiles;
        _unitOfWork = unitOfWork;
    }

    public async Task<EventSurveyDto> Handle(SubmitEventSurveyCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);
        var datingEvent = await _events.GetByIdAsync(request.EventId, cancellationToken)
            ?? throw new NotFoundException("DatingEvent", request.EventId);
        var ticket = await _tickets.GetByEventAndUserAsync(request.EventId, request.UserId, cancellationToken)
            ?? throw new BusinessRuleViolationException("Ticket required", "User must have a valid ticket to submit event survey");

        if (!ticket.IsValidForEventAccess)
            throw new BusinessRuleViolationException("Ticket is not valid", "Refunded or removed tickets cannot submit survey");

        if (datingEvent.DateTimeEnd > DateTime.UtcNow)
            throw new BusinessRuleViolationException("Event has not ended", "Survey can be submitted after event end time");

        var ratings = request.Ratings.Select(rating => new EventSurveyRatingInput(rating.Factor, rating.Score)).ToList();
        var survey = await _surveys.GetByEventAndUserAsync(request.EventId, request.UserId, cancellationToken);
        if (survey is null)
        {
            survey = new EventSurveyResponse(datingEvent, user, ratings, request.Comment);
            await _surveys.AddAsync(survey, cancellationToken);
        }
        else
        {
            survey.UpdateRatings(ratings, request.Comment);
            await _surveys.UpdateAsync(survey, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var plannerProfile = await _plannerProfiles.GetByUserIdAsync(datingEvent.EventPlannerUserId, cancellationToken);
        if (plannerProfile is not null)
        {
            var quality = await _surveys.GetPlannerQualityAsync(datingEvent.EventPlannerUserId, cancellationToken);
            plannerProfile.UpdateMetrics(
                quality.AverageRating,
                quality.SurveyCount,
                await _events.CountByPlannerAsync(datingEvent.EventPlannerUserId, cancellationToken),
                await _events.CountCancelledByPlannerAsync(datingEvent.EventPlannerUserId, cancellationToken),
                await _events.CountCompletedByPlannerAsync(datingEvent.EventPlannerUserId, DateTime.UtcNow, cancellationToken));
            await _plannerProfiles.UpdateAsync(plannerProfile, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return EventSurveyDto.FromEntity(survey);
    }
}
