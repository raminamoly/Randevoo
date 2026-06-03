using System.Security.Claims;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;

namespace Randevoo.AdminPanel.Services.State;

public sealed class CurrentSessionState
{
    public MockUser? CurrentUser { get; private set; }

    public AppLanguage Language { get; private set; } = AppLanguage.Farsi;

    public bool IsRtl => Language == AppLanguage.Farsi;

    public void Refresh(ClaimsPrincipal principal, string? languageCookie)
    {
        Language = AppLanguage.Farsi;

        if (principal.Identity?.IsAuthenticated == true)
        {
            var roleValue = principal.FindFirstValue(ClaimTypes.Role);
            Enum.TryParse(roleValue, ignoreCase: true, out AdminRole role);

            CurrentUser = new MockUser
            {
                Id = Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty,
                FullName = principal.Identity?.Name ?? "کاربر",
                Mobile = principal.FindFirstValue("mobile") ?? string.Empty,
                Role = role,
                IsActive = true
            };
        }
        else
        {
            CurrentUser = null;
        }
    }
}
