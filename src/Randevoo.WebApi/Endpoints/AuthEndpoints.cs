using MediatR;
using Randevoo.Application.Features.Auth.Commands.RequestMobileLoginCode;
using Randevoo.Application.Features.Auth.Commands.VerifyMobileLoginCode;
using Randevoo.Domain.Exceptions;

namespace Randevoo.WebApi.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/platform/auth/mobile")
            .WithTags("Platform Authentication");

        group.MapPost("/request-code", RequestMobileCodeAsync)
            .AllowAnonymous()
            .WithName("PlatformRequestMobileLoginCode");

        group.MapPost("/verify", VerifyMobileCodeAsync)
            .AllowAnonymous()
            .WithName("PlatformVerifyMobileLoginCode");

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
}
