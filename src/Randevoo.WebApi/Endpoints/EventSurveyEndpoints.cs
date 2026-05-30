using System.Security.Claims;
using MediatR;
using Randevoo.Application.Features.EventSurveys.Commands.SubmitEventSurvey;
using Randevoo.Application.Features.EventSurveys.Queries.GetMyEventSurvey;
using Randevoo.Domain.Exceptions;

namespace Randevoo.WebApi.Endpoints;

public static class EventSurveyEndpoints
{
    public static RouteGroupBuilder MapEventSurveyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/event-surveys")
            .RequireAuthorization("EndUserOnly")
            .WithTags("Event Surveys");

        group.MapGet("/events/{eventId:long}/me", GetMineAsync).WithName("GetMyEventSurvey");
        group.MapPost("/events/{eventId:long}/me", SubmitAsync).WithName("SubmitEventSurvey");
        return group;
    }

    private static async Task<IResult> GetMineAsync(long eventId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new GetMyEventSurveyQuery(EndpointHelpers.GetUserId(principal), eventId), cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> SubmitAsync(long eventId, SubmitSurveyRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new SubmitEventSurveyCommand(EndpointHelpers.GetUserId(principal), eventId, request.Ratings, request.Comment), cancellationToken));
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    public record SubmitSurveyRequest(IReadOnlyList<SurveyRatingInput> Ratings, string? Comment);
}
