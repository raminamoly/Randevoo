using Randevoo.AdminPanel.Models.Auth;

namespace Randevoo.AdminPanel.Models.Users;

public sealed class UserUpsertInput
{
    public string FullName { get; set; } = string.Empty;

    public string Mobile { get; set; } = string.Empty;

    public AdminRole Role { get; set; } = AdminRole.EventPlanner;

    public bool IsActive { get; set; } = true;
}

