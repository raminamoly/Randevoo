using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.DatingEvents.Common;

public record DatingEventDto(
    long Id,
    string Title,
    long EventTypeId,
    string EventType,
    string Country,
    string City,
    string Address,
    DateTime DateTimeStart,
    DateTime DateTimeEnd,
    bool IsOpenForSell,
    bool IsCancelled,
    EventReviewStatus ReviewStatus,
    EventOperationalStatus OperationalStatus,
    long EventPlannerUserId,
    decimal EventPlannerCommissionPercent,
    EventPaymentCollectionMethod PaymentCollectionMethod,
    string? OrganizerPaymentInstructions,
    int MaleCapacity,
    int FemaleCapacity,
    int NumberOfLikesAllowed,
    decimal MaleTicketPrice,
    string MaleTicketCurrencyCode,
    decimal FemaleTicketPrice,
    string FemaleTicketCurrencyCode,
    EventEducationLevelRestriction EducationLevelRestriction,
    IReadOnlyList<string> Tags,
    string EventDescriptionHtml)
{
    public static DatingEventDto FromEntity(DatingEvent datingEvent) =>
        new(
            datingEvent.Id,
            datingEvent.Title,
            datingEvent.EventTypeId,
            datingEvent.EventType.Name,
            datingEvent.Country?.Name ?? LookupCountryName(datingEvent.CountryId) ?? datingEvent.Location.Country,
            datingEvent.City?.Name ?? LookupCityName(datingEvent.CityId) ?? datingEvent.Location.City,
            datingEvent.Address,
            datingEvent.DateTimeStart,
            datingEvent.DateTimeEnd,
            datingEvent.IsOpenForSell,
            datingEvent.IsCancelled,
            datingEvent.ReviewStatus,
            datingEvent.ResolveOperationalStatus(DateTime.UtcNow),
            datingEvent.EventPlannerUserId,
            datingEvent.EventPlannerCommissionPercent,
            datingEvent.PaymentCollectionMethod,
            datingEvent.OrganizerPaymentInstructions,
            datingEvent.MaleCapacity,
            datingEvent.FemaleCapacity,
            datingEvent.NumberOfLikesAllowed,
            datingEvent.MaleTicketPrice,
            datingEvent.MaleTicketCurrencyCode,
            datingEvent.FemaleTicketPrice,
            datingEvent.FemaleTicketCurrencyCode,
            datingEvent.EducationLevelRestriction,
            datingEvent.Tags,
            datingEvent.EventDescriptionHtml);

    private static string? LookupCountryName(long? countryId) => countryId switch
    {
        1 => "Iran",
        2 => "United Arab Emirates",
        3 => "Turkey",
        _ => null
    };

    private static string? LookupCityName(long? cityId) => cityId switch
    {
        1 => "Tehran",
        2 => "Mashhad",
        3 => "Shiraz",
        4 => "Isfahan",
        5 => "Tabriz",
        6 => "Dubai",
        7 => "Abu Dhabi",
        8 => "Istanbul",
        9 => "Ankara",
        _ => null
    };
}
