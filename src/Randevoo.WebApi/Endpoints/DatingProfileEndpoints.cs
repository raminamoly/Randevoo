using MediatR;
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
            .WithTags("Dating Profiles");

        group.MapPost("/", CreateProfileAsync).WithName("CreateDatingProfile");
        group.MapGet("/{profileId:long}", GetProfileByIdAsync).WithName("GetDatingProfileById");
        group.MapGet("/by-user/{userId:long}", GetProfileByUserIdAsync).WithName("GetDatingProfileByUserId");
        group.MapPut("/{profileId:long}", UpdateProfileAsync).WithName("UpdateDatingProfile");
        group.MapDelete("/{profileId:long}", DeleteProfileAsync).WithName("DeleteDatingProfile");

        return group;
    }

    private static async Task<IResult> CreateProfileAsync(CreateDatingProfileRequest request, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            var profileId = await sender.Send(new CreateDatingProfileCommand(
                request.UserId,
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
        catch (Exception ex) when (ex is DomainException or InvalidOperationException)
        {
            return ToProblem(ex);
        }
    }

    private static async Task<IResult> GetProfileByIdAsync(long profileId, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new GetDatingProfileByIdQuery(profileId), cancellationToken));
        }
        catch (DomainException ex)
        {
            return ToProblem(ex);
        }
    }

    private static async Task<IResult> GetProfileByUserIdAsync(long userId, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new GetDatingProfileByUserIdQuery(userId), cancellationToken));
        }
        catch (DomainException ex)
        {
            return ToProblem(ex);
        }
    }

    private static async Task<IResult> UpdateProfileAsync(long profileId, UpdateDatingProfileRequest request, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
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

    private static async Task<IResult> DeleteProfileAsync(long profileId, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new DeleteDatingProfileCommand(profileId), cancellationToken);
            return Results.NoContent();
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
            NotFoundException => Results.Problem(ex.Message, statusCode: StatusCodes.Status404NotFound),
            BusinessRuleViolationException => Results.Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest),
            InvalidEntityStateException => Results.Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest),
            _ => Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    public record CreateDatingProfileRequest(
        long UserId,
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
