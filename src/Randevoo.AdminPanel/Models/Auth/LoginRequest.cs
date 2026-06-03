namespace Randevoo.AdminPanel.Models.Auth;

public sealed class LoginRequest
{
    public string Mobile { get; set; } = string.Empty;

    public string VerificationCode { get; set; } = string.Empty;

    public AdminRole Role { get; set; } = AdminRole.Admin;
}

