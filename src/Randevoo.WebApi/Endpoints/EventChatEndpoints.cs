using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Randevoo.Application.Features.EventChats.Commands.BlockEventChatUser;
using Randevoo.Application.Features.EventChats.Commands.SendEventChatMessage;
using Randevoo.Application.Features.EventChats.Commands.StartEventConversation;
using Randevoo.Application.Features.EventChats.Queries.ListMyEventConversations;
using Randevoo.Domain.Exceptions;
using Randevoo.WebApi.Hubs;

namespace Randevoo.WebApi.Endpoints;

public static class EventChatEndpoints
{
    public static RouteGroupBuilder MapEventChatEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/event-chats")
            .RequireAuthorization("EndUserOnly")
            .WithTags("Event Chats");

        group.MapGet("/me/conversations", ListMineAsync).WithName("ListMyEventConversations");
        group.MapPost("/events/{eventId:long}/conversations", StartConversationAsync).WithName("StartEventConversation");
        group.MapPost("/conversations/{conversationId:long}/messages", SendMessageAsync).WithName("SendEventChatMessage");
        group.MapPost("/conversations/{conversationId:long}/blocks", BlockUserAsync).WithName("BlockEventChatUser");
        return group;
    }

    private static async Task<IResult> ListMineAsync(ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new ListMyEventConversationsQuery(EndpointHelpers.GetUserId(principal)), cancellationToken));
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> StartConversationAsync(long eventId, StartConversationRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new StartEventConversationCommand(EndpointHelpers.GetUserId(principal), eventId, request.ParticipantUserId), cancellationToken);
            return Results.Created($"/api/event-chats/conversations/{result.Id}", result);
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> SendMessageAsync(
        long conversationId,
        SendMessageRequest request,
        ClaimsPrincipal principal,
        ISender sender,
        IHubContext<EventChatHub> hubContext,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new SendEventChatMessageCommand(EndpointHelpers.GetUserId(principal), conversationId, request.Body), cancellationToken);
            await hubContext.Clients.Groups(EventChatHub.UserGroup(result.StarterUserId), EventChatHub.UserGroup(result.ParticipantUserId))
                .SendAsync("eventConversationUpdated", result, cancellationToken);
            return Results.Ok(result);
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> BlockUserAsync(long conversationId, BlockUserRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new BlockEventChatUserCommand(EndpointHelpers.GetUserId(principal), conversationId, request.BlockedUserId), cancellationToken);
            return Results.NoContent();
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    public record StartConversationRequest(long ParticipantUserId);
    public record SendMessageRequest(string Body);
    public record BlockUserRequest(long BlockedUserId);
}
