using System.Security.Claims;
using MediatR;
using Randevoo.Application.Features.EventParticipants.Commands.RemoveEventParticipant;
using Randevoo.Application.Features.EventParticipants.Queries.ListEventParticipants;
using Randevoo.Application.Features.EventParticipants.Queries.ListMyEventArchive;
using Randevoo.Application.Features.EventParticipants.Queries.ListVisibleParticipantProfiles;
using Randevoo.Domain.Exceptions;

namespace Randevoo.WebApi.Endpoints;

public static class EventParticipantEndpoints
{
    public static RouteGroupBuilder MapEventParticipantEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/event-participants")
            .RequireAuthorization()
            .WithTags("Event Participants");

        group.MapGet("/me/archive", ListMyArchiveAsync).WithName("ListMyEventArchive");
        group.MapGet("/events/{eventId:long}/profiles", ListVisibleProfilesAsync).WithName("ListVisibleParticipantProfiles");
        group.MapGet("/events/{eventId:long}/participants", ListParticipantsAsync).RequireAuthorization("EventPlannerOnly").WithName("ListEventParticipants");
        group.MapPost("/events/{eventId:long}/participants/{participantUserId:long}/remove", RemoveParticipantAsync).RequireAuthorization("EventPlannerOnly").WithName("RemoveEventParticipant");
        return group;
    }

    private static async Task<IResult> ListMyArchiveAsync(ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new ListMyEventArchiveQuery(EndpointHelpers.GetUserId(principal)), cancellationToken));
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> ListVisibleProfilesAsync(long eventId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new ListVisibleParticipantProfilesQuery(EndpointHelpers.GetUserId(principal), eventId), cancellationToken));
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> ListParticipantsAsync(long eventId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new ListEventParticipantsQuery(EndpointHelpers.GetUserId(principal), eventId), cancellationToken));
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> RemoveParticipantAsync(long eventId, long participantUserId, RemoveParticipantRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new RemoveEventParticipantCommand(EndpointHelpers.GetUserId(principal), eventId, participantUserId, request.Reason), cancellationToken);
            return Results.NoContent();
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    public record RemoveParticipantRequest(string Reason);
}
