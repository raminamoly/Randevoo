using System.Security.Claims;
using MediatR;
using Randevoo.Application.Features.SupportTickets.Commands.ChangeSupportTicketStatus;
using Randevoo.Application.Features.SupportTickets.Commands.CreateSupportTicket;
using Randevoo.Application.Features.SupportTickets.Commands.ReassignSupportTicket;
using Randevoo.Application.Features.SupportTickets.Commands.ReplyToSupportTicket;
using Randevoo.Application.Features.SupportTickets.Common;
using Randevoo.Application.Features.SupportTickets.Queries.GetSupportTicket;
using Randevoo.Application.Features.SupportTickets.Queries.ListSupportTickets;
using Randevoo.Domain.Constants;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;

namespace Randevoo.WebApi.Endpoints;

public static class SupportTicketEndpoints
{
    public static RouteGroupBuilder MapSupportTicketEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/support-tickets")
            .RequireAuthorization()
            .WithTags("Support Tickets");

        group.MapPost("/", CreateAsync).WithName("CreateSupportTicket");
        group.MapGet("/", ListMineAsync).WithName("ListMySupportTickets");
        group.MapGet("/staff", ListStaffAsync).RequireAuthorization("SupportOrAdmin").WithName("ListSupportTicketsForStaff");
        group.MapGet("/{ticketId:long}", GetAsync).WithName("GetSupportTicket");
        group.MapPost("/{ticketId:long}/replies", ReplyAsync).WithName("ReplyToSupportTicket");
        group.MapPut("/{ticketId:long}/status", ChangeStatusAsync).RequireAuthorization("SupportOrAdmin").WithName("ChangeSupportTicketStatus");
        group.MapPut("/{ticketId:long}/assignee", ReassignAsync).RequireAuthorization("AdminOnly").WithName("ReassignSupportTicket");

        return group;
    }

    private static async Task<IResult> CreateAsync(CreateSupportTicketRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new CreateSupportTicketCommand(
                EndpointHelpers.GetUserId(principal),
                request.Title,
                request.TicketTypeId ?? (request.Category is null ? SupportTicketLookupIds.TypeGeneralQuestion : SupportTicketLookupIds.FromCategory(request.Category.Value)),
                request.TicketRecipientTypeId ?? SupportTicketLookupIds.RecipientPlatformSupport,
                request.EventId,
                request.Body,
                request.Attachments ?? Array.Empty<SupportTicketAttachmentInput>()), cancellationToken);
            return Results.Created($"/api/support-tickets/{result.Id}", result);
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> ListMineAsync(SupportTicketStatus? status, SupportTicketCategory? category, long? ticketStatusId, long? ticketTypeId, long? ticketRecipientTypeId, int? limit, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new ListSupportTicketsQuery(
                EndpointHelpers.GetUserId(principal),
                ticketStatusId ?? (status is null ? null : SupportTicketLookupIds.FromStatus(status.Value)),
                ticketTypeId ?? (category is null ? null : SupportTicketLookupIds.FromCategory(category.Value)),
                ticketRecipientTypeId,
                null,
                null,
                null,
                null,
                limit ?? 100), cancellationToken));
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> ListStaffAsync(
        SupportTicketStatus? status,
        SupportTicketCategory? category,
        long? ticketStatusId,
        long? ticketTypeId,
        long? ticketRecipientTypeId,
        UserRole? submitterRole,
        long? assigneeUserId,
        DateTime? createdFromUtc,
        DateTime? createdToUtc,
        int? limit,
        ClaimsPrincipal principal,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new ListSupportTicketsQuery(
                EndpointHelpers.GetUserId(principal),
                ticketStatusId ?? (status is null ? null : SupportTicketLookupIds.FromStatus(status.Value)),
                ticketTypeId ?? (category is null ? null : SupportTicketLookupIds.FromCategory(category.Value)),
                ticketRecipientTypeId,
                submitterRole,
                assigneeUserId,
                createdFromUtc,
                createdToUtc,
                limit ?? 100), cancellationToken));
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> GetAsync(long ticketId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new GetSupportTicketQuery(EndpointHelpers.GetUserId(principal), ticketId), cancellationToken));
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> ReplyAsync(long ticketId, ReplySupportTicketRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new ReplyToSupportTicketCommand(
                EndpointHelpers.GetUserId(principal),
                ticketId,
                request.Body,
                request.Attachments ?? Array.Empty<SupportTicketAttachmentInput>(),
                request.RepresentedUserId), cancellationToken));
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> ChangeStatusAsync(long ticketId, ChangeSupportTicketStatusRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new ChangeSupportTicketStatusCommand(
                EndpointHelpers.GetUserId(principal),
                ticketId,
                request.TicketStatusId ?? (request.Status is null ? SupportTicketLookupIds.StatusOpen : SupportTicketLookupIds.FromStatus(request.Status.Value)),
                request.Note), cancellationToken));
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> ReassignAsync(long ticketId, ReassignSupportTicketRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new ReassignSupportTicketCommand(
                EndpointHelpers.GetUserId(principal),
                ticketId,
                request.AssigneeUserId,
                request.Note), cancellationToken));
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    public record CreateSupportTicketRequest(string Title, long? TicketTypeId, long? TicketRecipientTypeId, long? EventId, SupportTicketCategory? Category, string Body, IReadOnlyList<SupportTicketAttachmentInput>? Attachments);
    public record ReplySupportTicketRequest(string Body, IReadOnlyList<SupportTicketAttachmentInput>? Attachments, long? RepresentedUserId);
    public record ChangeSupportTicketStatusRequest(long? TicketStatusId, SupportTicketStatus? Status, string? Note);
    public record ReassignSupportTicketRequest(long? AssigneeUserId, string? Note);
}
