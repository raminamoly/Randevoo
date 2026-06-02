using MediatR;
using Randevoo.Application.Features.Users.Commands.ChangeUserRole;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;

namespace Randevoo.WebApi.Endpoints;

public static class UserAdminEndpoints
{
    public static RouteGroupBuilder MapUserAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/users")
            .RequireAuthorization("AdminOnly")
            .WithTags("Admin Users");

        group.MapPut("/{userId:long}/role", ChangeRoleAsync).WithName("ChangeUserRole");
        return group;
    }

    private static async Task<IResult> ChangeRoleAsync(long userId, ChangeUserRoleRequest request, System.Security.Claims.ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new ChangeUserRoleCommand(EndpointHelpers.GetUserId(principal), userId, request.Role), cancellationToken);
            return Results.NoContent();
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    public record ChangeUserRoleRequest(UserRole Role);
}
