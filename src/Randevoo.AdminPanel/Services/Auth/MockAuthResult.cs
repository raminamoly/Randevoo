using Randevoo.AdminPanel.Models.Auth;

namespace Randevoo.AdminPanel.Services.Auth;

public sealed record MockAuthResult(bool Success, string? ErrorMessage, MockUser? User)
{
    public static MockAuthResult Ok(MockUser user) => new(true, null, user);

    public static MockAuthResult Fail(string message) => new(false, message, null);
}

