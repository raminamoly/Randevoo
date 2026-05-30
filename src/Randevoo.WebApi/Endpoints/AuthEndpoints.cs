using System.Security.Claims;
using MediatR;
using Randevoo.Application.Features.Auth.Commands.ConfirmEmail;
using Randevoo.Application.Features.Auth.Commands.RefreshAccessToken;
using Randevoo.Application.Features.Auth.Commands.RequestEmailConfirmation;
using Randevoo.Application.Features.Auth.Commands.RequestMobileLoginCode;
using Randevoo.Application.Features.Auth.Commands.RevokeRefreshToken;
using Randevoo.Application.Features.Auth.Commands.VerifyMobileLoginCode;
using Randevoo.Domain.Exceptions;

namespace Randevoo.WebApi.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        group.MapPost("/mobile/request-code", RequestMobileCodeAsync)
            .WithName("RequestMobileLoginCode");

        group.MapPost("/mobile/verify-code", VerifyMobileCodeAsync)
            .WithName("VerifyMobileLoginCode");

        group.MapPost("/refresh-token", RefreshTokenAsync)
            .WithName("RefreshAccessToken");

        group.MapPost("/logout", LogoutAsync)
            .WithName("Logout");

        group.MapPost("/email/request-confirmation", RequestEmailConfirmationAsync)
            .RequireAuthorization()
            .WithName("RequestEmailConfirmation");

        group.MapGet("/email/confirm", ConfirmEmailAsync)
            .WithName("ConfirmEmail");

        return group;
    }

    private static async Task<IResult> RequestMobileCodeAsync(RequestMobileCodeRequest request, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new RequestMobileLoginCodeCommand(request.MobileNumber), cancellationToken);
            return Results.Accepted();
        }
        catch (DomainException ex)
        {
            return ToProblem(ex);
        }
    }

    private static async Task<IResult> VerifyMobileCodeAsync(VerifyMobileCodeRequest request, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new VerifyMobileLoginCodeCommand(request.MobileNumber, request.Code), cancellationToken));
        }
        catch (DomainException ex)
        {
            return ToProblem(ex);
        }
    }

    private static async Task<IResult> RefreshTokenAsync(RefreshTokenRequest request, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new RefreshAccessTokenCommand(request.RefreshToken), cancellationToken));
        }
        catch (DomainException ex)
        {
            return ToProblem(ex);
        }
    }

    private static async Task<IResult> LogoutAsync(RefreshTokenRequest request, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new RevokeRefreshTokenCommand(request.RefreshToken), cancellationToken);
            return Results.NoContent();
        }
        catch (DomainException ex)
        {
            return ToProblem(ex);
        }
    }

    private static async Task<IResult> RequestEmailConfirmationAsync(
        RequestEmailConfirmationRequest request,
        ClaimsPrincipal principal,
        HttpRequest httpRequest,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId(principal);
            var baseUrl = $"{httpRequest.Scheme}://{httpRequest.Host}";
            await sender.Send(new RequestEmailConfirmationCommand(userId, request.Email, baseUrl), cancellationToken);
            return Results.Accepted();
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return ToProblem(ex);
        }
    }

    private static async Task<IResult> ConfirmEmailAsync(long userId, string token, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new ConfirmEmailCommand(userId, token), cancellationToken);
            return Results.Ok(new { message = "Email confirmed." });
        }
        catch (DomainException ex)
        {
            return ToProblem(ex);
        }
    }

    private static long GetUserId(ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(value, out var userId))
            throw new UnauthorizedAccessException("Authenticated user id was not found.");

        return userId;
    }

    private static IResult ToProblem(Exception ex)
    {
        return ex switch
        {
            UnauthorizedAccessException => Results.Problem(ex.Message, statusCode: StatusCodes.Status401Unauthorized),
            NotFoundException => Results.Problem(ex.Message, statusCode: StatusCodes.Status404NotFound),
            BusinessRuleViolationException => Results.Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest),
            InvalidEntityStateException => Results.Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest),
            _ => Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    public record RequestMobileCodeRequest(string MobileNumber);
    public record VerifyMobileCodeRequest(string MobileNumber, string Code);
    public record RefreshTokenRequest(string RefreshToken);
    public record RequestEmailConfirmationRequest(string Email);
}
