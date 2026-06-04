namespace Randevoo.AdminPanel.Models.Auth;

public sealed class MockUser
{
    public long Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Mobile { get; set; } = string.Empty;

    public AdminRole Role { get; set; } = AdminRole.EventPlanner;

    public bool IsActive { get; set; } = true;
}
