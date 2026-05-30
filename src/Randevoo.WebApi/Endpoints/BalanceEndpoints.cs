using System.Security.Claims;
using MediatR;
using Randevoo.Application.Features.Balances.Commands.AdjustBalance;
using Randevoo.Application.Features.Balances.Queries.GetBalance;
using Randevoo.Domain.Exceptions;

namespace Randevoo.WebApi.Endpoints;

public static class BalanceEndpoints
{
    public static RouteGroupBuilder MapBalanceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/balances")
            .RequireAuthorization()
            .WithTags("Balances");

        group.MapGet("/me", GetMineAsync).WithName("GetMyBalance");
        group.MapGet("/{userId:long}", GetByUserIdAsync).RequireAuthorization("AdminOnly").WithName("GetUserBalance");
        group.MapPost("/{userId:long}/adjust", AdjustAsync).RequireAuthorization("AdminOnly").WithName("AdjustUserBalance");
        return group;
    }

    private static async Task<IResult> GetByUserIdAsync(long userId, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new GetBalanceQuery(userId), cancellationToken));
        }
        catch (DomainException ex)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> GetMineAsync(ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new GetBalanceQuery(EndpointHelpers.GetUserId(principal)), cancellationToken));
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> AdjustAsync(long userId, AdjustBalanceRequest request, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new AdjustBalanceCommand(userId, request.Amount, request.Description), cancellationToken));
        }
        catch (DomainException ex)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    public record AdjustBalanceRequest(decimal Amount, string Description);
}
