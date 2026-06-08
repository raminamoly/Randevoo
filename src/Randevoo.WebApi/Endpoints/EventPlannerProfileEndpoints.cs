using System.Security.Claims;
using MediatR;
using Randevoo.Application.Features.EventPlannerProfiles.Commands.UpsertEventPlannerProfile;
using Randevoo.Domain.Exceptions;

namespace Randevoo.WebApi.Endpoints;

public static class EventPlannerProfileEndpoints
{
    public static RouteGroupBuilder MapEventPlannerProfileEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/event-planner-profile")
            .RequireAuthorization()
            .WithTags("Event Planner Profile");

        group.MapPut("/me", UpsertMineAsync).WithName("UpsertMyEventPlannerProfile");
        return group;
    }

    private static async Task<IResult> UpsertMineAsync(UpsertEventPlannerProfileRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            var userId = EndpointHelpers.GetUserId(principal);
            return Results.Ok(await sender.Send(new UpsertEventPlannerProfileCommand(userId, request.Title, request.PictureUrl, request.Resume, request.SettlementCurrencyCode), cancellationToken));
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    public record UpsertEventPlannerProfileRequest(string Title, string? PictureUrl, string Resume, string SettlementCurrencyCode = "IRR");
}
