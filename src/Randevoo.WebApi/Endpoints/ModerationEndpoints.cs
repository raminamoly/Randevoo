using System.Security.Claims;
using MediatR;
using Randevoo.Application.Features.Moderation.Commands.CreateModerationReport;
using Randevoo.Application.Features.Moderation.Commands.ReviewModerationReport;
using Randevoo.Application.Features.Moderation.Queries.ListModerationReports;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;

namespace Randevoo.WebApi.Endpoints;

public static class ModerationEndpoints
{
    public static RouteGroupBuilder MapModerationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/moderation-reports")
            .RequireAuthorization()
            .WithTags("Moderation Reports");

        group.MapPost("/", CreateAsync).WithName("CreateModerationReport");
        group.MapGet("/", ListMineAsync).WithName("ListMyModerationReports");
        group.MapGet("/admin", ListAdminAsync).RequireAuthorization("AdminOnly").WithName("ListModerationReportsForAdmin");
        group.MapPut("/{reportId:long}/review", ReviewAsync).RequireAuthorization("AdminOnly").WithName("ReviewModerationReport");
        return group;
    }

    private static async Task<IResult> CreateAsync(CreateReportRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new CreateModerationReportCommand(
                EndpointHelpers.GetUserId(principal),
                request.ReportedUserId,
                request.DatingEventId,
                request.EventConversationId,
                request.Reason,
                request.Description), cancellationToken);
            return Results.Created($"/api/moderation-reports/{result.Id}", result);
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> ListMineAsync(ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new ListModerationReportsQuery(EndpointHelpers.GetUserId(principal), false, null), cancellationToken));
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> ListAdminAsync(ModerationReportStatus? status, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new ListModerationReportsQuery(EndpointHelpers.GetUserId(principal), true, status), cancellationToken));
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> ReviewAsync(long reportId, ReviewReportRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new ReviewModerationReportCommand(EndpointHelpers.GetUserId(principal), reportId, request.Status, request.Note), cancellationToken));
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    public record CreateReportRequest(long ReportedUserId, long? DatingEventId, long? EventConversationId, ModerationReportReason Reason, string Description);
    public record ReviewReportRequest(ModerationReportStatus Status, string? Note);
}
