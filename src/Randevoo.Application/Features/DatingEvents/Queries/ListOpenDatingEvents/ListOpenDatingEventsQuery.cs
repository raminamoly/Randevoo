using MediatR;
using Randevoo.Application.Features.DatingEvents.Common;

namespace Randevoo.Application.Features.DatingEvents.Queries.ListOpenDatingEvents;

public record ListOpenDatingEventsQuery(
    int Limit = 50,
    long? AfterId = null,
    string? City = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    long? EventTypeId = null,
    decimal? PriceMin = null,
    decimal? PriceMax = null,
    string? GenderCapacityAvailable = null) : IRequest<IReadOnlyList<DatingEventDto>>;
