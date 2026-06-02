namespace Randevoo.Application.Features.Privacy.Common;

public record PrivacyExportDto(
    long UserId,
    string MobileNumber,
    string? Email,
    bool IsEmailConfirmed,
    string Role,
    object? DatingProfile,
    object? EventPlannerProfile,
    object? Balance,
    IReadOnlyList<object> Tickets);
