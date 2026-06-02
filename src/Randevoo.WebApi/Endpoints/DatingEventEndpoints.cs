using System.Security.Claims;
using MediatR;
using Randevoo.Application.Features.DatingEvents.Commands.BuyDatingEventTicket;
using Randevoo.Application.Features.DatingEvents.Commands.CancelDatingEvent;
using Randevoo.Application.Features.DatingEvents.Commands.ChangeDatingEventLocation;
using Randevoo.Application.Features.DatingEvents.Commands.CreateDatingEvent;
using Randevoo.Application.Features.DatingEvents.Commands.SendSmsToParticipants;
using Randevoo.Application.Features.DatingEvents.Commands.SetDatingEventCommission;
using Randevoo.Application.Features.DatingEvents.Commands.SetDatingEventSaleStatus;
using Randevoo.Application.Features.DatingEvents.Common;
using Randevoo.Application.Features.DatingEvents.Queries.ListOpenDatingEvents;
using Randevoo.Domain.Exceptions;

namespace Randevoo.WebApi.Endpoints;

public static class DatingEventEndpoints
{
    public static RouteGroupBuilder MapDatingEventEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dating-events")
            .WithTags("Dating Events");

        group.MapGet("/open", ListOpenAsync).WithName("ListOpenDatingEvents");
        group.MapPost("/", CreateAsync).RequireAuthorization("EventPlannerOnly").WithName("CreateDatingEvent");
        group.MapPost("/{eventId:long}/open", OpenAsync).RequireAuthorization("EventPlannerOnly").WithName("OpenDatingEvent");
        group.MapPost("/{eventId:long}/close", CloseAsync).RequireAuthorization("EventPlannerOnly").WithName("CloseDatingEvent");
        group.MapPost("/{eventId:long}/cancel", CancelAsync).RequireAuthorization("EventPlannerOnly").WithName("CancelDatingEvent");
        group.MapPut("/{eventId:long}/location", ChangeLocationAsync).RequireAuthorization("EventPlannerOnly").WithName("ChangeDatingEventLocation");
        group.MapPut("/{eventId:long}/commission", SetCommissionAsync).RequireAuthorization("AdminOnly").WithName("SetDatingEventCommission");
        group.MapPost("/{eventId:long}/tickets", BuyTicketAsync).RequireAuthorization("EndUserOnly").WithName("BuyDatingEventTicket");
        group.MapPost("/{eventId:long}/send-sms", SendSmsAsync).RequireAuthorization("EventPlannerOnly").WithName("SendSmsToParticipants");
        return group;
    }

    private static async Task<IResult> ListOpenAsync(
        int? limit,
        long? afterId,
        string? city,
        DateTime? dateFrom,
        DateTime? dateTo,
        long? eventTypeId,
        decimal? priceMin,
        decimal? priceMax,
        string? genderCapacityAvailable,
        ISender sender,
        CancellationToken cancellationToken) =>
        Results.Ok(await sender.Send(new ListOpenDatingEventsQuery(
            limit ?? 50,
            afterId,
            city,
            dateFrom,
            dateTo,
            eventTypeId,
            priceMin,
            priceMax,
            genderCapacityAvailable), cancellationToken));

    private static async Task<IResult> CreateAsync(DatingEventInput request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new CreateDatingEventCommand(EndpointHelpers.GetUserId(principal), request), cancellationToken);
            return Results.Created($"/api/dating-events/{result.Id}", result);
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static Task<IResult> OpenAsync(long eventId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        SetStatusAsync(eventId, true, principal, sender, cancellationToken);

    private static Task<IResult> CloseAsync(long eventId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        SetStatusAsync(eventId, false, principal, sender, cancellationToken);

    private static async Task<IResult> SetStatusAsync(long eventId, bool open, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new SetDatingEventSaleStatusCommand(eventId, EndpointHelpers.GetUserId(principal), open), cancellationToken);
            return Results.NoContent();
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> BuyTicketAsync(long eventId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            var ticketId = await sender.Send(new BuyDatingEventTicketCommand(EndpointHelpers.GetUserId(principal), eventId), cancellationToken);
            return Results.Created($"/api/dating-events/{eventId}/tickets/{ticketId}", new { ticketId });
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> CancelAsync(long eventId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new CancelDatingEventCommand(EndpointHelpers.GetUserId(principal), eventId), cancellationToken);
            return Results.NoContent();
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> ChangeLocationAsync(long eventId, ChangeLocationRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new ChangeDatingEventLocationCommand(
                EndpointHelpers.GetUserId(principal),
                eventId,
                request.Country,
                request.City,
                request.Region,
                request.Latitude,
                request.Longitude,
                request.Address), cancellationToken);
            return Results.NoContent();
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> SetCommissionAsync(long eventId, SetCommissionRequest request, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new SetDatingEventCommissionCommand(eventId, request.CommissionPercent), cancellationToken);
            return Results.NoContent();
        }
        catch (DomainException ex)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> SendSmsAsync(long eventId, SendSmsRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new SendSmsToParticipantsCommand(EndpointHelpers.GetUserId(principal), eventId, request.Message), cancellationToken);
            return Results.Accepted();
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    public record SendSmsRequest(string Message);
    public record ChangeLocationRequest(string Country, string City, string? Region, decimal Latitude, decimal Longitude, string Address);
    public record SetCommissionRequest(decimal CommissionPercent);
}
