using MediatR;
using System.Security.Claims;
using Randevoo.Application.Features.DatingProfile.Commands.CreateDatingProfile;
using Randevoo.Application.Features.DatingProfile.Commands.UpdateDatingProfile;
using Randevoo.Application.Features.DatingProfile.Queries.GetDatingProfile;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;

namespace Randevoo.WebApi.Endpoints;

public static class DatingProfileEndpoints
{
    public static RouteGroupBuilder MapDatingProfileEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/platform/profile")
            .RequireAuthorization("EndUserOnly")
            .WithTags("Platform Profile");

        group.MapGet("/me", GetMineAsync).WithName("GetMyPlatformProfile");
        group.MapPost("/me", CreateProfileAsync).WithName("CreateMyPlatformProfile");
        group.MapPut("/me", UpdateProfileAsync).WithName("UpdateMyPlatformProfile");

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
                request.HeightCm,
                request.EducationLevel,
                request.Smoking,
                request.Region,
                request.InterestNames,
                request.ZodiacSignId,
                request.PhotoUrls,
                request.PrimaryImageUrl), cancellationToken);

            var profile = await sender.Send(new GetDatingProfileByIdQuery(profileId), cancellationToken);
            return Results.Created("/api/v1/platform/profile/me", profile);
        }
        catch (Exception ex) when (ex is DomainException or InvalidOperationException or UnauthorizedAccessException)
        {
            return ToProblem(ex);
        }
    }

    private static async Task<IResult> GetMineAsync(ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new GetDatingProfileByUserIdQuery(EndpointHelpers.GetUserId(principal)), cancellationToken));
        }
        catch (DomainException ex)
        {
            return ToProblem(ex);
        }
    }

    private static async Task<IResult> UpdateProfileAsync(UpdateDatingProfileRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await sender.Send(new GetDatingProfileByUserIdQuery(EndpointHelpers.GetUserId(principal)), cancellationToken);

            await sender.Send(new UpdateDatingProfileCommand(
                profile.Id,
                request.DisplayName,
                request.DateOfBirth ?? profile.DateOfBirth,
                request.Gender,
                request.Country,
                request.City,
                request.Latitude,
                request.Longitude,
                request.HeightCm,
                request.EducationLevel,
                request.Smoking,
                request.Region,
                request.InterestNames,
                request.ZodiacSignId,
                request.PhotoUrls,
                request.PrimaryImageUrl), cancellationToken);

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
        string DisplayName,
        DateOnly DateOfBirth,
        Gender Gender,
        string Country,
        string City,
        decimal Latitude,
        decimal Longitude,
        int? HeightCm = null,
        EducationLevel EducationLevel = EducationLevel.NotSpecified,
        bool Smoking = false,
        string? Region = null,
        IReadOnlyList<string>? InterestNames = null,
        long? ZodiacSignId = null,
        IReadOnlyList<string>? PhotoUrls = null,
        string? PrimaryImageUrl = null);

    public record UpdateDatingProfileRequest(
        string DisplayName,
        DateOnly? DateOfBirth,
        Gender Gender,
        string Country,
        string City,
        decimal Latitude,
        decimal Longitude,
        int HeightCm,
        EducationLevel EducationLevel,
        bool Smoking,
        string? Region = null,
        IReadOnlyList<string>? InterestNames = null,
        long? ZodiacSignId = null,
        IReadOnlyList<string>? PhotoUrls = null,
        string? PrimaryImageUrl = null);
}
