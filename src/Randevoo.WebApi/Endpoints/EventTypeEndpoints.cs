using MediatR;
using Randevoo.Application.Features.EventTypes.Commands.UpsertEventType;
using Randevoo.Application.Features.EventTypes.Queries.ListEventTypes;
using Randevoo.Domain.Exceptions;

namespace Randevoo.WebApi.Endpoints;

public static class EventTypeEndpoints
{
    public static RouteGroupBuilder MapEventTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/event-types")
            .WithTags("Event Types");

        group.MapGet("/", ListAsync).WithName("ListEventTypes");
        group.MapPost("/", UpsertAsync).RequireAuthorization("AdminOnly").WithName("CreateEventType");
        group.MapPut("/{id:long}", UpdateAsync).RequireAuthorization("AdminOnly").WithName("UpdateEventType");
        return group;
    }

    private static async Task<IResult> ListAsync(ISender sender, CancellationToken cancellationToken) =>
        Results.Ok(await sender.Send(new ListEventTypesQuery(), cancellationToken));

    private static Task<IResult> UpsertAsync(UpsertEventTypeRequest request, ISender sender, CancellationToken cancellationToken) =>
        SaveAsync(null, request, sender, cancellationToken);

    private static Task<IResult> UpdateAsync(long id, UpsertEventTypeRequest request, ISender sender, CancellationToken cancellationToken) =>
        SaveAsync(id, request, sender, cancellationToken);

    private static async Task<IResult> SaveAsync(long? id, UpsertEventTypeRequest request, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new UpsertEventTypeCommand(id, request.Name, request.Description, request.IsActive), cancellationToken);
            return id is null ? Results.Created($"/api/event-types/{result.Id}", result) : Results.Ok(result);
        }
        catch (DomainException ex)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    public record UpsertEventTypeRequest(string Name, string? Description, bool IsActive = true);
}
