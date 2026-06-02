namespace Randevoo.ControlCenter.Models.Auth;

public sealed record MockUser(
    Guid Id,
    string DisplayName,
    string MobileNumber,
    ControlCenterRole Role);
