using System.Security.Claims;
using MediatR;
using Randevoo.Application.EndUsers.Events;
using Randevoo.Domain.Exceptions;

namespace Randevoo.WebApi.Endpoints;

public static class EndUserEventEndpoints
{
    public static RouteGroupBuilder MapEndUserEventEndpoints(this IEndpointRouteBuilder app)
    {
        var websiteGroup = app.MapGroup("/api/v1/website/events")
            .WithTags("Website Events");

        websiteGroup.MapGet("/", ListWebsiteAsync).WithName("ListWebsiteEvents");
        websiteGroup.MapGet("/{eventId:long}", GetWebsiteDetailsAsync).WithName("GetWebsiteEventDetails");

        var platformGroup = app.MapGroup("/api/v1/platform/events")
            .RequireAuthorization("EndUserOnly")
            .WithTags("Platform Events");

        platformGroup.MapGet("/", ListPlatformAsync).WithName("ListPlatformEvents");
        platformGroup.MapGet("/{eventId:long}", GetPlatformDetailsAsync).WithName("GetPlatformEventDetails");
        return platformGroup;
    }

    private static Task<IResult> ListWebsiteAsync(
        int? page,
        int? pageSize,
        long? cityId,
        bool? includeOnline,
        bool? includeInPerson,
        long? eventTypeId,
        DateTime? dateFromUtc,
        DateTime? dateToUtc,
        decimal? priceMin,
        decimal? priceMax,
        int? age,
        long? educationLevelId,
        EndUserEventSort? sort,
        ISender sender,
        CancellationToken cancellationToken) =>
        ListAsync(page, pageSize, cityId, includeOnline, includeInPerson, eventTypeId, dateFromUtc, dateToUtc, priceMin, priceMax, age, educationLevelId, false, sort, null, sender, cancellationToken);

    private static Task<IResult> ListPlatformAsync(
        int? page,
        int? pageSize,
        long? cityId,
        bool? includeOnline,
        bool? includeInPerson,
        long? eventTypeId,
        DateTime? dateFromUtc,
        DateTime? dateToUtc,
        decimal? priceMin,
        decimal? priceMax,
        int? age,
        long? educationLevelId,
        bool? onlyEligibleForMe,
        EndUserEventSort? sort,
        ClaimsPrincipal principal,
        ISender sender,
        CancellationToken cancellationToken) =>
        ListAsync(page, pageSize, cityId, includeOnline, includeInPerson, eventTypeId, dateFromUtc, dateToUtc, priceMin, priceMax, age, educationLevelId, onlyEligibleForMe ?? false, sort, EndpointHelpers.GetUserId(principal), sender, cancellationToken);

    private static async Task<IResult> ListAsync(
        int? page,
        int? pageSize,
        long? cityId,
        bool? includeOnline,
        bool? includeInPerson,
        long? eventTypeId,
        DateTime? dateFromUtc,
        DateTime? dateToUtc,
        decimal? priceMin,
        decimal? priceMax,
        int? age,
        long? educationLevelId,
        bool onlyEligibleForMe,
        EndUserEventSort? sort,
        long? userId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new ListEndUserEventsQuery(new EndUserEventCatalogRequest(
                userId,
                page ?? 1,
                pageSize ?? 20,
                cityId,
                includeOnline ?? true,
                includeInPerson ?? true,
                eventTypeId,
                dateFromUtc,
                dateToUtc,
                priceMin,
                priceMax,
                age,
                educationLevelId,
                onlyEligibleForMe,
                sort ?? EndUserEventSort.Recommended)), cancellationToken);

            return Results.Ok(result);
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static Task<IResult> GetWebsiteDetailsAsync(
        long eventId,
        ISender sender,
        CancellationToken cancellationToken) =>
        GetDetailsAsync(eventId, null, sender, cancellationToken);

    private static Task<IResult> GetPlatformDetailsAsync(
        long eventId,
        ClaimsPrincipal principal,
        ISender sender,
        CancellationToken cancellationToken) =>
        GetDetailsAsync(eventId, EndpointHelpers.GetUserId(principal), sender, cancellationToken);

    private static async Task<IResult> GetDetailsAsync(
        long eventId,
        long? userId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new GetEndUserEventDetailsQuery(eventId, userId), cancellationToken);
            return Results.Ok(result);
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }
}
