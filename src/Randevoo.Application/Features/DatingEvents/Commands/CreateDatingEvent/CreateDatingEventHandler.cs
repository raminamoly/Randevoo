using MediatR;
using Microsoft.Extensions.Logging;
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
    private readonly IEventTypeRepository _eventTypes;
    private readonly IDatingEventRepository _events;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateDatingEventHandler> _logger;

    public CreateDatingEventHandler(IUserRepository users, IEventPlannerProfileRepository plannerProfiles, IEventTypeRepository eventTypes, IDatingEventRepository events, IUnitOfWork unitOfWork, ILogger<CreateDatingEventHandler> logger)
    {
        _users = users;
        _plannerProfiles = plannerProfiles;
        _eventTypes = eventTypes;
        _events = events;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DatingEventDto> Handle(CreateDatingEventCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.EventPlannerUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.EventPlannerUserId);

        if (await _plannerProfiles.GetByUserIdAsync(request.EventPlannerUserId, cancellationToken) is null && user.Role != Randevoo.Domain.Enums.UserRole.Admin)
            throw new BusinessRuleViolationException("Missing event planner profile", "Create event planner profile before creating events");

        var input = request.Input;
        var eventType = await _eventTypes.GetByIdAsync(input.EventTypeId, cancellationToken)
            ?? throw new NotFoundException("EventType", input.EventTypeId);

        if (!eventType.IsActive)
            throw new BusinessRuleViolationException("Inactive event type", "Dating event must use an active event type");

        var datingEvent = new DatingEvent(
            user,
            input.Title,
            new Location(input.Country, input.City, new Coordinates(input.Latitude, input.Longitude), input.Region),
            input.Address,
            input.DateTimeStart,
            input.DateTimeEnd,
            eventType,
            new AgeRange(input.MaleMinAge, input.MaleMaxAge),
            new AgeRange(input.FemaleMinAge, input.FemaleMaxAge),
            input.MaleCapacity,
            input.FemaleCapacity,
            input.NumberOfChatAllowed,
            input.TicketPrice,
            input.EducationLevelRestriction,
            input.Tags,
            input.EventImage1,
            input.EventImage2,
            input.EventImage3,
            input.EventDescriptionHtml,
            input.EventPlannerCommissionPercent ?? 10);
        var (countryId, cityId) = MapLocationIds(input.Country, input.City);
        datingEvent.SetLocationLookup(countryId, cityId);

        await _events.AddAsync(datingEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Event planner {EventPlannerUserId} created dating event {EventId}", user.Id, datingEvent.Id);
        return DatingEventDto.FromEntity(datingEvent);
    }

    private static (long? CountryId, long? CityId) MapLocationIds(string countryName, string cityName)
    {
        var countryId = countryName switch
        {
            "ایران" or "Iran" => 1L,
            "امارات متحده عربی" or "UAE" or "United Arab Emirates" => 2L,
            "ترکیه" or "Turkey" => 3L,
            _ => (long?)null
        };

        var cityId = (countryId, cityName) switch
        {
            (1, "تهران" or "Tehran") => 1L,
            (1, "مشهد" or "Mashhad") => 2L,
            (1, "شیراز" or "Shiraz") => 3L,
            (1, "اصفهان" or "Isfahan") => 4L,
            (1, "تبریز" or "Tabriz") => 5L,
            (2, "دبی" or "Dubai") => 6L,
            (2, "ابوظبی" or "Abu Dhabi") => 7L,
            (3, "استانبول" or "Istanbul") => 8L,
            (3, "آنکارا" or "Ankara") => 9L,
            _ => (long?)null
        };

        return (countryId, cityId);
    }
}
