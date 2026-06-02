using System.Security.Claims;
using MediatR;
using Randevoo.Application.Features.Privacy.Commands.DeleteMyAccount;
using Randevoo.Application.Features.Privacy.Queries.ExportMyData;
using Randevoo.Domain.Exceptions;

namespace Randevoo.WebApi.Endpoints;

public static class PrivacyEndpoints
{
    public static RouteGroupBuilder MapPrivacyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/privacy")
            .RequireAuthorization()
            .WithTags("Privacy");

        group.MapGet("/me/export", ExportMeAsync).WithName("ExportMyData");
        group.MapDelete("/me", DeleteMeAsync).WithName("DeleteMyAccount");

        return group;
    }

    private static async Task<IResult> ExportMeAsync(ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new ExportMyDataQuery(EndpointHelpers.GetUserId(principal)), cancellationToken));
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> DeleteMeAsync(ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new DeleteMyAccountCommand(EndpointHelpers.GetUserId(principal)), cancellationToken);
            return Results.NoContent();
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }
}
