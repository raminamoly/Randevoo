using MediatR;
using Randevoo.Application.Features.DatingEvents.Common;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Domain.ValueObjects;

namespace Randevoo.Application.Features.DatingEvents.Commands.CreateDatingEvent;

public class CreateDatingEventHandler : IRequestHandler<CreateDatingEventCommand, DatingEventDto>
{
    private readonly IUserRepository _users;
    private readonly IEventPlannerProfileRepository _plannerProfiles;
    private readonly IDatingEventRepository _events;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDatingEventHandler(IUserRepository users, IEventPlannerProfileRepository plannerProfiles, IDatingEventRepository events, IUnitOfWork unitOfWork)
    {
        _users = users;
        _plannerProfiles = plannerProfiles;
        _events = events;
        _unitOfWork = unitOfWork;
    }

    public async Task<DatingEventDto> Handle(CreateDatingEventCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.EventPlannerUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.EventPlannerUserId);

        if (await _plannerProfiles.GetByUserIdAsync(request.EventPlannerUserId, cancellationToken) is null && user.Role != Randevoo.Domain.Enums.UserRole.Admin)
            throw new BusinessRuleViolationException("Missing event planner profile", "Create event planner profile before creating events");

        var input = request.Input;
        var datingEvent = new DatingEvent(
            user,
            input.Title,
            new Location(input.Country, input.City, new Coordinates(input.Latitude, input.Longitude), input.Region),
            input.Address,
            input.DateTimeStart,
            input.DateTimeEnd,
            input.EventType,
            new AgeRange(input.MaleMinAge, input.MaleMaxAge),
            new AgeRange(input.FemaleMinAge, input.FemaleMaxAge),
            input.MaleCapacity,
            input.FemaleCapacity,
            input.NumberOfChatAllowed,
            input.TicketPrice,
            input.EventImage1,
            input.EventImage2,
            input.EventImage3,
            input.EventDescriptionHtml,
            input.EventPlannerCommissionPercent ?? 10);

        await _events.AddAsync(datingEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return DatingEventDto.FromEntity(datingEvent);
    }
}
