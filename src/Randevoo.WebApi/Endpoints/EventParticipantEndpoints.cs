using System.Security.Claims;
using MediatR;
using Randevoo.Application.Features.EventParticipants.Queries.ListMyTicketOrders;
using Randevoo.Domain.Exceptions;

namespace Randevoo.WebApi.Endpoints;

public static class EventParticipantEndpoints
{
    public static RouteGroupBuilder MapEventParticipantEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/platform/tickets")
            .RequireAuthorization("EndUserOnly")
            .WithTags("Platform Tickets");

        group.MapGet("/", ListMyArchiveAsync).WithName("ListMyPlatformTickets");
        return group;
    }

    private static async Task<IResult> ListMyArchiveAsync(ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new ListMyTicketOrdersQuery(EndpointHelpers.GetUserId(principal)), cancellationToken));
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }
}
