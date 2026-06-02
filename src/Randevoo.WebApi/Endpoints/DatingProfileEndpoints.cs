using MediatR;
using System.Security.Claims;
using Randevoo.Application.Features.DatingProfile.Commands.CreateDatingProfile;
using Randevoo.Application.Features.DatingProfile.Commands.DeleteDatingProfile;
using Randevoo.Application.Features.DatingProfile.Commands.UpdateDatingProfile;
using Randevoo.Application.Features.DatingProfile.Queries.GetDatingProfile;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;

namespace Randevoo.WebApi.Endpoints;

public static class DatingProfileEndpoints
{
    public static RouteGroupBuilder MapDatingProfileEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dating-profiles")
            .RequireAuthorization()
            .WithTags("Dating Profiles");

        group.MapPost("/", CreateProfileAsync).WithName("CreateDatingProfile");
        group.MapGet("/{profileId:long}", GetProfileByIdAsync).WithName("GetDatingProfileById");
        group.MapGet("/by-user/{userId:long}", GetProfileByUserIdAsync).WithName("GetDatingProfileByUserId");
        group.MapPut("/{profileId:long}", UpdateProfileAsync).WithName("UpdateDatingProfile");
        group.MapDelete("/{profileId:long}", DeleteProfileAsync).WithName("DeleteDatingProfile");

        return group;
    }

    private static async Task<IResult> CreateProfileAsync(CreateDatingProfileRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            var userId = EndpointHelpers.GetUserId(principal);
            var profileId = await sender.Send(new CreateDatingProfileCommand(
                userId,
                request.DisplayName,
                request.DateOfBirth,
                request.Gender,
                request.Country,
                request.City,
                request.Latitude,
                request.Longitude,
                request.HeightCm), cancellationToken);

            var profile = await sender.Send(new GetDatingProfileByIdQuery(profileId), cancellationToken);
            return Results.Created($"/api/dating-profiles/{profileId}", profile);
        }
        catch (Exception ex) when (ex is DomainException or InvalidOperationException or UnauthorizedAccessException)
        {
            return ToProblem(ex);
        }
    }

    private static async Task<IResult> GetProfileByIdAsync(long profileId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await sender.Send(new GetDatingProfileByIdQuery(profileId), cancellationToken);
            return CanAccessProfile(principal, profile.UserId) ? Results.Ok(profile) : Results.Forbid();
        }
        catch (DomainException ex)
        {
            return ToProblem(ex);
        }
    }

    private static async Task<IResult> GetProfileByUserIdAsync(long userId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            if (!CanAccessProfile(principal, userId))
                return Results.Forbid();

            return Results.Ok(await sender.Send(new GetDatingProfileByUserIdQuery(userId), cancellationToken));
        }
        catch (DomainException ex)
        {
            return ToProblem(ex);
        }
    }

    private static async Task<IResult> UpdateProfileAsync(long profileId, UpdateDatingProfileRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await sender.Send(new GetDatingProfileByIdQuery(profileId), cancellationToken);
            if (!CanAccessProfile(principal, profile.UserId))
                return Results.Forbid();

            await sender.Send(new UpdateDatingProfileCommand(
                profileId,
                request.DisplayName,
                request.Gender,
                request.Country,
                request.City,
                request.Latitude,
                request.Longitude,
                request.HeightCm,
                request.EducationLevel,
                request.Smoking,
                request.Region), cancellationToken);

            return Results.NoContent();
        }
        catch (DomainException ex)
        {
            return ToProblem(ex);
        }
    }

    private static async Task<IResult> DeleteProfileAsync(long profileId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await sender.Send(new GetDatingProfileByIdQuery(profileId), cancellationToken);
            if (!CanAccessProfile(principal, profile.UserId))
                return Results.Forbid();

            await sender.Send(new DeleteDatingProfileCommand(profileId), cancellationToken);
            return Results.NoContent();
        }
        catch (DomainException ex)
        {
            return ToProblem(ex);
        }
    }

    private static bool CanAccessProfile(ClaimsPrincipal principal, long ownerUserId) =>
        EndpointHelpers.IsAdmin(principal) || EndpointHelpers.GetUserId(principal) == ownerUserId;

    private static IResult ToProblem(Exception ex)
    {
        return ex switch
        {
            NotFoundException => Results.Problem(ex.Message, statusCode: StatusCodes.Status404NotFound),
            BusinessRuleViolationException => Results.Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest),
            InvalidEntityStateException => Results.Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest),
            _ => Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    public record CreateDatingProfileRequest(
        string DisplayName,
        DateOnly DateOfBirth,
        Gender Gender,
        string Country,
        string City,
        decimal Latitude,
        decimal Longitude,
        int? HeightCm = null);

    public record UpdateDatingProfileRequest(
        string DisplayName,
        Gender Gender,
        string Country,
        string City,
        decimal Latitude,
        decimal Longitude,
        int HeightCm,
        EducationLevel EducationLevel,
        bool Smoking,
        string? Region = null);
}
