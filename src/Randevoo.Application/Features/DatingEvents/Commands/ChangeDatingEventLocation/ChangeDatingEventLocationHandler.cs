using MediatR;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Domain.ValueObjects;

namespace Randevoo.Application.Features.DatingEvents.Commands.ChangeDatingEventLocation;

public class ChangeDatingEventLocationHandler : IRequestHandler<ChangeDatingEventLocationCommand>
{
    private readonly IUserRepository _users;
    private readonly IDatingEventRepository _events;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeDatingEventLocationHandler(IUserRepository users, IDatingEventRepository events, IUnitOfWork unitOfWork)
    {
        _users = users;
        _events = events;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ChangeDatingEventLocationCommand request, CancellationToken cancellationToken)
    {
        var actor = await _users.GetByIdAsync(request.ActorUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.ActorUserId);
        var datingEvent = await _events.GetByIdAsync(request.EventId, cancellationToken)
            ?? throw new NotFoundException("DatingEvent", request.EventId);

        if (datingEvent.EventPlannerUserId != actor.Id && actor.Role != UserRole.Admin)
            throw new BusinessRuleViolationException("Access denied", "Only owner or admin can change event location");

        datingEvent.ChangeAddressLocation(
            new Location(request.Country, request.City, new Coordinates(request.Latitude, request.Longitude), request.Region),
            request.Address);
        var (countryId, cityId) = MapLocationIds(request.Country, request.City);
        datingEvent.SetLocationLookup(countryId, cityId);
        await _events.UpdateAsync(datingEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
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
