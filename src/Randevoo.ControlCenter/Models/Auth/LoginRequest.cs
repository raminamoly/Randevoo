namespace Randevoo.ControlCenter.Models.Auth;

public sealed record LoginRequest(string MobileNumber, string SmsCode, ControlCenterRole Role);
