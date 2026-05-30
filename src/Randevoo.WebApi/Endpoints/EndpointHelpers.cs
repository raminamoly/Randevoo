using System.Security.Claims;
using Randevoo.Domain.Exceptions;

namespace Randevoo.WebApi.Endpoints;

public static class EndpointHelpers
{
    public static long GetUserId(ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(value, out var userId))
            throw new UnauthorizedAccessException("Authenticated user id was not found.");

        return userId;
    }

    public static IResult ToProblem(Exception ex)
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
}
